using UnityEngine;

public class GlobalPlayerModifiers : MonoBehaviour
{
    public static float StaminaRechargeSpeedModifier = 1.0f;
    public static float BladderSpeedModifier = 1.0f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        StaminaRechargeSpeedModifier = 1;
        BladderSpeedModifier = 1;
    }

    public static void UpgradeStat(PlayerUpgradeData data)
    {
        switch (data.Upgrade)
        {
            case (PlayerUpgrades.StaminaRechargeSpeed):
                StaminaRechargeSpeedModifier = data.Value; break;
            case (PlayerUpgrades.BladderSpeed): 
                BladderSpeedModifier = data.Value; break;
        }
    }

}

public enum PlayerUpgrades
{
    StaminaRechargeSpeed,
    BladderSpeed
}