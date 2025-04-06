using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// manages window functionality - moving + resizing

[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
public class UI_Window : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private Button MaximizeButton, CloseButton;
    [SerializeField] private UI_WindowManager windowManagerScript;
    private RectTransform windowTransform;

    private Vector3 minimizedSize = new Vector3(0.5f, 0.57f, 1.0f);
    private Vector3 ogPos;
    private Vector2 ogAnchorMin;
    private Vector2 ogAnchorMax;
    private Vector2 ogSize;
    private bool isMaximized = false;
    public Canvas WindowCanvas { get; private set; }

    Vector3 dragStartposOffset;

    CanvasScaler canvasScaler;

    private void Awake()
    {
        WindowCanvas = GetComponent<Canvas>();
        windowTransform = GetComponent<RectTransform>();
        canvasScaler = GetComponentInParent<CanvasScaler>();

        if (MaximizeButton) MaximizeButton.onClick.AddListener(ToggleMaximize);
        if (CloseButton) CloseButton.onClick.AddListener(ToggleWindow);

        // store the original size, position, and anchors
        ogSize = windowTransform.sizeDelta;
        ogPos = windowTransform.anchoredPosition;
        ogAnchorMin = windowTransform.anchorMin;
        ogAnchorMax = windowTransform.anchorMax;
        windowTransform.localScale = minimizedSize;

        windowManagerScript.RegisterWindow(this);
        
        ToggleWindow();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        windowManagerScript.BringToFront(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isMaximized)
        {
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(windowTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
               
                GetComponent<RectTransform>().anchoredPosition += eventData.delta / (Screen.width / canvasScaler.referenceResolution.x);
            }

            //windowTransform.anchoredPosition += eventData.delta;
        }
    }

    private void ToggleMaximize()
    {
        if (isMaximized)
        {
            // restore to original size   + pos
            windowTransform.anchorMin = ogAnchorMin;
            windowTransform.anchorMax = ogAnchorMax;
            windowTransform.sizeDelta = ogSize;
            windowTransform.localScale = minimizedSize;
            windowTransform.anchoredPosition = ogPos;
        }
        else
        {
            // save current state
            ogSize = windowTransform.sizeDelta;
            ogPos = windowTransform.anchoredPosition;
            ogAnchorMin = windowTransform.anchorMin;
            ogAnchorMax = windowTransform.anchorMax;
            
            // stretch anchors to make it full screen
            windowTransform.anchorMin = Vector2.zero;
            windowTransform.anchorMax = Vector2.one;

            // resets offset
            windowTransform.offsetMin = Vector2.zero;
            windowTransform.offsetMax = Vector2.zero;
            windowTransform.sizeDelta = Vector2.zero;

            //set size
            windowTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        isMaximized = !isMaximized;
    }

    public void ToggleWindow()
    {
        WindowCanvas.enabled = !WindowCanvas.enabled;
        windowManagerScript.BringToFront(this);

    }
}
