public class WhenYourCardMovesBoostItAbility : Ability
{
    private readonly int boost;

    public WhenYourCardMovesBoostItAbility(int boost)
    {
        this.boost = boost;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (playerResources.cardManager.cardToPlay == card) return;
        playerResources.cardManager.cardToPlay.Power += boost;
    }
}