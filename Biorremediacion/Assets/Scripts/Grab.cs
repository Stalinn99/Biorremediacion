using UnityEngine;
using UnityEngine.UI;

public class Grab : MonoBehaviour
{
    [Header("Configuración de Agarre")]
    public GameObject HandPoint;
    public Image crosshair;
    public float distanciaAgarre = 5f;
    public Camera playerCamera;

    [Header("Sistema de Guantes y UI")]
    public GameObject GuanteVisualCamera;
    public GameObject textoIndicador;

    [Header("Cambio de Materiales (Guantes)")]
    [Tooltip("El SkinnedMeshRenderer de la Mano Izquierda")]
    public Renderer mallaManoIzquierda;
    [Tooltip("El SkinnedMeshRenderer de la Mano Derecha")]
    public Renderer mallaManoDerecha;
    [Tooltip("Material original de la piel/mano desnuda")]
    public Material materialManoDesnuda;
    [Tooltip("Material que simula el guante puesto")]
    public Material materialGuante;

    private GameObject objetoAgarrado = null;
    private GameObject objetoEnAlcance = null;
    private Vector3 escalaOriginal;

    // Variables de control internas
    private bool manoIzquierdaEquipada = false;
    private bool manoDerechaEquipada = false;

    void Awake()
    {

    }

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (textoIndicador != null)
            textoIndicador.SetActive(true);

        // 1. Primero le aplicamos el material de la mano desnuda a ambas mallas
        RestablecerMaterialesManos();

        // 2. Ahora que ya tienen el material inicial puesto, hacemos visible el contenedor
        if (GuanteVisualCamera != null)
        {
            GuanteVisualCamera.SetActive(true);
        }
    }

    void Update()
    {
        if (objetoAgarrado == null)
            DetectarObjetoConRaycast();

        if (Input.GetMouseButtonDown(0))
        {
            if (objetoAgarrado == null && objetoEnAlcance != null)
            {
                Agarrar();
            }
            else if (objetoAgarrado != null)
            {
                Soltar();
            }
        }

        if (objetoAgarrado != null)
            objetoAgarrado.transform.localScale = escalaOriginal;
    }

    private void DetectarObjetoConRaycast()
    {
        int layerMask = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, distanciaAgarre, layerMask))
        {
            if (hit.collider.CompareTag("Guante_Item") || (hit.collider.CompareTag("Grabbable_Object") && manoIzquierdaEquipada && manoDerechaEquipada))
            {
                if (hit.collider.gameObject != objetoEnAlcance)
                {
                    QuitarOutline(objetoEnAlcance);
                    objetoEnAlcance = hit.collider.gameObject;
                    AgregarOutline(objetoEnAlcance);

                    if (crosshair != null)
                        crosshair.color = Color.green;
                }
            }
            else
            {
                LimpiarSeleccion();
            }
        }
        else
        {
            LimpiarSeleccion();
        }
    }

    private void LimpiarSeleccion()
    {
        if (objetoEnAlcance != null)
        {
            QuitarOutline(objetoEnAlcance);
            objetoEnAlcance = null;

            if (crosshair != null)
                crosshair.color = Color.white;
        }
    }

    private void Agarrar()
    {
        bool esGuante = objetoEnAlcance.CompareTag("Guante_Item");
        if (!esGuante)
        {
            if (!manoIzquierdaEquipada || !manoDerechaEquipada)
            {
                Debug.LogWarning("¡No puedes tocar este objeto sin llevar puestos ambos guantes de seguridad!");
                return; // Detiene por completo la ejecución del método Agarrar
            }
        }

        if (esGuante)
        {
            GameObject guanteSuelo = objetoEnAlcance;
            string nombreGuante = guanteSuelo.name.ToLower();

            if (GuanteVisualCamera != null && !GuanteVisualCamera.activeSelf)
            {
                GuanteVisualCamera.SetActive(true);
                if (textoIndicador != null)
                    textoIndicador.SetActive(false);
            }

            if (nombreGuante.Contains("guante1"))
            {
                if (mallaManoIzquierda != null && materialGuante != null)
                {
                    mallaManoIzquierda.material = materialGuante;
                    manoIzquierdaEquipada = true;
                    Debug.Log("¡Guante 1 recogido! Mano izquierda cambiada de color.");
                }
            }
            else if (nombreGuante.Contains("guante2"))
            {
                if (mallaManoDerecha != null && materialGuante != null)
                {
                    mallaManoDerecha.material = materialGuante;
                    manoDerechaEquipada = true;
                    Debug.Log("¡Guante 2 recogido! Mano derecha cambiada de color.");
                }
            }

            LimpiarSeleccion();
            Destroy(guanteSuelo);
            return;
        }

        // =====================================================================
        // Agarre normal de objetos comunes (Solo se alcanzará si pasó el filtro)
        // =====================================================================
        objetoAgarrado = objetoEnAlcance;
        escalaOriginal = objetoAgarrado.transform.localScale;
        QuitarOutline(objetoAgarrado);

        if (crosshair != null)
            crosshair.color = Color.white;

        Rigidbody rb = objetoAgarrado.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (objetoAgarrado.TryGetComponent<Collider>(out Collider col))
            col.enabled = false;

        objetoAgarrado.transform.SetParent(HandPoint.transform, true);

        Transform snapPoint = objetoAgarrado.transform.Find("grip_point");

        if (snapPoint != null)
        {
            objetoAgarrado.transform.localRotation = Quaternion.Inverse(snapPoint.localRotation);

            objetoAgarrado.transform.localPosition = -snapPoint.localPosition;
        }
        else
        {
            objetoAgarrado.transform.localPosition = Vector3.zero;
            objetoAgarrado.transform.localRotation = Quaternion.identity;
        }

        objetoAgarrado.transform.localScale = escalaOriginal;

        objetoEnAlcance = null;
    }

    private void Soltar()
    {
        if (objetoAgarrado == null) return;

        if (objetoAgarrado.TryGetComponent<Collider>(out Collider col))
            col.enabled = true;

        Rigidbody rb = objetoAgarrado.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        objetoAgarrado.transform.SetParent(null);
        objetoAgarrado.transform.localScale = escalaOriginal;
        objetoAgarrado = null;
    }

    public void RestablecerMaterialesManos()
    {
        if (mallaManoIzquierda != null && materialManoDesnuda != null)
            mallaManoIzquierda.material = materialManoDesnuda;

        if (mallaManoDerecha != null && materialManoDesnuda != null)
            mallaManoDerecha.material = materialManoDesnuda;

        manoIzquierdaEquipada = false;
        manoDerechaEquipada = false;
    }

    private void AgregarOutline(GameObject obj)
    {
        if (obj == null) return;
        Outline outline = obj.GetComponent<Outline>();
        if (outline == null)
            outline = obj.AddComponent<Outline>();

        outline.effectColor = Color.green;
        outline.effectDistance = new Vector2(3f, 3f);
        outline.enabled = true;
    }

    private void QuitarOutline(GameObject obj)
    {
        if (obj == null) return;
        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;
    }
}