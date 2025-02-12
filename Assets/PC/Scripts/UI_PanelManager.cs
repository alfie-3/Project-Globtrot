using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//copy of UI_MenusManager - simplified to just handle panels for smaller menus
public class UI_PanelManager : MonoBehaviour
{
    public List<UI_OpenableCanvas> Panels; //panels to manage
    [SerializeField] private UI_OpenableCanvas startingPanel; 
    private Stack<UI_OpenableCanvas> panelStack = new Stack<UI_OpenableCanvas>();

    private void Start()
    {
        // loop through panels + enable starting panel
        foreach (var canvas in Panels)
            canvas.SetEnabled(canvas == startingPanel);

        // if starting panel exists push it onto stack
        if (startingPanel != null)
            panelStack.Push(startingPanel);
    }

    // opens new canvas + pushes onto stack
    public void OpenCanvas(UI_OpenableCanvas canvas)
    {
        // disable the currently open panel (if any)
        if (panelStack.Count > 0)
            panelStack.Peek().SetEnabled(false);

        // push the new canvas onto the stack + enable it
        panelStack.Push(canvas);
        canvas.SetEnabled(true);
    }

    // closes current canvas + returns to previous one
    public void CloseCurrentCanvas()
    {
        if (panelStack.Count <= 1) return;
        panelStack.Pop().SetEnabled(false);
        panelStack.Peek().SetEnabled(true);
    }
}
