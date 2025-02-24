using UnityEngine.UI;
using UnityEngine;

public class UI_CharacterSelect : MonoBehaviour
{
    [SerializeField] CharacterReferenceData defaultCharacter;

    [SerializeField] Button[] buttons;

    public void Start()
    {
        PlayerProfile.SetCharacter(defaultCharacter);
    }

    public void SetCharacter(CharacterReferenceData character)
    {
        PlayerProfile.SetCharacter(character);
    }

    public void OnButtonClicked(Button button)
    {
        foreach (var item in buttons)
        {
            if (item != button)
                item.interactable = true;
        }

        button.interactable = false;
    }
}
