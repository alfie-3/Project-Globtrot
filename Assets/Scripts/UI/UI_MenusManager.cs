using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenusManager : MonoBehaviour
{
    public List<UI_OpenableCanvas> canvases = new List<UI_OpenableCanvas>();
    public Stack<UI_OpenableCanvas> canvasStack = new Stack<UI_OpenableCanvas>();

    [SerializeField] UI_OpenableCanvas startingCanvas;
    [SerializeField] UI_OpenableCanvas loadingScreen;

    public static Action<bool> SetDisplayLoadingScreen = delegate { };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        SetDisplayLoadingScreen = delegate { };
    }

    private void Awake()
    {
        SetDisplayLoadingScreen += DisplayLoadingScreen;
    }

    private void OnDisable()
    {
        SetDisplayLoadingScreen -= DisplayLoadingScreen;
    }

    private void Start()
    {
        foreach (UI_OpenableCanvas canvas in canvases)
        {
            if (canvas == startingCanvas)
            {
                canvas.Canvas.enabled = true;
                canvasStack.Push(canvas);
            }
            else if (canvas.Canvas != null)
                canvas.Canvas.enabled = false;

            canvas.CanvasOpened += OnCanvasOpened;
            canvas.CanvasClosed += OnCanvasClosed;
        }
    }

    public void OnCanvasOpened(UI_OpenableCanvas canvas)
    {
        canvasStack.Peek().Canvas.enabled = false;

        canvasStack.Push(canvas);
        canvas.Canvas.enabled = true;
    }

    public void OnCanvasClosed(UI_OpenableCanvas canvas)
    {
        canvasStack.Pop().Canvas.enabled = false;
        canvasStack.Peek().Canvas.enabled = true;
    }

    public void CloseCurrentCanvas()
    {
        if (canvasStack.Count == 1) return;

        canvasStack.Peek().SetEnabled(false);
    }

    public void DisplayLoadingScreen(bool value)
    {
        if (loadingScreen == null) return;
        loadingScreen.SetEnabled(value);
    }

    private void OnDestroy()
    {
        SetDisplayLoadingScreen -= DisplayLoadingScreen;
    }
}
