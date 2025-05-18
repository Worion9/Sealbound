using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConsumeCardsAbility : Ability
{
    public readonly bool wholeRow;

    public ConsumeCardsAbility(bool wholeRow)
    {
        this.wholeRow = wholeRow;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        List<Card> cardsToConsume = new();
        List<Card> currentRow = card.ParentRowList;

        if (currentRow != null)
        {
            if (GameObject.FindFirstObjectByType<TutorialLogic>().grubHaveToEatEgg == true)
            {
                var allCards = new List<List<Card>> { playerResources.Row1List, playerResources.Row2List, playerResources.Row3List };

                cardsToConsume.Add(allCards.SelectMany(row => row).FirstOrDefault(c => c.Id == 103));
            }

            else if (wholeRow)
            {
                foreach (Card c in currentRow)
                {
                    if (c != card) cardsToConsume.Add(c);
                }
            }

            else
            {
                int cardIndex = currentRow.IndexOf(card);

                if (cardIndex > 0)
                {
                    cardsToConsume.Add(currentRow[cardIndex - 1]);
                }

                if (cardIndex < currentRow.Count - 1)
                {
                    cardsToConsume.Add(currentRow[cardIndex + 1]);
                }
            }
        }

        foreach (var c in cardsToConsume)
        {
            if (c.isInvulnerable) continue;
            card.Power += c.Power;
            CardManager.KillCard(c);
        }
    }
}