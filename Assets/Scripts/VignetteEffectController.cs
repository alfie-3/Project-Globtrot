using UnityEngine;
using UnityEngine.Rendering;

public class VignetteEffectController : MonoBehaviour
{
    [SerializeField] Volume bathroomVolume;
    [SerializeField] Volume buildingVolume;

    public void Init(GameObject player)
    {
        player.GetComponent<PlayerBathroomHandler>().OnBathroomNeedChange += UpdateBathroomVignetteWeight;
    }

    public void UpdateBathroomVignetteWeight(float normalisedBathroom)
    {
        bathroomVolume.weight = normalisedBathroom;
    }
}
