using UnityEngine;

public class WaterZone : MonoBehaviour
{
    [Header("Niveles Base de este cuerpo de agua")]
    [Tooltip("Ej: 7.0 (Neutro), menor a 7 (Ácido), mayor a 7 (Alcalino)")]
    public float basePH = 7.5f;

    [Tooltip("Oxígeno disuelto en mg/L")]
    public float baseO2 = 8.2f;

    [Tooltip("Conductividad en µS/cm")]
    public float baseConductividad = 300f;

    [Header("Realismo")]
    [Tooltip("Rango de fluctuación (Ej: 0.1 hará que el pH varíe entre 7.4 y 7.6)")]
    public float rangoVariacion = 0.1f;

    // Estas propiedades calculan el valor con el Random.
    public float nivelPH 
    { 
        get { return basePH + Random.Range(-rangoVariacion, rangoVariacion); } 
    }

    public float nivelO2 
    { 
        get { return baseO2 + Random.Range(-rangoVariacion, rangoVariacion); } 
    }

    public float nivelConductividad 
    { 
        get { return baseConductividad + Random.Range(-rangoVariacion, rangoVariacion); } 
    }
}