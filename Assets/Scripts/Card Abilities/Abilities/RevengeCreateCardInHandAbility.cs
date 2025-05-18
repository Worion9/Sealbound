public class RevengeCreateCardInHandAbility : Ability
{
    public override void Execute(PlayerResources playerResources, Card card)
    {
        Card cardToCreate = CardDatabase.cardList[card.Id + 1].Clone();

        playerResources.DeckList.Insert(0, cardToCreate);
        playerResources.cardManager.DrawCard(playerResources, false, 0, false);
    }
}
