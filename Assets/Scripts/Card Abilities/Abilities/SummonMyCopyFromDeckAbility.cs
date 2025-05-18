using System.Linq;
using UnityEngine;

public class SummonMyCopyFromDeckAbility : Ability
{
    private readonly int cardAmount;
    private readonly bool triggerOnPlay;

    public SummonMyCopyFromDeckAbility(int cardAmount, bool triggerOnPlay = true)
    {
        this.cardAmount = cardAmount;
        this.triggerOnPlay = triggerOnPlay;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if(!triggerOnPlay && playerResources.cardManager.cardToPlay == card) return;

        // Znajd� wszystkie karty odpowiadaj�ce "card" w talii
        var matchingCards = playerResources.DeckList
            .Where(c => c.Id == card.Id)
            .OrderBy(_ => Random.value) // Losowe przetasowanie
            .Take(cardAmount)           // We� maksymalnie `cardAmount` kart
            .ToList();

        // Je�li brak takich kart w talii, zako�cz
        if (matchingCards.Count == 0) return;

        int targetRowIndex = card.DisplayCard.transform.parent.GetComponent<DropZone>().associatedListIndex - 1;

        // Przywo�aj ka�d� wybran� kart�
        foreach (Card cardToSummon in matchingCards)
        {
            targetRowIndex = playerResources.boardManager.ReturnFirstValidRow(playerResources, targetRowIndex);

            playerResources.cardManager.SummonCard(playerResources, cardToSummon, false, playerResources.DeckList, targetRowIndex);
        }
    }
}