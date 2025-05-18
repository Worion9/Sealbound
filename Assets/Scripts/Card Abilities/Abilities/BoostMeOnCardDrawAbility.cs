public class BoostMeOnCardDrawAbility : Ability
{
    public readonly int boost;

    public BoostMeOnCardDrawAbility(int boost)
    {
        this.boost = boost;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        card.Power += boost;
    }
}