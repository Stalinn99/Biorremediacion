using UnityEngine;
using UnityEngine.UI;

public class Grab : MonoBehaviour
{
    [Header("Configuración de Agarre")]
    public GameObject HandPointIzquierdo; // AÑADIDO: Punto para el multiparámetro
    public GameObject HandPointDerecho;   // AÑADIDO: Punto para el agarrador
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

    // AÑADIDO: Se separaron las variables para cada mano
    private GameObject objetoAgarradoIzquierdo = null;
    private GameObject objetoAgarradoDerecho = null;
    private GameObject objetoEnAlcance = null;
    private Vector3 escalaOriginalIzquierda;
    private Vector3 escalaOriginalDerecha;

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
        // CORRECCIÓN: Siempre lanzamos el láser para que pueda ver la sonda, 
        // la lógica inteligente de qué resaltar va adentro.
        DetectarObjetoConRaycast();

        if (Input.GetMouseButtonDown(0))
        {
            if (objetoEnAlcance != null && (objetoAgarradoIzquierdo == null || objetoAgarradoDerecho == null))
            {
                Agarrar();
            }
            else
            {
                if (objetoAgarradoDerecho != null)
                    SoltarDerecha();
                else if (objetoAgarradoIzquierdo != null)
                    SoltarIzquierda();
            }
        }

        if (objetoAgarradoIzquierdo != null)
            objetoAgarradoIzquierdo.transform.localScale = escalaOriginalIzquierda;
        
        if (objetoAgarradoDerecho != null)
            objetoAgarradoDerecho.transform.localScale = escalaOriginalDerecha;
    }

    private void DetectarObjetoConRaycast()
    {
        int layerMask = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, distanciaAgarre, layerMask))
        {
            bool esGuante = hit.collider.CompareTag("Guante_Item");
            bool esMulti = hit.collider.CompareTag("Grabbable_Object");
            bool esAgarrador = hit.collider.CompareTag("Agarrador");
            bool esSonda = hit.collider.CompareTag("Sonda");

            // Lógica inteligente: Solo se puede resaltar si es válido interactuar con ello AHORA
            bool sePuedeResaltar = false;

            if (esGuante) 
            {
                sePuedeResaltar = true;
            }
            else if (manoIzquierdaEquipada && manoDerechaEquipada)
            {
                // Si la mano izquierda está libre, resalta el multi
                if (esMulti && objetoAgarradoIzquierdo == null) sePuedeResaltar = true; 
                // Si la mano derecha está libre, resalta el palo
                else if (esAgarrador && objetoAgarradoDerecho == null) sePuedeResaltar = true; 
                // Si TIENES el palo en la mano, resalta la sonda
                else if (esSonda && objetoAgarradoDerecho != null) sePuedeResaltar = true; 
            }

            if (sePuedeResaltar)
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
        GameObject objetoAProcesar = objetoEnAlcance;
        GameObject puntoManoDestino = null;

        // AÑADIDO: Decide a qué mano va dependiendo del Tag
        if (objetoAProcesar.CompareTag("Grabbable_Object"))
        {
            if (objetoAgarradoIzquierdo != null) return;
            objetoAgarradoIzquierdo = objetoAProcesar;
            escalaOriginalIzquierda = objetoAProcesar.transform.localScale;
            puntoManoDestino = HandPointIzquierdo;
        }
        else if (objetoAProcesar.CompareTag("Agarrador"))
        {
            if (objetoAgarradoDerecho != null) return;
            objetoAgarradoDerecho = objetoAProcesar;
            escalaOriginalDerecha = objetoAProcesar.transform.localScale;
            puntoManoDestino = HandPointDerecho;

            // CONEXIÓN NUEVA: Le avisamos al script propio del aparato que lo acabamos de equipar
            AgarradorTelescopico scriptAparato = objetoAProcesar.GetComponent<AgarradorTelescopico>();
            if (scriptAparato != null)
            {
                scriptAparato.SetEquipado(true, playerCamera);
            }
        }
        else if (objetoAProcesar.CompareTag("Sonda"))
        {
            // MODIFICADO: Si es una sonda, evitamos que se intente procesar con clic izquierdo,
            // ya que la recolección la maneja el script del brazo telescópico mediante el clic derecho.
            return;
        }

        if (puntoManoDestino == null) return;

        QuitarOutline(objetoAProcesar);

        if (crosshair != null)
            crosshair.color = Color.white;

        // TU LÓGICA DE FÍSICAS ORIGINAL INTACTA
        Rigidbody rb = objetoAProcesar.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // CORRECCIÓN MÍNIMA: Apagar todos los colliders hijos (evita caer del mapa)
        Collider[] todosLosColisionadores = objetoAProcesar.GetComponentsInChildren<Collider>();
        foreach (Collider col in todosLosColisionadores)
        {
            col.enabled = false;
        }

        objetoAProcesar.transform.SetParent(puntoManoDestino.transform, true);

        Transform snapPoint = objetoAProcesar.transform.Find("grip_point");

        if (snapPoint != null)
        {
            objetoAProcesar.transform.localRotation = Quaternion.Inverse(snapPoint.localRotation);
            objetoAProcesar.transform.localPosition = -snapPoint.localPosition;
        }
        else
        {
            objetoAProcesar.transform.localPosition = Vector3.zero;
            objetoAProcesar.transform.localRotation = Quaternion.identity;
        }

        objetoEnAlcance = null;
    }

    // AÑADIDO: Método Soltar dividido para Izquierda basado en tu código
    private void SoltarIzquierda()
    {
        if (objetoAgarradoIzquierdo == null) return;

        Collider[] todosLosColisionadores = objetoAgarradoIzquierdo.GetComponentsInChildren<Collider>();
        foreach (Collider col in todosLosColisionadores)
        {
            col.enabled = true;
        }

        Rigidbody rb = objetoAgarradoIzquierdo.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        objetoAgarradoIzquierdo.transform.SetParent(null);
        objetoAgarradoIzquierdo.transform.localScale = escalaOriginalIzquierda;
        objetoAgarradoIzquierdo = null;
    }

    // AÑADIDO: Método Soltar dividido para Derecha basado en tu código
    private void SoltarDerecha()
    {
        if (objetoAgarradoDerecho == null) return;

        // CONEXIÓN NUEVA: Le avisamos al aparato que lo soltamos para que apague sus funciones mecánicas
        AgarradorTelescopico scriptAparato = objetoAgarradoDerecho.GetComponent<AgarradorTelescopico>();
        if (scriptAparato != null)
        {
            scriptAparato.SetEquipado(false, null);
        }

        Collider[] todosLosColisionadores = objetoAgarradoDerecho.GetComponentsInChildren<Collider>();
        foreach (Collider col in todosLosColisionadores)
        {
            col.enabled = true;
        }

        Rigidbody rb = objetoAgarradoDerecho.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        objetoAgarradoDerecho.transform.SetParent(null);
        objetoAgarradoDerecho.transform.localScale = escalaOriginalDerecha;
        objetoAgarradoDerecho = null;
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
