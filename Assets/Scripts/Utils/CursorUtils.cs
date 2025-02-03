using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorUtils : MonoBehaviour
{
    public static void LockAndHideCusor()
    {
        Debug.Log("Cursor locked and hidden");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void UnlockAndShowCursor()
    {
        Debug.Log("Cursor unlocked and shown");

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}
