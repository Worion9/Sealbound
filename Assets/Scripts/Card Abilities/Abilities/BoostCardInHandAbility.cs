public class BoostCardInHandAbility : Ability
{
    public readonly int boost;
    public readonly bool armorInstead;

    public BoostCardInHandAbility(int boost, bool armorInstead = false)
    {
        this.boost = boost;
        this.armorInstead = armorInstead;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        playerResources.mulliganManager.BoostCardsInHand(playerResources, 1, boost, armorInstead);
    }
}