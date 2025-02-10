using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class UI_PlayerTag : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] TMP_Text playerIDText;

    NetworkVariable<FixedString64Bytes> PlayerName = new NetworkVariable<FixedString64Bytes>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);

    private void OnEnable()
    {
        PlayerName.OnValueChanged += OnNameUpdate;
    }

    private void OnNameUpdate(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        UpdateNameTag(newValue.ToString());
    }

    private void UpdateNameTag(string newName)
    {
        playerNameText.text = newName;
        playerIDText.text = newName;
    }

    public void UpdateNameTagText()
    {
        if (!string.IsNullOrEmpty(PlayerProfile.PlayerName))
            PlayerName.Value = PlayerProfile.PlayerName;
        else
        {
            PlayerName.Value = AuthenticationService.Instance.PlayerName;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            UpdateNameTagText();
        }
        else
        {
            UpdateNameTag(PlayerName.Value.ToString());
        }
    }
}
