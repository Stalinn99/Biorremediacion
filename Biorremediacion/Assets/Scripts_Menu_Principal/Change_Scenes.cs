using UnityEngine;
using UnityEngine.SceneManagement; 

public class Change_Scenes : MonoBehaviour
{
    public void IrAlJuego()
    {
        // Carga el entorno (El mapa del río)
        SceneManager.LoadScene("DemoScene"); 

        // Carga al personaje or encima del mapa 
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive); 
    }
}