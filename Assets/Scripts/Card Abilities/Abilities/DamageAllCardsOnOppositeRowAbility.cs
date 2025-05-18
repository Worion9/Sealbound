using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageAllCardsOnOppositeRowAbility : Ability
{
    public readonly int damage;
    public readonly bool activateOnPlay;
    public readonly bool repeatOnCardKill;

    public DamageAllCardsOnOppositeRowAbility(int damage, bool activateOnPlay = true, bool repeatOnCardKill = false)
    {
        this.damage = damage;
        this.activateOnPlay = activateOnPlay;
        this.repeatOnCardKill = repeatOnCardKill;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (!activateOnPlay && !card.wasAbilityExecuted)
        {
            card.wasAbilityExecuted = true;
            return;
        }

        void DamageRowAndRepeatIfNecessary()
        {
            List<Card> targetRow = playerResources.Row1List.Contains(card) ? playerResources.myEnemy.Row1List.ToList()
                           : playerResources.Row2List.Contains(card) ? playerResources.myEnemy.Row2List.ToList()
                           : playerResources.Row3List.Contains(card) ? playerResources.myEnemy.Row3List.ToList()
                           : null;

            if (targetRow == null)
            {
                Debug.LogWarning("Card not found in any row.");
                return;
            }

            bool willRepeat = false;

            foreach (Card c in targetRow)
            {
                if (c.isInvulnerable) continue;
                if (c.Power <= damage && repeatOnCardKill)
                {
                    willRepeat = true;
                }
                c.Power -= damage;
            }

            if (willRepeat)
            {
                DamageRowAndRepeatIfNecessary();
            }
        }

        DamageRowAndRepeatIfNecessary();
    }
}