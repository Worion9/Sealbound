using UnityEngine;

public class BoostMeAbility : Ability
{
    public readonly int boost;
    public readonly bool doubleMyPower;

    public BoostMeAbility(int boost, bool doubleMyPower = false)
    {
        this.boost = boost;
        this.doubleMyPower = doubleMyPower;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (doubleMyPower)
        {
            card.Power = Mathf.RoundToInt(card.Power * 1.5001f);
            return;
        }

        if (card.isInvulnerable)
        {
            card.power += boost;
            card.DisplayCard.UpdateCardUI();
            return;
        }

        card.Power += boost;
    }
}