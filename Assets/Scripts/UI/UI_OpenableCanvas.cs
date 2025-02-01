using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
public class UI_OpenableCanvas : MonoBehaviour
{
    public Canvas Canvas {  get; private set; }

    public Action<UI_OpenableCanvas> CanvasOpened = delegate { };
    public Action<UI_OpenableCanvas> CanvasClosed = delegate { };

    private void Awake()
    {
        Canvas = GetComponent<Canvas>(); 
    }

    public void SetEnabled(bool value)
    {
        Canvas.enabled = value;

        if (value == false)
            CanvasClosed.Invoke(this);
        else
            CanvasOpened.Invoke(this);
    }
}
