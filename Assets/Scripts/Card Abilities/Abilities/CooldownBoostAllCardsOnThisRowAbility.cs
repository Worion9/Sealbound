using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CooldownBoostAllCardsOnThisRowAbility : Ability
{
    public readonly int boost;
    public readonly int cooldown;

    public CooldownBoostAllCardsOnThisRowAbility(int boost, int cooldown)
    {
        this.boost = boost;
        this.cooldown = cooldown;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (cooldown > 1)
        {
            card.Counter--;

            if (card.Counter <= 0)
            {
                card.Counter = cooldown;
            }

            else return;
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