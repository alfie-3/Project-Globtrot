using System.IO;
using UnityEngine;

public static class ScreenshotTool
{
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
