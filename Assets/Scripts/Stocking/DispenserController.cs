using UnityEngine;

public class DispenserController : MonoBehaviour
{
    [SerializeField] Animator dispenserAnimator;

    [SerializeField] bool startDisabled;

    private void Start()
    {
        SetDispenserDisabled(startDisabled);
    }

    public void SetDispenserDisabled(bool value)
    {
        dispenserAnimator.SetBool("Disabled", value);
    }
}
