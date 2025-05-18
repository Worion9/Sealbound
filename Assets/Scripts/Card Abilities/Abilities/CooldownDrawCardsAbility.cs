public class CooldownDrawCardsAbility : Ability
{
    public readonly int renewal;

    public CooldownDrawCardsAbility(int renewal)
    {
        this.renewal = renewal;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        card.Counter--;
        if (card.Counter <= 0)
        {
            playerResources.cardManager.DrawCard(playerResources, true);
            card.Counter = renewal;
        }
    }
}