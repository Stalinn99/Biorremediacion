# Menú Principal y Flujo de Escenas

Se agregaron nuevas carpetas en la raíz del proyecto (`Assets/`) para separar la lógica y los recursos propios del menú principal de los assets externos del paquete `LowPolyNature`.

---

# 1. Estructura de Prefabs (`Assets/Prefabs_UI_Menu_Principal/`)

Esta carpeta contiene los elementos visuales e interactivos reutilizables del menú principal.

## Prefabs incluidos

### `Canvas.prefab`

Contiene toda la estructura visual de la interfaz principal:

- imágenes de fondo
- paneles
- textos
- botones

### `EventSystem.prefab`

Sistema de eventos de Unity encargado de procesar:

- clics del mouse
- navegación UI
- efectos hover/interacción

---

# 2. Lógica y Controladores (`Assets/Scripts_Menu_Principal/`)

Directorio dedicado a los scripts propios relacionados con la interfaz del menú.

## Scripts incluidos

### `Change_Scenes.cs`

Controlador encargado de gestionar la transición entre escenas.

Este script contiene el método asociado al evento `OnClick` del botón **EMPEZAR**, realizando la carga asíncrona de niveles.

---

# 3. Configuración de Carga de Niveles (`SceneManagement`)

El sistema utiliza carga aditiva (`LoadSceneMode.Additive`) para mantener modularizado el mapa y el jugador.

## Flujo de carga

### `DemoScene`

Se carga primero como escena base:

- entorno 3D
- río
- terreno
- elementos del mapa

### `SampleScene`

Se carga de forma aditiva sobre `DemoScene`:

- jugador
- cámara principal
- físicas
- controladores
