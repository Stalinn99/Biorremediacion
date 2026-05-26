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

    private GameObject objetoAgarrado = null;
    private GameObject objetoEnAlcance = null;
    private Vector3 escalaOriginal;
    private bool tieneGuantesPuestos = false;

    void Awake()
    {
        if (GuanteVisualCamera != null)
            GuanteVisualCamera.SetActive(false);
    }

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
        if (textoIndicador != null)
            textoIndicador.SetActive(true);
    }

    void Update()
    {
        if (objetoAgarrado == null)
            DetectarObjetoConRaycast();
        if (Input.GetMouseButtonDown(0))
        {
            if (objetoAgarrado == null && objetoEnAlcance != null)
                Agarrar();
            else if (objetoAgarrado != null)
                Soltar();
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
            if (hit.collider.CompareTag("Grabbable_Object") || hit.collider.CompareTag("Guante_Item"))
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
        if (objetoEnAlcance.CompareTag("Guante_Item"))
        {
            GameObject guanteSuelo = objetoEnAlcance;
            LimpiarSeleccion();
            Destroy(guanteSuelo);
            if (GuanteVisualCamera != null)
                GuanteVisualCamera.SetActive(true);
            if (textoIndicador != null)
                textoIndicador.SetActive(false);
            tieneGuantesPuestos = true;
            return;
        }

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

        objetoAgarrado.transform.SetParent(HandPoint.transform, false);
        objetoAgarrado.transform.localPosition = Vector3.zero;
        objetoAgarrado.transform.localRotation = Quaternion.identity;
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