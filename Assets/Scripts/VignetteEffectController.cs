using UnityEngine;
using UnityEngine.Rendering;

public class VignetteEffectController : MonoBehaviour
{
    [SerializeField] Volume bathroomVolume;
    [SerializeField] Volume buildingVolume;
    [Space]
    [SerializeField] VolumeProfile buildProfile;
    [SerializeField] VolumeProfile destroyProfile;

    public void Init(GameObject player)
    {
        player.GetComponent<PlayerBathroomHandler>().OnBathroomNeedChange += UpdateBathroomVignetteWeight;
        player.GetComponent<PlayerBuildingManager>().OnChangeMode += UpdateBuildVignette;
    }

    public void UpdateBathroomVignetteWeight(float normalisedBathroom)
    {
        if (normalisedBathroom > 0.75f)
        {
            float clampedBathroom = Remap(normalisedBathroom, 0.75f, 0.85f, 0, 1);
            bathroomVolume.weight = clampedBathroom;
        }
        else
        {
            bathroomVolume.weight = 0;
        }
    }

    public void UpdateBuildVignette(PlayerBuildingManager.mode buildingMode)
    {
        switch (buildingMode)
        {
            case PlayerBuildingManager.mode.selectionMode:
            case PlayerBuildingManager.mode.placementMode:
                buildingVolume.profile = buildProfile;
                buildingVolume.weight = 1;
                break;
            case PlayerBuildingManager.mode.destroyMode:
                buildingVolume.profile = destroyProfile;
                buildingVolume.weight = 1;
                break;
            case PlayerBuildingManager.mode.inactive:
            default:
                buildingVolume.weight = 0;
                break;
        }
    }

    //https://discussions.unity.com/t/re-map-a-number-from-one-range-to-another/465623/12
    //PraetorBlue
    public static float Remap(float input, float oldLow, float oldHigh, float newLow, float newHigh)
    {
        float t = Mathf.InverseLerp(oldLow, oldHigh, input);
        return Mathf.Lerp(newLow, newHigh, t);
    }
}
