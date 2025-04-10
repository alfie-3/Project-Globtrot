using UnityEngine;

public class UI_EmailFlash : MonoBehaviour
{
    [SerializeField] float flashDuration = 1f;
    float currentTime;
    [Space]
    [SerializeField] Canvas canvas;

    private void OnEnable()
    {
        currentTime = flashDuration;
    }

    private void Update()
    {
        if (currentTime <= 0)
        {
            ToggleCanvas();
            currentTime = flashDuration;
        }

        currentTime -= Time.deltaTime;
    }

    public void ToggleCanvas()
    {
        canvas.enabled = !canvas.enabled;
    }
}
