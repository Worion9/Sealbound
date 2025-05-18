public class BoostMeByTurnNumberAndDagameMeByEnemyCardsOnBoardAbility : Ability
{
    public readonly int boost;
    public readonly int damage;

    public BoostMeByTurnNumberAndDagameMeByEnemyCardsOnBoardAbility(int boost, int damage)
    {
        this.boost = boost;
        this.damage = damage;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        card.Power += boost * TurnsLogic.turnNumber;
        int enemyCards = playerResources.myEnemy.Row1List.Count + playerResources.myEnemy.Row2List.Count + playerResources.myEnemy.Row3List.Count;
        card.Power -= damage * enemyCards;
    }
}