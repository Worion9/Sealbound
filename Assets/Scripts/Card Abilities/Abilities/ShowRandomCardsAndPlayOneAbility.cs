using System.Collections.Generic;
using UnityEngine;

public class ShowRandomCardsAndPlayOneAbility : Ability
{
    private readonly int amount;

    public ShowRandomCardsAndPlayOneAbility(int amount)
    {
        this.amount = amount;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        List<Card> cardsToShow = new();
        HashSet<int> usedIndices = new();

        for (int i = 0; i < amount; i++)
        {
            int randomIndex;

            do
            {
                randomIndex = Random.Range(1, CardDatabase.cardList.Count);
            }
            while (usedIndices.Contains(randomIndex));

            usedIndices.Add(randomIndex);

            Card newCard = CardDatabase.cardList[randomIndex].Clone();

            if (newCard.CardRarity != Card.Rarity.None)
            {
                cardsToShow.Add(newCard);
            }
            else
            {
                i--;
            }
        }

        playerResources.selectionManager.SetUpSelection(playerResources, cardsToShow, true);
    }
}