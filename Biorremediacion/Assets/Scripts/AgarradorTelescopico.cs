using UnityEngine;

public class AgarradorTelescopico : MonoBehaviour
{
    [Header("Configuración de la Punta")]
    [Tooltip("El objeto vacío situado en las pinzas de la punta")]
    public Transform puntaAgarrador;
    [Tooltip("Distancia a la que la punta puede detectar una sonda en el suelo")]
    public float distanciaDeteccionSonda = 4f;

    // Ajuste exclusivo para la sonda de oxígeno
    [Header("Ajuste Especial Sonda Oxígeno")]
    [Tooltip("Mueve estos valores en Play para centrar la sonda de O2 (ej: X: -0.2, Y: 0.1)")]
    public Vector3 offsetOxigeno = new Vector3(0f, 0f, 0f);

    private bool estaEquipado = false;
    private Camera camaraJugador;
    private GameObject sondaAcoplada = null;
    private Vector3 escalaOriginalSonda;

    void Update()
    {
        // Si el aparato está tirado en el suelo, no procesa clics ni hace nada
        if (!estaEquipado) return;

        // Equipar la sonda
        if (Input.GetMouseButtonDown(1)) // Clic derecho del mouse
        {
            if (sondaAcoplada == null)
            {
                DetectarYAcoplarSonda();
            }
            else
            {
                SoltarSonda();
            }
        }

        // Mantiene la escala de la sonda fija mientras esté sujeta
        if (sondaAcoplada != null)
        {
            sondaAcoplada.transform.localScale = escalaOriginalSonda * 6f;
        }
    }

    public void SetEquipado(bool estado, Camera cam)
    {
        estaEquipado = estado;
        camaraJugador = cam;
    }

    private void DetectarYAcoplarSonda()
    {
        if (camaraJugador == null) return;

        // Lanzamos un raycast desde el centro de la pantalla para apuntar a la sonda
        Ray ray = camaraJugador.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        int layerMask = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));

        if (Physics.Raycast(ray, out RaycastHit hit, distanciaDeteccionSonda, layerMask))
        {
            if (hit.collider.CompareTag("Sonda"))
            {
                sondaAcoplada = hit.collider.gameObject;
                escalaOriginalSonda = sondaAcoplada.transform.localScale;

                Rigidbody rbSonda = sondaAcoplada.GetComponent<Rigidbody>();
                if (rbSonda != null)
                {
                    rbSonda.isKinematic = true;
                    rbSonda.useGravity = false;
                }

                Collider[] colliders = sondaAcoplada.GetComponentsInChildren<Collider>();
                foreach (Collider col in colliders)
                {
                    col.isTrigger = true;
                }

                sondaAcoplada.transform.SetParent(puntaAgarrador, false);

                if (sondaAcoplada.name.Contains("OXÍGENO") || sondaAcoplada.name.Contains("OXIGENO"))
                {
                    sondaAcoplada.transform.localPosition = offsetOxigeno;
                }
                else
                {
                    sondaAcoplada.transform.localPosition = Vector3.zero;
                }

                sondaAcoplada.transform.localRotation = Quaternion.identity;

                Debug.Log("¡Sonda acoplada con éxito en la punta del agarrador!");
            }
        }
    }

    public void SoltarSonda()
    {
        if (sondaAcoplada == null) return;

        sondaAcoplada.transform.SetParent(null);
        // Le devolvemos su tamaño original exacto al tirarla al suelo
        sondaAcoplada.transform.localScale = escalaOriginalSonda;

        // Le quitamos el modo Trigger y aseguramos que esté activado para que choque con el piso
        Collider[] colliders = sondaAcoplada.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.isTrigger = false;
            col.enabled = true;
        }

        // Restauramos sus físicas normales
        Rigidbody rb = sondaAcoplada.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        sondaAcoplada = null;
        Debug.Log("Sonda liberada.");
    }
}