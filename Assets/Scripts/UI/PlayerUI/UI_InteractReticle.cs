using UnityEngine;
using UnityEngine.UI;

public class UI_InteractReticle : MonoBehaviour, IInitPlayerUI
{
    [SerializeField] Image ReticleImage;
    [Space]
    [SerializeField] Sprite NormalSprite;
    [SerializeField] Sprite TargetingSprite;

    public void Init(PlayerUI_Manager uiManager)
    {
        if (uiManager.TryGetComponent(out PlayerInteractionManager interactManager))
        {
            interactManager.OnSetObjectViewed += SetTargetingReticle;
        }
    }

    public void SetTargetingReticle(bool value, InteractionContext _)
    {
        if (value)
        {
            ReticleImage.sprite = TargetingSprite;
        }
        else
        {
            ReticleImage.sprite = NormalSprite;
        }
    }
}
