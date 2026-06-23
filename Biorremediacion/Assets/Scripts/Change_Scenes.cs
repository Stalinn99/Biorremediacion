using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Change_Scenes : MonoBehaviour
{
    public void IrAlJuego()
    {
        // Hacemos que este objeto sobreviva a la carga para que la corrutina no se muera
        DontDestroyOnLoad(gameObject);
        StartCoroutine(CargarNivelComplementario());
    }

    private IEnumerator CargarNivelComplementario()
    {
        // 1. Cargamos primero el terreno en modo Single para limpiar el menú
        AsyncOperation cargaTerreno = SceneManager.LoadSceneAsync("DemoScene", LoadSceneMode.Single);
        while (!cargaTerreno.isDone)
        {
            yield return null;
        }

        // 2. Con el terreno ya cargado, traemos de inmediato al jugador de forma Aditiva
        AsyncOperation cargaJugador = SceneManager.LoadSceneAsync("SampleScene 1", LoadSceneMode.Additive);
        while (!cargaJugador.isDone)
        {
            yield return null;
        }

        Debug.Log("¡Ambas escenas cargadas y ejecutándose juntas!");

        // 3. Una vez terminado el trabajo, destruimos este objeto manager para no dejar basura en el juego
        Destroy(gameObject);
    }
}