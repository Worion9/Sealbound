public class SelectCardFromDecksAbility : Ability
{
    private readonly int amount;
    private readonly bool toDiscard;

    public SelectCardFromDecksAbility(int amount, bool toDiscard)
    {
        this.amount = amount;
        this.toDiscard = toDiscard;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (playerResources.DeckList.Count <= 0) return;
        playerResources.deckManager.SelectCardsFromDeck(playerResources, amount, toDiscard);
    }
}