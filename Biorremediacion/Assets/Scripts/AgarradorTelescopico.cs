using UnityEngine;

public class AgarradorTelescopico : MonoBehaviour
{
    [Header("Configuración de la Punta")]
    [Tooltip("El objeto vacío situado en las pinzas de la punta")]
    public Transform puntaAgarrador;
    [Tooltip("Distancia a la que la punta puede detectar una sonda en el suelo")]
    public float distanciaDeteccionSonda = 4f;

    [Header("Ajuste Especial Sonda Oxígeno")]
    [Tooltip("Mueve estos valores en Play para centrar la sonda de O2 (ej: X: -0.2, Y: 0.1)")]
    public Vector3 offsetOxigeno = new Vector3(0f, 0f, 0f);

    private bool estaEquipado = false;
    private Camera camaraJugador;
    private GameObject sondaAcoplada = null;
    private Vector3 escalaOriginalSonda;

    void Update()
    {
        if (!estaEquipado) return;
        if (Input.GetMouseButtonDown(1))
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
        Ray ray = camaraJugador.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        int layerMask = ~((1 << LayerMask.NameToLayer("Ignore Raycast")) | (1 << LayerMask.NameToLayer("Water")));
        if (Physics.Raycast(ray, out RaycastHit hit, distanciaDeteccionSonda, layerMask, QueryTriggerInteraction.Ignore))
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
        sondaAcoplada.transform.localScale = escalaOriginalSonda;
        Collider[] colliders = sondaAcoplada.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.isTrigger = false;
            col.enabled = true;
        }
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