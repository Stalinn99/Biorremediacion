using UnityEngine;
using UnityEngine.UIElements;

public class FormularioUIToolkitController : MonoBehaviour
{
    [Header("UI Toolkit Document")]
    [SerializeField] private UIDocument uiDocument;

    [Header("Configuración de Teclas")]
    [SerializeField] private KeyCode teclaAbrir = KeyCode.F;

    private VisualElement root;
    private ScrollView docScrollView;
    private TextField primerCampoInput;
    private Label promptFLabel;
    private Label promptEscLabel;
    private WaterZone zonaActual;
    private bool estaEnZona = false;
    private bool formularioAbierto = false;

    private void OnEnable()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        if (uiDocument == null) return;

        root = uiDocument.rootVisualElement;

        if (root != null)
        {
            docScrollView = root.Q<ScrollView>("doc-scrollview");
            primerCampoInput = root.Q<TextField>("proyecto-field");

            root.style.justifyContent = Justify.Center;
            root.style.alignItems = Align.Center;

            promptFLabel = root.Q<Label>("prompt-f");
            if (promptFLabel == null)
            {
                promptFLabel = CrearEtiquetaPista("[F] Formulario", "prompt-f");
                root.Add(promptFLabel);
            }

            promptEscLabel = root.Q<Label>("prompt-esc");
            if (promptEscLabel == null)
            {
                promptEscLabel = CrearEtiquetaPista("[Esc] Salir", "prompt-esc");
                root.Add(promptEscLabel);
            }

            OcultarFormulario();
            if (promptFLabel != null) promptFLabel.style.display = DisplayStyle.None;
            if (promptEscLabel != null) promptEscLabel.style.display = DisplayStyle.None;
        }
    }

    private void Update()
    {
        if (root == null) return;

        FocusController focusCtrl = root.focusController;
        bool escribiendoEnInput = focusCtrl != null && focusCtrl.focusedElement is TextField;

        if (estaEnZona && Input.GetKeyDown(teclaAbrir) && !escribiendoEnInput)
        {
            if (!formularioAbierto) AbrirFormulario();
            else CerrarFormulario();
        }

        if (formularioAbierto && Input.GetKeyDown(KeyCode.Escape))
        {
            CerrarFormulario();
        }
    }

    public void AbrirFormulario()
    {
        formularioAbierto = true;
        root.style.display = DisplayStyle.Flex;

        if (docScrollView != null)
        {
            docScrollView.style.display = DisplayStyle.Flex;
            docScrollView.scrollOffset = Vector2.zero;
        }

        if (promptFLabel != null) promptFLabel.style.display = DisplayStyle.None;
        if (promptEscLabel != null) promptEscLabel.style.display = DisplayStyle.Flex;

        Time.timeScale = 0f;
        UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        primerCampoInput?.Focus();
    }

    public void CerrarFormulario()
    {
        formularioAbierto = false;
        OcultarFormulario();

        if (estaEnZona && promptFLabel != null)
        {
            root.style.display = DisplayStyle.Flex;
            promptFLabel.style.display = DisplayStyle.Flex;
        }

        Time.timeScale = 1f;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void OcultarFormulario()
    {
        if (docScrollView != null) docScrollView.style.display = DisplayStyle.None;
        if (promptEscLabel != null) promptEscLabel.style.display = DisplayStyle.None;

        if (!estaEnZona)
        {
            if (root != null) root.style.display = DisplayStyle.None;
        }
    }

    private Label CrearEtiquetaPista(string texto, string nombre)
    {
        Label label = new Label(texto);
        label.name = nombre;
        label.style.position = Position.Absolute;
        label.style.top = 25;
        label.style.right = 25;
        label.style.fontSize = 20;
        label.style.color = Color.white;
        label.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.75f);
        label.style.paddingTop = 8;
        label.style.paddingBottom = 8;
        label.style.paddingLeft = 14;
        label.style.paddingRight = 14;
        label.style.borderTopLeftRadius = 6;
        label.style.borderTopRightRadius = 6;
        label.style.borderBottomLeftRadius = 6;
        label.style.borderBottomRightRadius = 6;
        return label;
    }

    private void OnTriggerEnter(Collider other)
    {
        WaterZone zona = other.GetComponent<WaterZone>();
        if (zona != null)
        {
            estaEnZona = true;
            zonaActual = zona;

            if (promptFLabel != null && !formularioAbierto)
            {
                root.style.display = DisplayStyle.Flex;
                promptFLabel.style.display = DisplayStyle.Flex;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        WaterZone zona = other.GetComponent<WaterZone>();
        if (zona != null)
        {
            estaEnZona = false;
            zonaActual = null;

            if (promptFLabel != null) promptFLabel.style.display = DisplayStyle.None;
            CerrarFormulario();
        }
    }
}