using UnityEngine;

public class DealDamageReducedByTurnNumberAbility : Ability
{
    public readonly int damage;

    public DealDamageReducedByTurnNumberAbility(int damage)
    {
        this.damage = damage;
    }

    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        targetedCard.Power -= Mathf.Max(0, damage - TurnsLogic.turnNumber);
    }
}