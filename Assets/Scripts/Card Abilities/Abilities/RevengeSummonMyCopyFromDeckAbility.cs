using System.Linq;
using UnityEngine;

public class RevengeSummonMyCopyFromDeckAbility : Ability
{
    public override void Execute(PlayerResources playerResources, Card card)
    {
        Card cardToSummon = playerResources.DeckList
            .Where(c => c.Id == card.Id)
            .OrderBy(_ => Random.value)
            .FirstOrDefault();

        if (cardToSummon == null) return;

        int targetRowIndex = CardManager.rowIndexOfDeadCard;
        targetRowIndex = playerResources.boardManager.ReturnFirstValidRow(playerResources, targetRowIndex);

        playerResources.cardManager.SummonCard(playerResources, cardToSummon, false, playerResources.DeckList, targetRowIndex, true, CardManager.indexOfDeadCard);
    }
}
