using UnityEngine;
using UnityEngine.UI;

public class Grab : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject HandPoint;
    public Image crosshair;
    public float distanciaAgarre = 3f;
    public Camera playerCamera;

    private GameObject objetoAgarrado = null;
    private GameObject objetoEnAlcance = null;
    private Vector3 escalaOriginal;

    void Update()
    {
        if (objetoAgarrado == null)
            DetectarObjetoConRaycast();

        if (Input.GetKeyDown(KeyCode.E))
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
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, distanciaAgarre))
        {
            if (hit.collider.CompareTag("Grabbable_Object"))
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