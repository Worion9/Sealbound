using UnityEngine;

public class IncreaseOrDecreasePointsToWinAbility : Ability
{
    private readonly int pointsChange;
    private readonly int selfBoost;

    public IncreaseOrDecreasePointsToWinAbility(int pointsChange, int selfBoost)
    {
        this.pointsChange = pointsChange;
        this.selfBoost = selfBoost;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (playerResources.Row1List.Contains(card))
        {
            playerResources.turnsLogic.ChangeBaseHP(pointsChange);
        }
        else if (playerResources.Row2List.Contains(card))
        {
            card.Power += selfBoost;
        }
        else if (playerResources.Row3List.Contains(card))
        {
            playerResources.turnsLogic.ChangeBaseHP(-pointsChange);
        }
        else
        {
            Debug.LogWarning("Card not found in any list.");
        }
    }
}