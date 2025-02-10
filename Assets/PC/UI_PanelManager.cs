using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//copy of UI_MenusManager - simplified to just do panels

// public class UI_PanelManager : MonoBehaviour
// {
//     public List<GameObject> panels = new List<GameObject>();
//     public Stack<GameObject>  panelStack = new Stack<GameObject>();

//     [SerializeField] GameObject startingCanvas;

//     public static UI_MenusManager Singleton;

//     private void Start()
//     {
//         foreach (GameObject panel in panels)
//         {
//             if (panel == startingCanvas)
//             {
//                 panel.Canvas.enabled = true;
//                 panelStack.Push(canvas);
//             }
//             else
//                 canvas.Canvas.enabled = false;

//             canvas.CanvasOpened += OnCanvasOpened;
//             canvas.CanvasClosed += OnCanvasClosed;
//         }
//     }

//     public void OnCanvasOpened(UI_OpenableCanvas canvas)
//     {
//         panelStack.Peek().Canvas.enabled = false;

//         panelStack.Push(canvas);
//         canvas.Canvas.enabled = true;
//     }

//     public void OnCanvasClosed(UI_OpenableCanvas canvas)
//     {
//         panelStack.Pop().Canvas.enabled = false;
//         panelStack.Peek().Canvas.enabled = true;
//     }

//     public void CloseCurrentCanvas()
//     {
//         if (panelStack.Count == 1) return;

//         panelStack.Peek().SetEnabled(false);
//     }
// }
