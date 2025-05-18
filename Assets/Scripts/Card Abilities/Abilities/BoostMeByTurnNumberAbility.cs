public class BoostMeByTurnNumberAbility : Ability
{
    private readonly int amount;

    public BoostMeByTurnNumberAbility(int amount)
    {
        this.amount = amount;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        card.Power += amount * TurnsLogic.turnNumber;
    }
}