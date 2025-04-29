using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class CustomerHologramPlatform : MonoBehaviour
{
    [SerializeField] CustomerData[] customerData;
    CustomerData selectedData;
    Dictionary<string, CustomerData> customerDataDict = new Dictionary<string, CustomerData>();
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
            customerDataDict.Add(customerData.Name, customerData);
            customerData.SetGameobjectsEnable(false);
        }

        orderPort.OnOrderAdded.AddListener(() => ActivateRandomHologram());

        orderPort.OnOrderCorrect.AddListener(() => { PlayCustomerAnimation(CustomerAnimations.Happy); RemoveCustomerHologram(3); });

        orderPort.OnOrderTimout.AddListener(() => { PlayCustomerAnimation(CustomerAnimations.Upset); RemoveCustomerHologram(3); });
        orderPort.OnOrderIncorrect.AddListener(() => { PlayCustomerAnimation(CustomerAnimations.Upset); RemoveCustomerHologram(3); });

    }

    public void ActivateRandomHologram()
    {
        ActivateHologram(customerData[Random.Range(0, customerData.Length)].Name);
    }

    public void ActivateHologram(string customerName)
    {
        if (customerDataDict.TryGetValue(customerName, out CustomerData obj))
        {
            obj.CustomerAsset.SetActive(true);
            selectedData = obj;
            PlayCustomerAnimation(CustomerAnimations.Hello);

            Vector3 ogScale = obj.CustomerAsset.transform.localScale;
            obj.CustomerAsset.transform.localScale = new Vector3(obj.CustomerAsset.transform.localScale.x * 0.5f, obj.CustomerAsset.transform.localScale.y * 1.5f, obj.CustomerAsset.transform.localScale.z * 0.5f);

            obj.CustomerAsset.transform.DOScale(ogScale, 0.3f).SetEase(Ease.OutElastic);

            foreach (var item in customerData)
            {
                item.ShipAsset.SetActive(false);
            }

            obj.ShipAsset.SetActive(true);

            splineAnimator.Container = arriveSpline;
            splineAnimator.Restart(true);
        }
    }

    public void PlayCustomerAnimation(CustomerAnimations animation)
    {
        if (selectedData == null) return;

        if (selectedData.CustomerAsset.TryGetComponent(out Animator animator))
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

        Vector3 ogScale = selectedData.CustomerAsset.transform.localScale;

        splineAnimator.Container = leaveSpline;
        splineAnimator.Restart(true);

        selectedData.CustomerAsset.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBounce).OnComplete(() => {
            selectedData.CustomerAsset.gameObject.SetActive(false);
            selectedData.CustomerAsset.transform.localScale = ogScale;
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
public class CustomerData
{
    public string Name;
    [Space]
    public GameObject CustomerAsset;
    public GameObject ShipAsset;

    public void SetGameobjectsEnable(bool value)
    {
        CustomerAsset.SetActive(value);
        ShipAsset.SetActive(value);
    }
}