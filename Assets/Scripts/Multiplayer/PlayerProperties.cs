using System.Collections.Generic;
using Unity.Services.Multiplayer;
using UnityEngine;

public static class PlayerProperties
{
    public const string PlayerNamePropertyKey = "playerName";

    public static Dictionary<string, PlayerProperty> GetPlayerProperties()
    {
        var playerNameProperty = new PlayerProperty(PlayerProfile.PlayerName, VisibilityPropertyOptions.Member);

        return new Dictionary<string, PlayerProperty> { { PlayerNamePropertyKey, playerNameProperty } };
    }
}
