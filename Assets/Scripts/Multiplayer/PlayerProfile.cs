using UnityEngine;

public static class PlayerProfile
{
    public static string PlayerName { get; private set; }

    public static void SetPlayerName(string playerName)
    {
        if (playerName.Length < 3)
        {
            Debug.Log("PlayerNameTooShort");
            return;
        }

        if (playerName.Length > 14)
        {
            Debug.Log("PlayerNameTooLong");
            return;
        }

        PlayerName = playerName;    
    }
}
