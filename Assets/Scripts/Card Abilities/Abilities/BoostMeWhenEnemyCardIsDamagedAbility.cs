public class BoostMeWhenEnemyCardIsDamagedAbility : Ability
{
    public readonly int boost;

    public BoostMeWhenEnemyCardIsDamagedAbility(int boost)
    {
        this.boost = boost;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        card.Power += boost;
    }
}