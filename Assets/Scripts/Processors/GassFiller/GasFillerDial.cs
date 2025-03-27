using UnityEngine;

public class GasFillerDial : MonoBehaviour
{
    [SerializeField] GasFillerController controller;
    [SerializeField] Vector2 rotationalMinMax;

    private void Awake()
    {
        controller.OnFillAmountChanged += SetRotation;
    }

    public void SetRotation(float currentFillAmount)
    {
        Vector3 newRotation = Vector3.zero;

        newRotation.z = Mathf.Lerp(rotationalMinMax.x, rotationalMinMax.y, currentFillAmount);

        transform.localRotation = Quaternion.Euler(newRotation);
    }
}
