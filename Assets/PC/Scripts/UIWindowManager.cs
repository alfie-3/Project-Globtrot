using System.Collections.Generic;
using UnityEngine;


// initialises window + reorganises sort order 
public class UI_WindowManager : MonoBehaviour
{
    private List<UI_Window> windows = new List<UI_Window>();

    // register the window
    public void RegisterWindow(UI_Window window)
    {
        if (!windows.Contains(window))
            windows.Add(window);
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
