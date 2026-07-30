using UnityEngine;

public class BillboardText : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Si se deja vacío, buscará automáticamente la cámara principal con la etiqueta MainCamera.")]
    [SerializeField] private Transform camTransform;

    void Start()
    {
        ObtenerCamara();
    }

    void LateUpdate()
    {
        if (camTransform == null)
        {
            ObtenerCamara();
            if (camTransform == null) return;
        }
        transform.rotation = camTransform.rotation * Quaternion.Euler(0, 180, 0);
    }

    private void ObtenerCamara()
    {
        if (camTransform != null) return;

        if (Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }
        else if (Camera.current != null)
        {
            camTransform = Camera.current.transform;
        }
    }
}