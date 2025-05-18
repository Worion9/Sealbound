using System.Collections.Generic;
using UnityEngine;

public class ShowAndPlayCardsFromDeckAbility : Ability
{
    public readonly int cardsAmount;

    public ShowAndPlayCardsFromDeckAbility(int cardsAmount)
    {
        this.cardsAmount = cardsAmount;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (playerResources.DeckList.Count == 0) return;
        
        List<Card> tempDeckList = new(playerResources.DeckList);
        List<Card> options = new();

        for (int i = 0; i < cardsAmount && tempDeckList.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, tempDeckList.Count);
            Card selectedCard = tempDeckList[randomIndex];
            options.Add(selectedCard);

            tempDeckList.RemoveAt(randomIndex);
        }

        playerResources.selectionManager.SetUpSelection(playerResources, options);
        if (card.isSpell) CardManager.KillCard(card);
    }
}