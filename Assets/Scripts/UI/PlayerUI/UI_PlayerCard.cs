using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;

public class UI_PlayerCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerNameText;

    public void InitCard(IReadOnlyPlayer player)
    {
        if (player.Properties.TryGetValue(PlayerProperties.PlayerNamePropertyKey, out PlayerProperty property))
        {
            playerNameText.text = property.Value.ToString();
        }

    }
}
