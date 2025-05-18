public class MulliganCardsAbility : Ability
{
    private readonly int mulligans;
    private readonly bool discardUnwantedCards;

    public MulliganCardsAbility(int mulligans, bool discardUnwantedCards)
    {
        this.mulligans = mulligans;
        this.discardUnwantedCards = discardUnwantedCards;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        int mulligans = this.mulligans;

        if (playerResources.DeckList.Count < 1) return;
        if (playerResources.DeckList.Count < mulligans) mulligans = playerResources.DeckList.Count;
        
        playerResources.maxHandSize += mulligans;

        for (int i = 0; i < mulligans; i++)
        {
            playerResources.cardManager.DrawCard(playerResources, true);
        }

        playerResources.maxHandSize -= mulligans;

        playerResources.cardManager.StartForcingPlayer(playerResources);
        playerResources.mulliganManager.MulliganCards(playerResources, mulligans, discardUnwantedCards, false);
    }
}