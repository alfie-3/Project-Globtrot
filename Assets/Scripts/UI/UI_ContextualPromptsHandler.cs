using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class UI_ContextualPromptsHandler : MonoBehaviour, IInitPlayerUI
{
    [SerializeField] TextMeshProUGUI BuildPrompt;
    [SerializeField] TextMeshProUGUI InteractPrompt;
    [SerializeField] TextMeshProUGUI ThrowPrompt;
    [SerializeField] TextMeshProUGUI DropPrompt;
    [SerializeField] TextMeshProUGUI GetUpPrompt;
    [Space]
    [SerializeField] TextMeshProUGUI BackPrompt;
    [SerializeField] TextMeshProUGUI RotatePrompt;
    [SerializeField] TextMeshProUGUI DestroyPrompt;
    [SerializeField] TextMeshProUGUI PlacePrompt;

    PlayerCharacterController playerCharacterController;

    private void OnEnable()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        BuildPrompt.gameObject.SetActive(true);
    }

    public void Init(PlayerUI_Manager uiManager)
    {
        if (uiManager.TryGetComponent(out PlayerInteractionManager interactionManager))
        {
            interactionManager.OnSetObjectViewed += (value, ctx) => { ToggleInteractionPromptWithContext(value, ctx); };
        }

        if (uiManager.TryGetComponent(out PlayerHoldingManager holdingManager))
        {
            holdingManager.NetworkedHeldObj.OnValueChanged += (prev, current) => { if (current == null) return; TogglePrompt(current.IsHolding, DropPrompt.gameObject); TogglePrompt(current.IsHolding, ThrowPrompt.gameObject); };
        }

        if (uiManager.TryGetComponent(out PlayerBuildingManager buildingManager))
        {
            buildingManager.OnChangeMode += ChangeBuildMode;
        }

        playerCharacterController = uiManager.GetComponent<PlayerCharacterController>();
    }

    private void Update()
    {
        if (playerCharacterController.ReadyToGetUp)
        {
            GetUpPrompt.gameObject.SetActive(true);
        }
        else if (GetUpPrompt.gameObject.activeSelf)
        {
            GetUpPrompt.gameObject.SetActive(false);
        }
    }

    private void TogglePrompt(bool value, GameObject prompt)
    {
        prompt.SetActive(value);
    }

    private void ToggleInteractionPromptWithContext(bool value, InteractionContext promptWithContext)
    {
        InteractPrompt.gameObject.SetActive(value);
        InteractPrompt.text = $"{promptWithContext.InteractionContextText} <sprite=170>";
    }

    public void ChangeBuildMode(PlayerBuildingManager.mode buildingMode)
    {
        if (buildingMode == PlayerBuildingManager.mode.inactive)
        {
            RotatePrompt.gameObject.SetActive(false);
            PlacePrompt.gameObject.SetActive(false);
            DestroyPrompt.gameObject.SetActive(false);
            BuildPrompt.text = "Build <sprite=164>";
        }
        else
        {
            RotatePrompt.gameObject.SetActive(true);
            PlacePrompt.gameObject.SetActive(true);
            DestroyPrompt.gameObject.SetActive(true);

            BuildPrompt.text = "Exit Build <sprite=164>";
        }

        if (buildingMode == PlayerBuildingManager.mode.selectionMode)
        {
            RotatePrompt.gameObject.SetActive(true);
            BackPrompt.gameObject.SetActive(false);

            PlacePrompt.text = "Select <sprite=19>";
            RotatePrompt.text = "Navigate <sprite=31>";
            DestroyPrompt.text = "Destroy Mode <sprite=150>";
        }
        if (buildingMode == PlayerBuildingManager.mode.placementMode)
        {
            RotatePrompt.gameObject.SetActive(true);
            BackPrompt.gameObject.SetActive(true);

            BackPrompt.text = "Back To Select <sprite=23>";
            PlacePrompt.text = "Place <sprite=19>";
            RotatePrompt.text = "Rotate <sprite=31>";
            DestroyPrompt.text = "Destroy Mode <sprite=150>";
        }
        if (buildingMode == PlayerBuildingManager.mode.destroyMode)
        {
            RotatePrompt.gameObject.SetActive(false);
            BackPrompt.gameObject.SetActive(false);

            DestroyPrompt.text = "Build Mode <sprite=150>";
            PlacePrompt.text = "Destroy <sprite=19>";
        }
    }
}
