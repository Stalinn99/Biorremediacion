using UnityEngine;
using TMPro; // Esencial para interactuar con TextMeshPro

public class GPS_IN_GAME : MonoBehaviour
{
    [Header("UI de la Pantalla")]
    public TextMeshProUGUI textoPantalla; // Arrastra aquí tu Texto (TextMeshPro)

    [Header("Referencia del Jugador")]
    public Transform jugador; // Arrastra aquí a tu Personaje (Player)

    [Header("Configuración del Punto de Origen (Simulado)")]
    [Tooltip("Latitud ficticia asignada al punto (0, 0) de tu mapa de Unity")]
    public float latitudOrigen = -4.0016f;
    [Tooltip("Longitud ficticia asignada al punto (0, 0) de tu mapa de Unity")]
    public float longitudOrigen = -79.2012f;
    [Tooltip("Altitud inicial base en metros sobre el nivel del mar")]
    public float altitudBase = 2060f;

    [Header("Factor de Escala Geográfica")]
    [Tooltip("Cuántos metros en Unity equivalen a un grado geográfico (Aprox 111000m por grado)")]
    public float metrosPorGrado = 111000f;

    void Start()
    {
        // Si olvidaste asignar al jugador en el inspector, lo busca de forma automática por su Tag
        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) jugador = playerObj.transform;
        }
    }

    void Update()
    {
        if (jugador == null || textoPantalla == null) return;

        // 1. Obtener la posición actual del jugador en el espacio de Unity
        Vector3 posicionJugador = jugador.position;

        // 2. Convertir el movimiento en Unity (metros) a coordenadas geográficas relativas
        // El eje Z en Unity suele representar el Norte/Sur (Latitud)
        // El eje X en Unity suele representar el Este/Oeste (Longitud)
        float latitudActual = latitudOrigen + (posicionJugador.z / metrosPorGrado);
        float longitudActual = longitudOrigen + (posicionJugador.x / metrosPorGrado);

        // El eje Y en Unity es la altura física
        float altitudActual = altitudBase + posicionJugador.y;

        // 3. Imprimir los datos con formato limpio en el recuadro gris de la pantalla
        // "F5" muestra 5 decimales fijos para que se vea estético y no se desborde del recuadro
        textoPantalla.text = $"<b>" +
                             $"LAT: {latitudActual.ToString("F5")}°\n\n" +
                             $"LON: {longitudActual.ToString("F5")}°\n\n" +
                             $"ALT: {altitudActual.ToString("F1")} m" +
                             $"</b>";
    }
}