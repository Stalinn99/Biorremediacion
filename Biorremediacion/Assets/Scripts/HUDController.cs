using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI infoDetailsText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // FIX: chequeo temprano para detectar referencias faltantes apenas arranca la escena.
        if (hintText == null)
        {
            Debug.LogWarning("HUDController: el campo 'hintText' NO está asignado en el Inspector. " +
                              "Por eso no se muestra el texto '[I] Info' / '[ESC] Exit'. " +
                              "Arrastra el objeto de texto correspondiente en el Inspector de HUDController.");
        }
        else
        {
            if (!hintText.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("HUDController: el GameObject de 'hintText' está inactivo en la Hierarchy. " +
                                  "Actívalo para que el texto pueda mostrarse.");
            }

            // FIX: si el Scale del RectTransform quedó en 0 (por error humano en el Editor),
            // el texto es invisible aunque todo lo demás esté bien. Lo forzamos a 1,1,1.
            if (hintText.transform.localScale == Vector3.zero)
            {
                Debug.LogWarning("HUDController: el Scale de 'hintText' estaba en (0,0,0) y lo corregí a (1,1,1) automáticamente.");
                hintText.transform.localScale = Vector3.one;
            }
        }

        HideHint();
        CloseInfoPanel();
    }

    public void UpdateHint(string text)
    {
        if (hintText == null)
        {
            Debug.LogWarning("HUDController.UpdateHint: 'hintText' es null, no se puede mostrar: " + text);
            return;
        }
        hintText.text = text;
    }

    public void HideHint()
    {
        if (hintText == null) return;
        hintText.text = "";
    }

    public void OpenInfoPanel(string name, string size, string specifications)
    {
        Debug.Log("Intentando abrir panel para: " + name);
        if (infoPanel != null && infoDetailsText != null)
        {
            infoPanel.SetActive(true);
            Debug.Log("Panel activado: " + infoPanel.activeSelf);
            infoDetailsText.text = $"<b>{name}</b>\nSize: {size}\n\n{specifications}";
            UpdateHint("[ESC] Exit");
        }
        else
        {
            Debug.LogError("¡Faltan referencias en el HUDController! Panel: " + (infoPanel != null) + " Texto: " + (infoDetailsText != null));
        }
    }

    public void CloseInfoPanel()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }

        // Fuerza el bloqueo del cursor aquí para que sea instantáneo
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool IsPanelOpen() => infoPanel != null && infoPanel.activeSelf;

}