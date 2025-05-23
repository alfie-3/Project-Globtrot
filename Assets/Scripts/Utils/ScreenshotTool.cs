using System.IO;
using UnityEngine;

public class ScreenshotTool : MonoBehaviour
{
    [SerializeField] int size = 1;

    [ContextMenu("Screenshot")]
    public void TakeScreenshot()
    {
        CaptureScreenshot(size);
    }

    public static void CaptureScreenshot(int superSize = 1)
    {
        if (!Directory.Exists("Screenshots"))
        {
            Directory.CreateDirectory("Screenshots");
        }

        string fileName = $"Screenshots/Screenshot-{System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.png";

        ScreenCapture.CaptureScreenshot(fileName, superSize);
    }
}
