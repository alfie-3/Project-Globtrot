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

    private void Start()
    {
        canvas = GetComponent<Canvas>();
    }
    public void StartAnim(float duration)
    {
        canvas.enabled = true;
        StartCoroutine(charging(duration));
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

    public void Thrown()
    {
        StopAllCoroutines();
        canvas.enabled = false;
        //Invoke("Thrown", 2f);
    }

    /*IEnumerator charging(float duration)
    {
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime / duration;
            Image.fillAmount = time;
            yield return null;
        }
    }*/
}
