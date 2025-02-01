using UnityEngine;

public class UI_PlayerNameSetter : MonoBehaviour
{
    public void SetName(string name)
    {
        PlayerProfile.SetPlayerName(name);
    }
}
