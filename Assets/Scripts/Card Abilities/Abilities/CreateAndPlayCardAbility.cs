public class CreateAndPlayCardAbility : Ability
{
    public override void Execute(PlayerResources playerResources, Card card)
    {
        Card cardToCreate = CardDatabase.cardList[card.Id].Clone();

        playerResources.DeckList.Insert(0, cardToCreate);
        playerResources.cardManager.ForceCardToBePlayed(playerResources, 0, false, false);
    }
}
