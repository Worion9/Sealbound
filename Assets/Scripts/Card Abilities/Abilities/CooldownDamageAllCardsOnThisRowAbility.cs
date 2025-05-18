using System.Collections.Generic;
using UnityEngine;

public class CooldownDamageAllCardsOnThisRowAbility : Ability
{
    public readonly int damage;
    public readonly int renewal;
    public readonly bool enemyRow;

    public CooldownDamageAllCardsOnThisRowAbility(int damage, int renewal, bool enemyRow = false)
    {
        this.damage = damage;
        this.renewal = renewal;
        this.enemyRow = enemyRow;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (card.IsSpy && playerResources.cardManager.cardToPlay == card) return;

        if (renewal > 1)
        {
            card.Counter--;

            if (card.Counter <= 0)
            {
                card.Counter = renewal;
            }

            else return;
        }

        List<Card> targetRow;
        if (!enemyRow)
        {
            targetRow = playerResources.Row1List.Contains(card) ? playerResources.Row1List
                           : playerResources.Row2List.Contains(card) ? playerResources.Row2List
                           : playerResources.Row3List.Contains(card) ? playerResources.Row3List
                           : null;
        }
        else
        {
            targetRow = playerResources.Row1List.Contains(card) ? playerResources.myEnemy.Row1List
                           : playerResources.Row2List.Contains(card) ? playerResources.myEnemy.Row2List
                           : playerResources.Row3List.Contains(card) ? playerResources.myEnemy.Row3List
                           : null;
        }

        if (targetRow != null)
        {
            List<Card> targetRowCopy = new(targetRow);

            foreach (Card c in targetRowCopy)
            {
                c.Power -= damage;
            }
        }
        else
        {
            Debug.LogWarning("Card not found in any row.");
        }
    }
}