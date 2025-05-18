public class DiscrardCardsToBoostMeAbility : Ability
{
    private readonly int boost;
    private readonly int discards;
    private readonly bool boostMeByItsPower;

    public DiscrardCardsToBoostMeAbility(int boost, int discards, bool byItsPower = false)
    {
        this.boost = boost;
        this.discards = discards;
        boostMeByItsPower = byItsPower;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        playerResources.mulliganManager.MulliganCards(playerResources, discards, true, false, card, boost, boostMeByItsPower);
    }
}