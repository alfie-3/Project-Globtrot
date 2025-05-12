using System;
using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    float baseMovementSpeed = 0;
    float movementSpeedMultiplier = 1;

    float baseBathroomNeedMultiplier = 1;
    float bathroomNeedMultiplier = 1;

    private void Awake()
    {
        GameStateManager.OnReset += ResetTemporaryStats;

        baseMovementSpeed = GetComponent<CharacterMovement>().BaseMovementSpeed;
    }

    public void UpdateState(PlayerStatModifier stat, float modifier)
    {
        switch (stat)
        {
            case (PlayerStatModifier.MovementSpeed):
                movementSpeedMultiplier += modifier;
                GetComponent<CharacterMovement>().BaseMovementSpeed = baseMovementSpeed * movementSpeedMultiplier;
                break;
            case (PlayerStatModifier.BathroomNeed):
                bathroomNeedMultiplier += modifier;
                GetComponent<PlayerBathroomHandler>().BathroomNeedMultiplier = bathroomNeedMultiplier;
                break;
        }
    }

    private void ResetTemporaryStats()
    {
        GetComponent<CharacterMovement>().BaseMovementSpeed = baseMovementSpeed;
        GetComponent<PlayerBathroomHandler>().BathroomNeedMultiplier = baseBathroomNeedMultiplier;
    }

    private void OnDestroy()
    {
        GameStateManager.OnReset -= ResetTemporaryStats;
    }
}

public enum PlayerStatModifier
{
    MovementSpeed,
    BathroomNeed

}
