using UnityEngine;

public class BottleFill : MonoBehaviour
{
    [Header("Configuración del Líquido")]
    [Tooltip("El objeto 3D del agua interna de la botella")]
    [SerializeField] private GameObject liquidMesh;

    [Header("Estado")]
    public bool isFilled = false;

    private void Start()
    {
        if (liquidMesh != null)
        {
            liquidMesh.SetActive(isFilled);
        }
    }

    private void Update()
    {
        // Si ya está llena, no volvemos a evaluar
        if (isFilled) return;

        // Comprobamos si la botella está dentro de algún Collider en este instante
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.1f);

        foreach (var col in hitColliders)
        {
            WaterZone zona = col.GetComponent<WaterZone>();
            if (zona != null)
            {
                FillBottle();
                break;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isFilled) return;

        WaterZone zonaMuestreo = other.GetComponent<WaterZone>();
        if (zonaMuestreo != null)
        {
            FillBottle();
        }
    }

    public void FillBottle()
    {
        isFilled = true;

        if (liquidMesh != null)
        {
            liquidMesh.SetActive(true);
        }

        Debug.Log($"¡Botella {gameObject.name} llena mientras se sostenía!");
    }
}