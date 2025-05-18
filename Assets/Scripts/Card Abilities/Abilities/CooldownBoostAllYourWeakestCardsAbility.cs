using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CooldownBoostAllYourWeakestCardsAbility : Ability
{
    public readonly int boost;
    public readonly int renewal;
    public readonly bool onlyOneCard;

    public CooldownBoostAllYourWeakestCardsAbility(int boost, int renewal, bool onlyOneCard = false)
    {
        this.boost = boost;
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
        allCards.AddRange(playerResources.Row1List);
        allCards.AddRange(playerResources.Row2List);
        allCards.AddRange(playerResources.Row3List);

        int minPower = allCards.Min(card => card.Power);

        List<Card> weakestCards = allCards.Where(card => card.Power == minPower).ToList();

        if (onlyOneCard)
        {
            int randomIndex = Random.Range(0, weakestCards.Count);
            Card selectedCard = weakestCards[randomIndex];
            selectedCard.Power += boost;
        }
        else
        {
            foreach (Card selectedCard in weakestCards)
            {
                selectedCard.Power += boost;
            }
        }
    }
}