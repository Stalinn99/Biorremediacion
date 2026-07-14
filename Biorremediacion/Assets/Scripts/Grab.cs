using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Grab : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject handPoint;
    public Image crosshair;
    public float grabDistance = 5f;
    public Camera playerCamera;

    [Header("Glove & Tutorial UI")]
    public GameObject gloveVisualCamera;
    public GameObject initialPromptText;

    [Header("Hand Material Swapping")]
    public Renderer leftHandRenderer;
    public Renderer rightHandRenderer;
    public Material defaultHandMat;
    public Material gloveMaterial;

    [Header("Input Settings")]
    public KeyCode infoKey = KeyCode.I;

    private GameObject grabbedObject = null;
    private GameObject hoveredObject = null;
    private Vector3 originalScale;
    private bool isLeftGloveEquipped = false;
    private bool isRightGloveEquipped = false;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (initialPromptText != null) initialPromptText.SetActive(true);
        if (gloveVisualCamera != null) gloveVisualCamera.SetActive(true);

        ResetHandMaterials();
        LockCursor(); // Bloquea el cursor al iniciar el juego
    }

    void Update()
    {
        // 1. Manejar la apertura y cierre del panel de información
        HandleObjectInfo();

        // IMPORTANTE: Si el panel está abierto, bloqueamos el resto del script.
        // Esto evita que el raycast funcione o que un clic accidental agarre cosas.
        if (HUDController.Instance != null && HUDController.Instance.IsPanelOpen())
        {
            return;
        }

        // 2. Detectar objetos interactivos si no tenemos nada agarrado
        if (grabbedObject == null)
        {
            DetectInteractableObject();
        }

        // 3. Actualizar el texto "[I] Info" en la esquina
        UpdateHoverHint();

        // 4. Agarrar o soltar objetos con clic izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            if (grabbedObject == null && hoveredObject != null)
            {
                GrabObject();
            }
            else if (grabbedObject != null)
            {
                ReleaseObject();
            }
        }
    }

    private void HandleObjectInfo()
    {
        if (HUDController.Instance == null) return;

        // Si el panel ESTÁ ABIERTO
        if (HUDController.Instance.IsPanelOpen())
        {
            // Cerramos con ESC o con la misma tecla (I)
            if (Input.GetKeyDown(infoKey) || Input.GetKeyDown(KeyCode.Escape))
            {
                HUDController.Instance.CloseInfoPanel();

                // FIX: Unity libera automáticamente el cursor cuando se presiona ESC
                // (comportamiento propio del motor), y lo hace en el mismo frame en el
                // que intentamos volver a bloquearlo, ganando la "carrera" y dejando
                // el cursor libre. Por eso re-bloqueamos un frame después.
                StartCoroutine(RelockCursorNextFrame());
            }
            return;
        }

        // Si el panel ESTÁ CERRADO y presionamos Info sobre un objeto válido
        if (Input.GetKeyDown(infoKey) && hoveredObject != null)
        {
            InformationObject info = hoveredObject.GetComponentInParent<InformationObject>();
            if (info != null)
            {
                HUDController.Instance.OpenInfoPanel(info.displayName, info.size, info.specifications);
                UnlockCursor(); // Pausa el juego y muestra el mouse
            }
        }
    }

    private void UpdateHoverHint()
    {
        // Si no estamos mirando nada, limpiamos el texto
        if (hoveredObject == null)
        {
            if (HUDController.Instance != null) HUDController.Instance.HideHint();
            return;
        }

        // Si estamos mirando un objeto con información, mostramos la "I"
        InformationObject info = hoveredObject.GetComponentInParent<InformationObject>();
        if (info != null && HUDController.Instance != null)
        {
            HUDController.Instance.UpdateHint($"[{infoKey}] Info");
        }
    }

    private void DetectInteractableObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance))
        {
            GameObject hitObject = hit.collider.gameObject;
            bool hasInfo = hitObject.GetComponentInParent<InformationObject>() != null;

            if (hit.collider.CompareTag("Guante_Item") || hit.collider.CompareTag("Grabbable_Object") || hasInfo)
            {
                if (hitObject != hoveredObject)
                {
                    ClearSelection();
                    hoveredObject = hitObject;
                    EnableOutline(hoveredObject);
                    if (crosshair != null) crosshair.color = Color.green;
                }
            }
            else
            {
                ClearSelection();
            }
        }
        else
        {
            ClearSelection();
        }
    }

    private void ClearSelection()
    {
        if (hoveredObject != null)
        {
            DisableOutline(hoveredObject);
            hoveredObject = null;
            if (crosshair != null) crosshair.color = Color.white;
            if (HUDController.Instance != null && !HUDController.Instance.IsPanelOpen())
            {
                HUDController.Instance.HideHint();
            }
        }
    }

    private void GrabObject()
    {
        if (hoveredObject.CompareTag("Guante_Item"))
        {
            string gloveName = hoveredObject.name.ToLower();
            if ((gloveName.Contains("guante1") || gloveName.Contains("left")) && leftHandRenderer != null)
            {
                leftHandRenderer.material = gloveMaterial;
                isLeftGloveEquipped = true;
            }
            else if ((gloveName.Contains("guante2") || gloveName.Contains("right")) && rightHandRenderer != null)
            {
                rightHandRenderer.material = gloveMaterial;
                isRightGloveEquipped = true;
            }
            if (initialPromptText != null) initialPromptText.SetActive(false);
            Destroy(hoveredObject);
            ClearSelection();
            return;
        }

        grabbedObject = hoveredObject;
        originalScale = grabbedObject.transform.localScale;
        DisableOutline(grabbedObject);

        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (grabbedObject.TryGetComponent<Collider>(out Collider col)) col.enabled = false;

        grabbedObject.transform.SetParent(handPoint.transform, true);
        grabbedObject.transform.localPosition = Vector3.zero;
        grabbedObject.transform.localRotation = Quaternion.identity;

        // FIX: antes se hacía "hoveredObject = null;" directamente, sin pasar por
        // ClearSelection(), por lo que el crosshair se quedaba pintado de verde
        // (ya que nadie volvía a ponerlo en blanco) hasta mirar otro objeto válido.
        hoveredObject = null;
        if (crosshair != null) crosshair.color = Color.white;
    }

    private void ReleaseObject()
    {
        if (grabbedObject == null) return;
        if (grabbedObject.TryGetComponent<Collider>(out Collider col)) col.enabled = true;

        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        grabbedObject.transform.SetParent(null);
        grabbedObject = null;
    }

    public void ResetHandMaterials()
    {
        if (leftHandRenderer != null && defaultHandMat != null) leftHandRenderer.material = defaultHandMat;
        if (rightHandRenderer != null && defaultHandMat != null) rightHandRenderer.material = defaultHandMat;
        isLeftGloveEquipped = false;
        isRightGloveEquipped = false;
    }

    private void EnableOutline(GameObject obj)
    {
        if (obj == null) return;
        Outline outline = obj.GetComponent<Outline>() ?? obj.AddComponent<Outline>();
        outline.effectColor = Color.green;
        outline.enabled = true;
    }

    private void DisableOutline(GameObject obj)
    {
        if (obj != null && obj.GetComponent<Outline>()) obj.GetComponent<Outline>().enabled = false;
    }

    // --- Métodos Helper para el Cursor ---
    private void LockCursor()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None; // Libera el mouse para la UI
        Cursor.visible = true;
    }

    // FIX: espera al final del frame antes de re-bloquear el cursor,
    // para no pelear con el auto-unlock que Unity dispara al presionar ESC.
    private IEnumerator RelockCursorNextFrame()
    {
        yield return new WaitForEndOfFrame();
        LockCursor();
    }
}