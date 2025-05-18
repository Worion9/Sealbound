using System.Collections;

public class WhenOtherCardDieSpawnCardAbility : Ability
{
    public override void Execute(PlayerResources playerResources, Card card)
    {
        playerResources.StartCoroutine(DelayedExecution(playerResources, card));
    }

    private IEnumerator DelayedExecution(PlayerResources playerResources, Card card)
    {
        yield return null;

        if (card.ParentRowList == null ||
            card.ParentRowList == playerResources.GraveyardList ||
            card.ParentRowList == playerResources.myEnemy.GraveyardList) yield break;

        Card cardToSummon = CardDatabase.cardList[card.Id + 1].Clone();
        int targetRowIndex = CardManager.rowIndexOfDeadCard;

        targetRowIndex = playerResources.boardManager.ReturnFirstValidRow(playerResources, targetRowIndex);
        playerResources.cardManager.SummonCard(playerResources, cardToSummon, false, null, targetRowIndex, true);
    }
}