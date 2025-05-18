public class ChangeBasePowerAbility : Ability
{
    private readonly int boost;
    private readonly bool usePowerInstead;

    public ChangeBasePowerAbility(int boost, bool usePowerInstead = false)
    {
        this.boost = boost;
        this.usePowerInstead = usePowerInstead;
    }

    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        if (usePowerInstead)
        {
            targetedCard.BasePower = targetedCard.Power;
        }
        else
        {
            targetedCard.BasePower += boost;
            targetedCard.power += boost;
            targetedCard.DisplayCard.UpdateCardUI();
        }
    }
}