using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// manages window functionality - moving + resizing

[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
public class UI_Window : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler
{
    [SerializeField] private Button MaximizeButton, CloseButton;

    private UI_WindowManager windowManager;

    RectTransform canvasRectTransform => windowManager.RectTransform;
    RectTransform panelRectTransform => transform as RectTransform;

    bool clampedToBottom = true;
    bool clampedToTop = true;
    bool clampedToLeft = true;
    bool clampedToRight = true;

    private RectTransform windowTransform;

    [SerializeField] private Vector3 minimizedSize = new Vector3(0.5f, 0.57f, 1.0f);
    private Vector3 ogPos;
    private Vector2 ogAnchorMin;
    private Vector2 ogAnchorMax;
    private Vector2 ogSize;
    private bool isMaximized = false;
    public Canvas WindowCanvas { get; private set; }

    Vector2 pointerOffset = Vector2.one;

    CanvasScaler canvasScaler;

    private void Awake()
    {
        WindowCanvas = GetComponent<Canvas>();
        windowTransform = GetComponent<RectTransform>();
        canvasScaler = GetComponentInParent<CanvasScaler>();

        if (MaximizeButton) MaximizeButton.onClick.AddListener(ToggleMaximize);
        if (CloseButton) CloseButton.onClick.AddListener(() => SetWindowEnabled(!WindowCanvas.enabled));

        // store the original size, position, and anchors
        ogSize = windowTransform.sizeDelta;
        ogPos = windowTransform.anchoredPosition;
        ogAnchorMin = windowTransform.anchorMin;
        ogAnchorMax = windowTransform.anchorMax;
        windowTransform.localScale = minimizedSize;
    }

    public void RegisterWindow(UI_WindowManager windowManager)
    {
        this.windowManager = windowManager;
        SetWindowEnabled(false);

        panelRectTransform.localPosition = Vector3.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        windowManager.BringToFront(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ClampToWindow();
        panelRectTransform.SetAsLastSibling();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, eventData.position, eventData.pressEventCamera, out pointerOffset);

    }

    public void OnDrag(PointerEventData eventData)
    {

        if (panelRectTransform == null)
        {
            return;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPointerPosition))
        {
            panelRectTransform.localPosition = localPointerPosition - (pointerOffset * panelRectTransform.localScale.x);
            ClampToWindow();
            Vector2 clampedPosition = panelRectTransform.localPosition;
            if (clampedToRight)
            {
                clampedPosition.x = (canvasRectTransform.rect.width * 0.5f) - (panelRectTransform.localScale.x * panelRectTransform.rect.width * (1 - panelRectTransform.pivot.x));
            }
            else if (clampedToLeft)
            {
                clampedPosition.x = (-canvasRectTransform.rect.width * 0.5f) + (panelRectTransform.localScale.x * panelRectTransform.rect.width * panelRectTransform.pivot.x);
            }

            if (clampedToTop)
            {
                clampedPosition.y = (canvasRectTransform.rect.height * 0.5f) - (panelRectTransform.localScale.y * panelRectTransform.rect.height * (1 - panelRectTransform.pivot.y));
            }
            else if (clampedToBottom)
            {
                clampedPosition.y = (-canvasRectTransform.rect.height * 0.5f) + (panelRectTransform.localScale.y * panelRectTransform.rect.height * panelRectTransform.pivot.y);
            }
            panelRectTransform.localPosition = clampedPosition;
        }
    }

    void ClampToWindow()
    {
        Vector3[] canvasCorners = new Vector3[4];
        Vector3[] panelRectCorners = new Vector3[4];
        canvasRectTransform.GetWorldCorners(canvasCorners);
        panelRectTransform.GetWorldCorners(panelRectCorners);

        if (panelRectCorners[2].x > canvasCorners[2].x)
        {
            if (!clampedToRight)
            {
                clampedToRight = true;
            }
        }
        else if (clampedToRight)
        {
            clampedToRight = false;
        }
        else if (panelRectCorners[0].x < canvasCorners[0].x)
        {
            if (!clampedToLeft)
            {
                clampedToLeft = true;
            }
        }
        else if (clampedToLeft)
        {
            clampedToLeft = false;
        }

        if (panelRectCorners[2].y > canvasCorners[2].y)
        {
            if (!clampedToTop)
            {
                clampedToTop = true;
            }
        }
        else if (clampedToTop)
        {
            clampedToTop = false;
        }
        else if (panelRectCorners[0].y < canvasCorners[0].y)
        {
            if (!clampedToBottom)
            {
                clampedToBottom = true;
            }
        }
        else if (clampedToBottom)
        {
            clampedToBottom = false;
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

    public void ToggleWindowEnabled()
    {
        SetWindowEnabled(!WindowCanvas.enabled);
    }

    public void SetWindowEnabled(bool value)
    {
        WindowCanvas.enabled = value;

        if (value)
            windowManager.BringToFront(this);
    }
}
