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

        // ZnajdŸ wszystkie karty odpowiadaj¹ce "card" w talii
        var matchingCards = playerResources.DeckList
            .Where(c => c.Id == card.Id)
            .OrderBy(_ => Random.value) // Losowe przetasowanie
            .Take(cardAmount)           // WeŸ maksymalnie `cardAmount` kart
            .ToList();

        // Jeœli brak takich kart w talii, zakoñcz
        if (matchingCards.Count == 0) return;

        int targetRowIndex = card.DisplayCard.transform.parent.GetComponent<DropZone>().associatedListIndex - 1;

        // Przywo³aj ka¿d¹ wybran¹ kartê
        foreach (Card cardToSummon in matchingCards)
        {
            targetRowIndex = playerResources.boardManager.ReturnFirstValidRow(playerResources, targetRowIndex);

            playerResources.cardManager.SummonCard(playerResources, cardToSummon, false, playerResources.DeckList, targetRowIndex);
        }
    }
}