public class RevengeSpawnCardAbility : Ability
{
    public override void Execute(PlayerResources playerResources, Card card)
    {
        Card cardToSummon = CardDatabase.cardList[card.Id + 1].Clone();

        int targetRowIndex = CardManager.rowIndexOfDeadCard;

        targetRowIndex = playerResources.boardManager.ReturnFirstValidRow(playerResources, targetRowIndex);
        playerResources.cardManager.SummonCard(playerResources, cardToSummon, false, null, targetRowIndex, true);
    }
}