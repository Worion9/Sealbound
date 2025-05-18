public class BoostMeByTurnNumberReductedByCardsInRowAbility : Ability
{
    public readonly int boost;
    public readonly int damage;

    public BoostMeByTurnNumberReductedByCardsInRowAbility(int boost, int damage)
    {
        this.boost = boost;
        this.damage = damage;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        card.Power += boost * TurnsLogic.turnNumber;
        int cardInRow = card.ParentRowList.Count -1;
        card.Power -= damage * cardInRow;
    }
}