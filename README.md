# Informe Técnico - Scripts del Proyecto de Biorremediación

**Proyecto:** Simulación de Biorremediación (Unity 3D)

**Hecho por:** Lennin Stalin Salinas Quinche

**Carrera:** Computación - UTPL

**Fecha:** Julio 2026

---

## Lista de scripts

1. Movement.cs
2. Camera_Controller.cs
3. SimpleCameraController.cs
4. WaterZone.cs
5. WaterProbe.cs
6. MultiparameterScreen.cs
7. BottleFill.cs
8. AgarradorTelescopico.cs
9. Grab.cs
10. HUDController.cs
11. InformationObject.cs
12. FormularioUIToolkitController.cs
13. FadeInOut.cs
14. BillboardText.cs
15. GPS_IN_GAME.cs
16. WaterfallAudio.cs
17. Change_Scenes.cs

---

## Movimiento y cámara

**Movement.cs**
Es el script que hace que el jugador camine, salte y gire con el mouse. Usa el Rigidbody para moverse (así que la física la maneja Unity) y detecta si estás tocando el piso para saber si puedes saltar o no.

**Camera_Controller.cs**
Este solo se encarga de mover la cámara hacia arriba/abajo cuando mueves el mouse (el "pitch"). Es chiquito, básicamente un Clamp para que no puedas girar la cámara más allá de cierto ángulo y quedar mirando para atrás.

**SimpleCameraController.cs**
Esta es la cámara de vuelo libre que trae Unity de ejemplo (viene de sus templates oficiales), la dejamos para poder volar por el mapa y revisar cosas en modo edición. No es la cámara que usa el jugador normalmente.

> Nota: Movement.cs mueve al jugador hacia los lados (yaw) y Camera_Controller mueve la cámara arriba/abajo , o sea están divididos entre dos scripts. Funciona bien pero hay que tenerlo claro para no duplicar código de sensibilidad del mouse en otro lado más adelante.

---

## Todo lo relacionado con el agua

**WaterZone.cs**
Este script se pone en los cubos/zonas de agua y define los valores base de pH, oxígeno y conductividad. Cada vez que se lee un valor le suma un numerito random para que no siempre marque exactamente lo mismo como simulando que el agua varía un poco.

**WaterProbe.cs**
Es la sonda que usa el jugador. Cuando toca algo con el tag "Water", agarra el WaterZone de eso, ve qué tipo de sonda es (pH, O2 o Conductividad) y manda ese valor a la pantalla.

**MultiparameterScreen.cs**
La pantalla que muestra los números de la sonda. Usa TextMeshPro para que se vea bien el texto y le pone las unidades (mg/L, µS/cm, etc).

**BottleFill.cs**
Hace que la botella se llene cuando toca el agua. Tiene dos formas de detectarlo: por el trigger normal (OnTriggerStay) y también revisando cada frame con un OverlapSphere, como para asegurarse de que no se quede sin detectar el agua en ningún caso.

> - Como WaterZone genera el valor random cada vez que lo lees si dos scripts leen el mismo valor en el mismo instante pueden salir números distintos entre sí
> - En WaterProbe, el tipo de sonda se escribe a mano como texto ("pH", "O2", etc), entonces si alguien lo escribe mal en el Inspector (con espacio o minúscula) simplemente no va a funcionar y no salta ningún error

---

## Agarrar y manipular objetos

**AgarradorTelescopico.cs**
Es la herramienta que sirve para agarrar las sondas desde lejos. Con clic derecho lanza un Raycast y si le pega a algo con el tag "Sonda" lo engancha en la punta del agarrador. Tiene un ajuste especial nada más para la sonda de oxígeno porque quedaba mal posicionada.

**Grab.cs**
Este es el script más grande y el que maneja todo el sistema de manos del jugador: agarrar guantes, agarrar el agarrador telescópico, resaltar objetos con un outline cuando les apuntas, y también abre el panel de información cuando le das a la tecla de info (I por defecto). Cuando se abre ese panel, pausa el juego con Time.timeScale = 0.

> - Tanto AgarradorTelescopico como Grab identifican ciertos objetos comparando el nombre (por ejemplo si el nombre contiene "OXÍGENO" o "left"/"guante1"). Esto funciona pero es medio frágil, si alguien cambia el nombre del objeto en Unity se rompe el ajuste sin que salte ningún error simplemente deja de funcionar bien

> - Tanto Grab.cs como FormularioUIToolkitController.cs tocan el Time.timeScale por su cuenta si en algún momento se abren los dos casi al mismo tiempo, uno le puede pisar la pausa al otro y quedar el juego en un estado raro

---

## Interfaz, formulario y transiciones

**HUDController.cs**
Es el controlador central del HUD. Tiene un patrón Singleton (esa variable estática Instance) para que cualquier script pueda llamarlo fácil. Maneja los mensajitos de ayuda tipo "[F] Formulario" y el panel de información de los objetos.

**InformationObject.cs**
Un script bien simple, solo guarda el nombre, tamaño y especificaciones de un objeto para que Grab.cs y HUDController lo puedan mostrar en el panel.

**FormularioUIToolkitController.cs**
Maneja el formulario que aparece cuando estás dentro de una zona de agua y le das a F. Usa UI Toolkit (el sistema nuevo de UI de Unity). Pausa el juego, libera el cursor y le pone el foco al primer campo del formulario para que puedas escribir directo.

**FadeInOut.cs**
Sirve para hacer fundidos (fade in / fade out) en pantallas o imágenes del Canvas, usando una lista de "a los cuántos segundos aparece" y "a los cuántos segundos desaparece".

**BillboardText.cs**
Este es chiquito pero útil: sirve para que un texto que está puesto en el mundo (por ejemplo un letrero o una etiqueta flotante sobre algún objeto) siempre quede mirando hacia la cámara, sin importar hacia dónde te muevas. Busca la cámara principal (Camera.main) sola si no le asignas una a mano, y en LateUpdate le copia la rotación de la cámara (con un giro de 180° para que el texto no se vea al revés).

> - Usa `Camera.main` como respaldo si no le asignas la cámara manualmente, y eso internamente hace un `FindGameObjectWithTag("MainCamera")` por dentro de Unity, que es un poco más lento que tener la referencia ya guardada. Como solo la busca si `camTransform` está vacío no debería ser un problema real, pero si se usa este script en muchos textos a la vez convendría asignarle la cámara desde el Inspector en vez de dejar que la busque sola.

> - HUDController usa Singleton pero no tiene DontDestroyOnLoad como el proyecto carga una escena adicional con Change_Scenes, hay que fijarse que el HUDController no se destruya sin querer porque si eso pasa cualquier script que llame a HUDController.Instance va a tirar un error de referencia nula.

---

## GPS simulado

**GPS_IN_GAME.cs**
Convierte la posición del jugador en Unity a coordenadas de latitud/longitud/altitud "reales"

---

## Audio y cambio de escenas

**WaterfallAudio.cs**
Pone a sonar el audio de la cascada con un pequeño retraso (delay), para que combine mejor con la animación del agua cayendo.

**Change_Scenes.cs**
Es el que hace la transición del menú principal al juego. Primero carga la escena del terreno (en modo Single, para limpiar el menú) y después carga la escena del jugador encima (en modo Additive), usando una corrutina para esperar a que cada una termine de cargar.

> Nota: los nombres de las escenas están escritos directo en el código ("DemoScene", "SampleScene 1", con el espacio incluido). Si alguien renombra la escena en Unity esto se rompe y no muestra ningún mensaje de error claro, solo se queda pegado cargando. Se podría mejorar poniendo esos nombres como variables configurables desde el Inspector.

---

## Cómo se conecta todo (el flujo básico)

1. Change_Scenes te lleva del menú al mapa del juego.
2. Movement y Camera_Controller te dejan caminar y mirar alrededor.
3. GPS_IN_GAME va mostrando tu ubicación simulada mientras caminas.
4. Con Grab te puedes poner los guantes y agarrar el AgarradorTelescopico.
5. Con el agarrador puesto, puedes enganchar una sonda.
6. Al meter la sonda en el agua, WaterProbe lee el dato y lo manda a MultiparameterScreen. Al mismo tiempo, si tienes la botella, BottleFill la va llenando.
7. Estando en la zona de agua, puedes abrir el formulario con F (FormularioUIToolkitController) para anotar los datos.
8. En cualquier momento puedes apuntar a un objeto y presionar la tecla de info para ver sus datos (esto lo maneja Grab + HUDController + InformationObject).
9. Los letreros y etiquetas de texto en el mundo (con BillboardText) se van girando solos para quedar siempre mirando hacia donde estés parado.
10. FadeInOut y WaterfallAudio le dan más ambiente a la experiencia.

---

## Conclusión y cosas por mejorar

En general el proyecto funciona bien y cada script cumple su parte, están bastante ordenados por responsabilidad (cada uno hace una sola cosa principalmente). Como ideas para seguir mejorando el código más adelante:

- Ordenar el tema del Time.timeScale para que no lo controlen dos scripts distintos sin saber uno del otro.
- Cambiar las comparaciones por texto (tipo de sonda, nombre de guante, nombre de sonda de oxígeno) por algo más seguro como enums o tags, para no depender de que el nombre esté escrito exactamente igual.
- Revisar que HUDController no se destruya al cambiar de escena.
- Dejar anotado qué tags y layers se necesitan configurar para que todo funcione, porque varios scripts dependen de eso y no se ve a simple vista en el código. Esto ya quedó documentado en la siguiente sección.

---

## Configuración necesaria del proyecto (Tags y Layers)

Esta parte es aparte del código en sí: son las **etiquetas (Tags)** y **capas (Layers)** que hay que tener configuradas en el editor de Unity para que los scripts de arriba funcionen bien. Si alguien clona el repo y no las tiene creadas, varias cosas del juego simplemente no van a funcionar y no va a saltar ningún error que lo explique (porque el código está bien, solo que no encuentra el tag/layer que espera).

### Tags que hay que crear

| Tag                | Para qué sirve                                                                                              | Dónde se usa                                                                           |
| ------------------ | ----------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| `Water`            | Marca los objetos/zonas de agua para que la sonda los detecte                                               | `WaterProbe.cs` (OnTriggerEnter/Exit)                                                  |
| `Sonda`            | Marca las sondas de medición (pH, O2, Conductividad) que se pueden agarrar del suelo                        | `AgarradorTelescopico.cs`, `Grab.cs`                                                   |
| `Agarrador`        | Marca el objeto del agarrador telescópico, para que se pueda agarrar con la mano derecha                    | `Grab.cs`                                                                              |
| `Guante_Item`      | Marca los guantes que el jugador puede recoger para equipárselos                                            | `Grab.cs`                                                                              |
| `Grabbable_Object` | Marca los objetos genéricos que se pueden agarrar con la mano izquierda (por ejemplo la botella de muestra) | `Grab.cs`                                                                              |
| `Player`           | Marca al GameObject del jugador                                                                             | `GPS_IN_GAME.cs` (lo busca automáticamente si no se lo asignas a mano en el Inspector) |

**Cómo crear un Tag nuevo en Unity:**

1. Selecciona cualquier GameObject.
2. En el Inspector, arriba donde dice **Tag**, dale clic y luego **Add Tag...**
3. En la lista de Tags, dale al `+` y escribe el nombre exacto (por ejemplo `Sonda`, tal cual, respetando mayúsculas).
4. Repite para cada tag de la tabla de arriba.
5. Ya con el tag creado, selecciona el objeto correspondiente en la escena (la zona de agua, la sonda, el agarrador, etc) y asígnale su tag desde el Inspector.

> Importante: los nombres tienen que ser **exactamente iguales** a como están escritos en el código (`"Water"`, `"Sonda"`, etc), incluyendo mayúsculas y guion bajo donde corresponda. Si le pones un nombre distinto no va a coincidir y el script no lo va a detectar.

### Layers que hay que tener

| Layer            | Para qué sirve                                                                                                                        | Dónde se usa                         |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------ |
| `Water`          | Se excluye del Raycast al agarrar objetos, para que el jugador no "agarre" el agua por accidente al apuntar a través de ella          | `AgarradorTelescopico.cs`, `Grab.cs` |
| `Ignore Raycast` | Es la capa por defecto de Unity para que ciertos objetos no bloqueen los Raycasts (por ejemplo efectos visuales, triggers invisibles) | `AgarradorTelescopico.cs`, `Grab.cs` |

**Cómo configurar un Layer:**

1. Ve a **Edit > Project Settings > Tags and Layers**.
2. En la lista de **Layers**, busca un espacio vacío (por ejemplo Layer 8 o el que esté libre) y escribe `Water`.
3. `Ignore Raycast` ya viene creado por defecto en Unity, no hay que crearlo, solo hay que asignarlo a los objetos que corresponda (por ejemplo objetos puramente visuales o triggers que no deban bloquear el raycast del agarrador).
4. Selecciona los objetos de agua en la escena y en el Inspector, arriba a la derecha donde dice **Layer**, cámbialo a `Water`.

### Checklist rápido antes de correr el proyecto

- Tag `Water` creado y puesto en las zonas de agua (los mismos objetos que tienen el script `WaterZone.cs`)
- Tag `Sonda` creado y puesto en los objetos de las sondas (pH, O2, Conductividad)
- Tag `Agarrador` creado y puesto en el objeto del agarrador telescópico
- Tag `Guante_Item` creado y puesto en los guantes recogibles
- Tag `Grabbable_Object` creado y puesto en los objetos agarrables con la mano izquierda (ej. botella)
- Tag `Player` puesto en el GameObject del jugador
- Layer `Water` creado y asignado a los objetos de agua
- Objetos que no deban bloquear el Raycast del agarrador puestos en `Ignore Raycast`
