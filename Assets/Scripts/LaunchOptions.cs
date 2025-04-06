using UnityEngine;

public class LaunchOptions : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {
        Application.targetFrameRate = 100;
    }
}
