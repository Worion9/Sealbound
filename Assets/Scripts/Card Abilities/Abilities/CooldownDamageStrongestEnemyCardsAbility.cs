using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CooldownDamageStrongestEnemyCardsAbility : Ability
{
    public readonly int damage;
    public readonly int renewal;
    public readonly bool onlyOneCard;

    public CooldownDamageStrongestEnemyCardsAbility(int damage, int renewal, bool onlyOneCard = false)
    {
        this.damage = damage;
        this.renewal = renewal;
        this.onlyOneCard = onlyOneCard;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (renewal > 1)
        {
            card.Counter--;

            if (card.Counter <= 0)
            {
                card.Counter = renewal;
            }
            else return;
        }

        List<Card> allCards = new();
        allCards.AddRange(playerResources.myEnemy.Row1List);
        allCards.AddRange(playerResources.myEnemy.Row2List);
        allCards.AddRange(playerResources.myEnemy.Row3List);

        int maxPower = allCards.Max(card => card.Power);

        List<Card> strongestCards = allCards.Where(card => card.Power == maxPower).ToList();

        if (strongestCards.Count > 0)
        {
            if (onlyOneCard)
            {
                int randomIndex = Random.Range(0, strongestCards.Count);
                Card selectedCard = strongestCards[randomIndex];
                selectedCard.Power -= damage;
            }
            else
            {
                foreach (Card selectedCard in strongestCards)
                {
                    selectedCard.Power -= damage;
                }
            }
        }
    }
}