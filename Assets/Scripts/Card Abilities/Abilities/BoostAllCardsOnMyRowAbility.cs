using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoostAllCardsOnMyRowAbility : Ability
{
    public readonly int boost;
    public readonly bool activateOnPlay;

    public BoostAllCardsOnMyRowAbility(int boost, bool activateOnPlay = true)
    {
        this.boost = boost;
        this.activateOnPlay = activateOnPlay;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (!activateOnPlay && !card.wasAbilityExecuted)
        {
            card.wasAbilityExecuted = true;
            return;
        }

        List<Card> targetRow = card.ParentRowList.ToList();

        if (targetRow != null)
        {
            foreach (Card c in targetRow)
            {
                if (c == card) continue;
                c.Power += boost;
            }
        }
        else
        {
            Debug.LogWarning("Card not found in any row.");
        }
    }
}