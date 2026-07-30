// Capturo los elementos del html
const inputCorreo = document.getElementById('inputCorreo');
const inputClave = document.getElementById('inputClave');
const btnIniciarSesion = document.getElementById('btnIniciarSesion');
const mensajeError = document.getElementById('mensajeError');
const btnToggleClave = document.getElementById('btnToggleClave');

// Mostrar/ ocultar contraseña
btnToggleClave.addEventListener('click', () => {
    const icono = btnToggleClave.querySelector('i');
    if (inputClave.type === 'password') {
        inputClave.type = 'text';
        icono.classList.remove('bx-show');
        icono.classList.add('bx-hide');
    } else {
        inputClave.type = 'password';
        icono.classList.remove('bx-hide');
        icono.classList.add('bx-show');
    }
})
// Cuando le den clic
btnIniciarSesion.addEventListener('click', async() => {

    // limpiar cualquier error previo
    mensajeError.innerText = "";
    // Obtenemos el texto y le quitamos espacios en blanco al inicio y fin
    const correo = inputCorreo.value.trim();
    const clave = inputClave.value.trim();

    // Validaciones
    // Que no esten vacios
    if (correo == "" || clave === "") {
        mensajeError.style.color = "#ff6b6b";
        mensajeError.innerText = "Por favor, ingresa tu correo y contraseña.";
        return;
    }

    // Que sea de la UTPL
    if (!correo.endsWith("@utpl.edu.ec")) {
        mensajeError.style.color = "#ff6b6b";
        mensajeError.innerText = "Acceso denegado. Debes usar correo institucional UTPL.";
        return;
    }

    // Preparación
    // Cambiar el botón para que el usario sepa que está cargando
    btnIniciarSesion.innerText = "Iniciando...";
    btnIniciarSesion.disabled = true;

    // Extraemos solo el nombre del usuario
    const nombreUsuario = correo.split('@')[0];

    // Ejecución
    const loginExitoso = await loginWeb(nombreUsuario, clave);

    if (loginExitoso) {
        mensajeError.style.color = "#4ade80";
        mensajeError.innerText = "!Bienvenido! Cargando simulador...";
        localStorage.setItem("sesion_activa", true);
        setTimeout(() => {
            window.location.href = "https://jefferson-999.itch.io/biorremediacion";
        }, 1000);
    } else {
        // Si falló
        mensajeError.style.color = "#ff6b6b";
        mensajeError.innerText = "Correo o contraseña incorrectos. Intente de nuevo.";
        btnIniciarSesion.innerText = "Iniciar Sesión";
        btnIniciarSesion.disabled = false;
    }
})
