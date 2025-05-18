using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RevengeBoostTheWeakestCardOnEnemySideAbility : Ability
{
    public readonly int cardId;
    public readonly int boost;

    public RevengeBoostTheWeakestCardOnEnemySideAbility(int cardId, int boost)
    {
        this.cardId = cardId;
        this.boost = boost;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        List<Card> allCards = new();
        allCards.AddRange(playerResources.myEnemy.Row1List);
        allCards.AddRange(playerResources.myEnemy.Row2List);
        allCards.AddRange(playerResources.myEnemy.Row3List);

        List<Card> filteredCards = allCards.Where(c => c.Id == cardId).ToList();

        if (filteredCards.Count > 0)
        {
            int minPower = filteredCards.Min(c => c.Power);

            List<Card> weakestCards = filteredCards.Where(c => c.Power == minPower).ToList();

            if (weakestCards.Count > 0)
            {
                int randomIndex = Random.Range(0, weakestCards.Count);
                Card selectedCard = weakestCards[randomIndex];

                selectedCard.Power += boost;
            }
        }
    }
}