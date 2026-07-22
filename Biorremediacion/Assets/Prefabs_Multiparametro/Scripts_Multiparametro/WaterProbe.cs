using UnityEngine;

public class WaterProbe : MonoBehaviour
{
    [Header("Configuración de la Sonda")]
    [Tooltip("Escribe exactamente: pH, O2 o Conductividad")]
    public string tipoSonda; 

    [Header("Referencias")]
    public MultiparameterScreen pantallaMultiparametro;

    private void OnTriggerEnter(Collider other)
    {
        // Detectamos si tocamos algo con la etiqueta "Water"
        if (other.CompareTag("Water"))
        {
            // Extraemos el script "WaterZone" del cubo de agua que tocamos
            WaterZone zonaDeAgua = other.GetComponent<WaterZone>();

            if (zonaDeAgua != null && pantallaMultiparametro != null)
            {
                float valorExtraido = 0f;

                // Dependiendo de qué sonda sea, sacamos un valor u otro del lago
                switch (tipoSonda)
                {
                    case "pH":
                        valorExtraido = zonaDeAgua.nivelPH;
                        break;
                    case "O2":
                        valorExtraido = zonaDeAgua.nivelO2;
                        break;
                    case "Conductividad":
                        valorExtraido = zonaDeAgua.nivelConductividad;
                        break;
                }

                // Mandamos el valor correcto a la pantalla
                pantallaMultiparametro.ActualizarMedicion(tipoSonda, valorExtraido);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Al sacar la sonda del agua, limpiamos la pantalla
        if (other.CompareTag("Water"))
        {
            if (pantallaMultiparametro != null)
            {
                pantallaMultiparametro.ResetearPantalla();
            }
        }
    }
}