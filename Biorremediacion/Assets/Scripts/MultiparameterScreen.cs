using UnityEngine;
using TMPro;

public class MultiparameterScreen : MonoBehaviour
{
    [Header("Conexión con la Interfaz")]
    public TextMeshProUGUI textoPH;
    public TextMeshProUGUI textoO2;
    public TextMeshProUGUI textoCond;

    void Start()
    {
        // Al iniciar la simulación la pantalla muestra guiones 
        // indicando que no hay sondas leyendo datos aún.
        ResetearPantalla();
    }

    public void ActualizarMedicion(string tipoSonda, float valor)
    {
        switch (tipoSonda)
        {
            case "pH":
                textoPH.text = "pH: " + valor.ToString("F2");
                break;
            case "O2":
                textoO2.text = "O2: " + valor.ToString("F2") + " mg/L";
                break;
            case "Conductividad":
                textoCond.text = "Cond: " + valor.ToString("F0") + " µS/cm";
                break;
        }
    }

    // Método para limpiar la pantalla
    public void ResetearPantalla()
    {
        textoPH.text = "pH: --";
        textoO2.text = "O2: --";
        textoCond.text = "Cond: --";
    }
}