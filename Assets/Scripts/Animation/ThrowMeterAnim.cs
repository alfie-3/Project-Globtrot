using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Canvas))]
public class ThrowMeterAnim : MonoBehaviour
{
    Canvas canvas;
    [SerializeField] Image Image;
    float maxDuration;
    private void Start()
    {
        canvas = GetComponent<Canvas>();
    }
    public void SetThrowMaxDuration(float duration)
    {
        maxDuration = duration;
    }

    public void ThrowingState(bool throwing)
    {
        canvas.enabled = throwing;
        if (throwing) StartCoroutine(charging(maxDuration)); else StopAllCoroutines();
    }

    IEnumerator charging(float duration)
    {
        float time = 0;
        while (time < 1){
            time += Time.deltaTime / duration;
            Image.fillAmount = time;
            yield return null;
        }
    }

}
