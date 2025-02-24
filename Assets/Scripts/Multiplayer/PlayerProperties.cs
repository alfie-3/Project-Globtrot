using System.Collections.Generic;
using Unity.Services.Multiplayer;
using UnityEngine;

public static class PlayerProperties
{
    public const string PlayerNamePropertyKey = "playerName";
    public const string PlayerCharacterTypePropertyKey = "playerCharacterType";

    public static Dictionary<string, PlayerProperty> GetPlayerProperties()
    {
        Dictionary<string, PlayerProperty> playerProperties = new();

        var playerNameProperty = new PlayerProperty(PlayerProfile.PlayerName, VisibilityPropertyOptions.Member);

        playerProperties.Add(PlayerNamePropertyKey, playerNameProperty);

        var playerCharacterTypeProperty = new PlayerProperty(PlayerProfile.CharacterReferenceData.PlayerReferenceID, VisibilityPropertyOptions.Member); 

        playerProperties.Add(PlayerCharacterTypePropertyKey, playerCharacterTypeProperty);

        return playerProperties;
    }
}
