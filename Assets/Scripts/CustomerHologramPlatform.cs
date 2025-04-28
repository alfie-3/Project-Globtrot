using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class CustomerHologramPlatform : MonoBehaviour
{
    [SerializeField] CustomerData[] customerData;
    GameObject selectedData;
    Dictionary<string, GameObject> customerDataDict = new Dictionary<string, GameObject>();
    [Space]
    [SerializeField] SplineAnimate splineAnimator;
    [SerializeField] SplineContainer arriveSpline;
    [SerializeField] SplineContainer leaveSpline;
    [SerializeField] float splinePauseTime;
    [Space]
    [SerializeField] OrderPort orderPort;

    private void Awake()
    {
        foreach (CustomerData customerData in customerData)
        {
            customerDataDict.Add(customerData.Name, customerData.Asset);
            customerData.Asset.SetActive(false);
        }

        orderPort.OnOrderAdded.AddListener(() => ActivateRandomHologram());

        orderPort.OnOrderCorrect.AddListener(() => PlayCustomerAnimation(CustomerAnimations.Happy));

        orderPort.OnOrderTimout.AddListener(() => { PlayCustomerAnimation(CustomerAnimations.Upset); RemoveCustomerHologram(3); });
        orderPort.OnOrderIncorrect.AddListener(() => { PlayCustomerAnimation(CustomerAnimations.Upset); RemoveCustomerHologram(3); });

    }

    public void ActivateRandomHologram()
    {
        ActivateHologram(customerData[Random.Range(0, customerData.Length)].Name);
    }

    public void ActivateHologram(string customerName)
    {
        if (customerDataDict.TryGetValue(customerName, out GameObject obj))
        {
            obj.SetActive(true);
            selectedData = obj;
            PlayCustomerAnimation(CustomerAnimations.Hello);

            Vector3 ogScale = obj.transform.localScale;
            obj.transform.localScale = new Vector3(obj.transform.localScale.x * 0.5f, obj.transform.localScale.y * 1.5f, obj.transform.localScale.z * 0.5f);

            obj.transform.DOScale(ogScale, 0.3f).SetEase(Ease.OutElastic);

            splineAnimator.Container = arriveSpline;
            splineAnimator.Restart(true);
        }
    }

    public void PlayCustomerAnimation(CustomerAnimations animation)
    {
        if (selectedData == null) return;

        if (selectedData.TryGetComponent(out Animator animator))
        {
            Debug.Log(animation.ToString());
            animator.Play(animation.ToString());
        }
    }

    public void RemoveCustomerHologram(int delay)
    {
        if (delay == 0)
        {
            RemoveCustomerHologram();
        }
        else
        {
            Invoke(nameof(RemoveCustomerHologram), delay);
        }
    }

    private void RemoveCustomerHologram()
    {
        if (selectedData == null) return;

        Vector3 ogScale = selectedData.transform.localScale;

        splineAnimator.Container = leaveSpline;
        splineAnimator.Restart(true);

        selectedData.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBounce).OnComplete(() => {
            selectedData.gameObject.SetActive(false);
            selectedData.transform.localScale = ogScale;
            selectedData = null;
        });
    }
}

[System.Serializable]
public enum CustomerAnimations
{
    Happy,
    Hello,
    Upset
}

[System.Serializable]
public struct CustomerData
{
    public string Name;
    public GameObject Asset;
}