public class CooldownBoostMeAbility : Ability
{
    public readonly int boost;
    public readonly int renewal;

    public CooldownBoostMeAbility(int boost, int renewal)
    {
        this.boost = boost;
        this.renewal = renewal;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (renewal == 1)
        {
            card.Power += boost;
        }
        else if (renewal > 1)
        {
            card.Counter--;

            if (card.Counter <= 0)
            {
                card.Power += boost;
                card.Counter = renewal;
            }
        }
    }
}