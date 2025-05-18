using System.Linq;
using UnityEngine;

public class SummonAllMyCopiesFromGraveyardAbility : Ability
{
    public override void Execute(PlayerResources playerResources, Card card)
    {
        // ZnajdŸ wszystkie karty odpowiadaj¹ce "card" w talii
        var matchingCards = playerResources.GraveyardList
            .Where(c => c.Id == card.Id)
            .OrderBy(_ => Random.value) // Losowe przetasowanie
            .ToList();

        if (matchingCards.Count == 0) return;

        int targetRowIndex = card.DisplayCard.transform.parent.GetComponent<DropZone>().associatedListIndex - 1;

        foreach (Card cardToSummon in matchingCards)
        {
            targetRowIndex = playerResources.boardManager.ReturnFirstValidRow(playerResources, targetRowIndex);

            playerResources.cardManager.SummonCard(playerResources, cardToSummon, false, playerResources.GraveyardList, targetRowIndex);

            playerResources.GraveyardList.Remove(cardToSummon);
            playerResources.boardManager.UpdateDeckAndGraveyardImages(playerResources);
        }
    }
}