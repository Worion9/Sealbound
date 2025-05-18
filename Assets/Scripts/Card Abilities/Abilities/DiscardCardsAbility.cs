public class DiscardCardsAbility : Ability
{
    public readonly int amount;

    public DiscardCardsAbility(int amount)
    {
        this.amount = amount;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (playerResources.isAI) return;
        if (playerResources.HandList.Count < 1) return;

        playerResources.cardManager.StartForcingPlayer(playerResources);
        playerResources.mulliganManager.MulliganCards(playerResources, amount, true, false);
    }
}