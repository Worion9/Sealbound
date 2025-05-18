using UnityEngine;

public class ChangePowerByPercentAbility : Ability
{
    public readonly float Power;

    public ChangePowerByPercentAbility(float power)
    {
        Power = power;
    }

    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        targetedCard.Power += Mathf.RoundToInt(targetedCard.Power * Power / 100 + 0.001f);
    }
}