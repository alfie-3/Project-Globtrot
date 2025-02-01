using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenusManager : MonoBehaviour
{
    public List<UI_OpenableCanvas> canvases = new List<UI_OpenableCanvas>();
    public Stack<UI_OpenableCanvas> canvasStack = new Stack<UI_OpenableCanvas>();

    [SerializeField] UI_OpenableCanvas startingCanvas;
    [SerializeField] UI_OpenableCanvas loadingScreen;

    public static UI_MenusManager Singleton;

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;
        else
            Destroy(this);
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
            else
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
        loadingScreen.SetEnabled(value);
    }
}
