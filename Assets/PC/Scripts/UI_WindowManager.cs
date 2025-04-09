using System.Collections.Generic;
using UnityEngine;


// initialises window + reorganises sort order 
public class UI_WindowManager : MonoBehaviour
{
    [SerializeField] private List<UI_Window> windows = new List<UI_Window>();
    public Canvas Canvas { get; private set; }
    public RectTransform RectTransform { get; private set; }

    private void Awake()
    {
        Canvas = GetComponent<Canvas>();
        RectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        foreach (var window in windows)
        {
            window.RegisterWindow(this);
        }
    }

    // bring clicked window to the front
    public void BringToFront(UI_Window window)
    {
        if (windows.Contains(window))
        {
            windows.Remove(window);
            windows.Add(window);

            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].WindowCanvas.sortingOrder = i+1;
            }
        }
    }

}
