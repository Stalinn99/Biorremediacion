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
        if (hintText == null)
        {
            Debug.LogWarning("HUDController: el campo 'hintText' NO está asignado en el Inspector.");
        }
        else
        {
            if (hintText.transform.localScale == Vector3.zero)
            {
                hintText.transform.localScale = Vector3.one;
            }
        }

        HideHint();
        CloseInfoPanel();
    }

    public void UpdateHint(string text)
    {
        if (hintText == null) return;
        hintText.text = text;
    }

    public void HideHint()
    {
        if (hintText == null) return;
        hintText.text = "";
    }

    public void OpenInfoPanel(string name, string size, string specifications)
    {
        if (infoPanel != null && infoDetailsText != null)
        {
            infoPanel.SetActive(true);
            infoDetailsText.text = $"<b>{name}</b>\nSize: {size}\n\n{specifications}";
            UpdateHint("[ESC] Exit");
        }
    }

    public void CloseInfoPanel()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool IsPanelOpen() => infoPanel != null && infoPanel.activeSelf;
}