public class BoostMeWhenEnemyCardDiesAbility : Ability
{
    public readonly int boost;

    public BoostMeWhenEnemyCardDiesAbility(int boost)
    {
        this.boost = boost;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        card.Power += boost;
    }
}