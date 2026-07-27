using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Grab : MonoBehaviour
{
    [Header("Configuración de Agarre")]
    public GameObject HandPointIzquierdo;
    public GameObject HandPointDerecho;
    public Image crosshair;
    public float grabDistance = 5f;
    public Camera playerCamera;

    [Header("Glove & Tutorial UI")]
    public GameObject gloveVisualCamera;

    [Header("Hand Material Swapping")]
    public Renderer leftHandRenderer;
    public Renderer rightHandRenderer;
    public Material defaultHandMat;
    public Material gloveMaterial;

    private GameObject objetoAgarradoIzquierdo = null;
    private GameObject objetoAgarradoDerecho = null;
    private GameObject objetoEnAlcance = null;
    private Vector3 escalaOriginalIzquierda;
    private Vector3 escalaOriginalDerecha;

    private GameObject hoveredObject = null;
    private bool isLeftGloveEquipped = false;
    private bool isRightGloveEquipped = false;

    [Header("Input Settings")]
    public KeyCode infoKey = KeyCode.I;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (gloveVisualCamera != null) gloveVisualCamera.SetActive(true);

        ResetHandMaterials();
        LockCursor();
    }

    void Update()
    {
        HandleObjectInfo();
        if (HUDController.Instance != null && HUDController.Instance.IsPanelOpen())
        {
            return;
        }

        DetectarObjetoConRaycast();

        if (Input.GetMouseButtonDown(0))
        {
            if (objetoEnAlcance != null && (objetoAgarradoIzquierdo == null || objetoAgarradoDerecho == null))
            {
                Agarrar();
            }
            else
            {
                if (objetoAgarradoDerecho != null)
                    SoltarDerecha();
                else if (objetoAgarradoIzquierdo != null)
                    SoltarIzquierda();
            }
        }

        if (objetoAgarradoIzquierdo != null)
            objetoAgarradoIzquierdo.transform.localScale = escalaOriginalIzquierda;

        if (objetoAgarradoDerecho != null)
            objetoAgarradoDerecho.transform.localScale = escalaOriginalDerecha;
    }

    private void HandleObjectInfo()
    {
        if (HUDController.Instance == null) return;
        if (HUDController.Instance.IsPanelOpen())
        {
            if (Input.GetKeyDown(infoKey) || Input.GetKeyDown(KeyCode.Escape))
            {
                HUDController.Instance.CloseInfoPanel();
                if (crosshair != null) crosshair.gameObject.SetActive(true);
                StartCoroutine(RelockCursorNextFrame());
            }
            return;
        }

        if (Input.GetKeyDown(infoKey) && hoveredObject != null)
        {
            InformationObject info = hoveredObject.GetComponentInParent<InformationObject>();
            if (info != null)
            {
                HUDController.Instance.OpenInfoPanel(info.displayName, info.size, info.specifications);
                if (crosshair != null) crosshair.gameObject.SetActive(false);
                UnlockCursor();
            }
        }
    }

    private void UpdateHoverHint()
    {
        if (hoveredObject == null)
        {
            if (HUDController.Instance != null) HUDController.Instance.HideHint();
            return;
        }
        InformationObject info = hoveredObject.GetComponentInParent<InformationObject>();
        if (info != null && HUDController.Instance != null)
        {
            HUDController.Instance.UpdateHint($"[{infoKey}] Info");
        }
        else if (HUDController.Instance != null && !HUDController.Instance.IsPanelOpen())
        {
            HUDController.Instance.HideHint();
        }
    }

    private void DetectarObjetoConRaycast()
    {
        // Ignora explícitamente la capa Water y la capa Ignore Raycast
        int layerMask = ~((1 << LayerMask.NameToLayer("Water")) | (1 << LayerMask.NameToLayer("Ignore Raycast")));

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            GameObject hitObject = hit.collider.gameObject;
            hoveredObject = hitObject;
            UpdateHoverHint();

            bool esGuante = hit.collider.CompareTag("Guante_Item");
            bool esMulti = hit.collider.CompareTag("Grabbable_Object");
            bool esAgarrador = hit.collider.CompareTag("Agarrador");
            bool esSonda = hit.collider.CompareTag("Sonda");

            bool sePuedeResaltar = false;

            if (esGuante)
            {
                sePuedeResaltar = true;
            }
            else if (isLeftGloveEquipped && isRightGloveEquipped)
            {
                if (esMulti && objetoAgarradoIzquierdo == null) sePuedeResaltar = true;
                else if (esAgarrador && objetoAgarradoDerecho == null) sePuedeResaltar = true;
                else if (esSonda && objetoAgarradoDerecho != null) sePuedeResaltar = true;
            }

            if (sePuedeResaltar)
            {
                if (hitObject != objetoEnAlcance)
                {
                    QuitarOutline(objetoEnAlcance);
                    objetoEnAlcance = hitObject;
                    AgregarOutline(objetoEnAlcance);
                    if (crosshair != null) crosshair.color = Color.green;
                }
            }
            else
            {
                QuitarOutline(objetoEnAlcance);
                objetoEnAlcance = null;
                if (crosshair != null) crosshair.color = Color.white;
            }
        }
        else
        {
            QuitarOutline(objetoEnAlcance);
            objetoEnAlcance = null;
            hoveredObject = null;
            if (crosshair != null) crosshair.color = Color.white;
            if (HUDController.Instance != null && !HUDController.Instance.IsPanelOpen())
            {
                HUDController.Instance.HideHint();
            }
        }
    }

    private void AgregarOutline(GameObject obj)
    {
        if (obj == null) return;
        Outline outline = obj.GetComponent<Outline>() ?? obj.AddComponent<Outline>();
        outline.effectColor = Color.green;
        outline.enabled = true;
    }

    private void QuitarOutline(GameObject obj)
    {
        if (obj != null && obj.GetComponent<Outline>()) obj.GetComponent<Outline>().enabled = false;
    }

    private void Agarrar()
    {
        if (objetoEnAlcance.CompareTag("Guante_Item"))
        {
            string gloveName = objetoEnAlcance.name.ToLower();
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
            Destroy(objetoEnAlcance);
            objetoEnAlcance = null;
            if (crosshair != null) crosshair.color = Color.white;
            return;
        }

        GameObject objetoAProcesar = objetoEnAlcance;
        GameObject puntoManoDestino = null;

        if (objetoAProcesar.CompareTag("Grabbable_Object"))
        {
            if (objetoAgarradoIzquierdo != null) return;
            objetoAgarradoIzquierdo = objetoAProcesar;
            escalaOriginalIzquierda = objetoAProcesar.transform.localScale;
            puntoManoDestino = HandPointIzquierdo;
        }
        else if (objetoAProcesar.CompareTag("Agarrador"))
        {
            if (objetoAgarradoDerecho != null) return;
            objetoAgarradoDerecho = objetoAProcesar;
            escalaOriginalDerecha = objetoAProcesar.transform.localScale;
            puntoManoDestino = HandPointDerecho;

            AgarradorTelescopico scriptAparato = objetoAProcesar.GetComponent<AgarradorTelescopico>();
            if (scriptAparato != null)
            {
                scriptAparato.SetEquipado(true, playerCamera);
            }
        }
        else if (objetoAProcesar.CompareTag("Sonda"))
        {
            return;
        }

        if (puntoManoDestino == null) return;

        QuitarOutline(objetoAProcesar);

        if (crosshair != null)
            crosshair.color = Color.white;

        Rigidbody rb = objetoAProcesar.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider[] todosLosColisionadores = objetoAProcesar.GetComponentsInChildren<Collider>();
        foreach (Collider col in todosLosColisionadores)
        {
            col.enabled = false;
        }

        objetoAProcesar.transform.SetParent(puntoManoDestino.transform, true);

        Transform snapPoint = objetoAProcesar.transform.Find("grip_point");
        if (snapPoint != null)
        {
            objetoAProcesar.transform.localRotation = Quaternion.Inverse(snapPoint.localRotation);
            objetoAProcesar.transform.localPosition = -snapPoint.localPosition;
        }
        else
        {
            objetoAProcesar.transform.localPosition = Vector3.zero;
            objetoAProcesar.transform.localRotation = Quaternion.identity;
        }

        objetoEnAlcance = null;
    }

    private void SoltarIzquierda()
    {
        if (objetoAgarradoIzquierdo == null) return;

        Collider[] todosLosColisionadores = objetoAgarradoIzquierdo.GetComponentsInChildren<Collider>();
        foreach (Collider col in todosLosColisionadores)
        {
            col.enabled = true;
        }

        Rigidbody rb = objetoAgarradoIzquierdo.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        objetoAgarradoIzquierdo.transform.SetParent(null);
        objetoAgarradoIzquierdo.transform.localScale = escalaOriginalIzquierda;
        objetoAgarradoIzquierdo = null;
    }

    private void SoltarDerecha()
    {
        if (objetoAgarradoDerecho == null) return;

        AgarradorTelescopico scriptAparato = objetoAgarradoDerecho.GetComponent<AgarradorTelescopico>();
        if (scriptAparato != null)
        {
            scriptAparato.SetEquipado(false, null);
        }

        Collider[] todosLosColisionadores = objetoAgarradoDerecho.GetComponentsInChildren<Collider>();
        foreach (Collider col in todosLosColisionadores)
        {
            col.enabled = true;
        }

        Rigidbody rb = objetoAgarradoDerecho.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        objetoAgarradoDerecho.transform.SetParent(null);
        objetoAgarradoDerecho.transform.localScale = escalaOriginalDerecha;
        objetoAgarradoDerecho = null;
    }

    public void ResetHandMaterials()
    {
        if (leftHandRenderer != null && defaultHandMat != null) leftHandRenderer.material = defaultHandMat;
        if (rightHandRenderer != null && defaultHandMat != null) rightHandRenderer.material = defaultHandMat;
        isLeftGloveEquipped = false;
        isRightGloveEquipped = false;
    }

    private void LockCursor()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator RelockCursorNextFrame()
    {
        yield return new WaitForEndOfFrame();
        LockCursor();
    }
}