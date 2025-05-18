public class BoostMeIfTurnNumberEqualsAbility : Ability
{
    public readonly int turnNumber;
    public readonly int boost;

    public BoostMeIfTurnNumberEqualsAbility(int turnNumber, int boost)
    {
        this.turnNumber = turnNumber;
        this.boost = boost;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (TurnsLogic.turnNumber >= turnNumber)
        {
            card.Counter--;
            card.Power += boost;
        }
    }
}