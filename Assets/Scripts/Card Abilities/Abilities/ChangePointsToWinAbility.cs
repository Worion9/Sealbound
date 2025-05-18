public class ChangePointsToWinAbility : Ability
{
    private readonly int pointsChange;

    public ChangePointsToWinAbility(int pointsChange)
    {
        this.pointsChange = pointsChange;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        playerResources.turnsLogic.ChangeBaseHP(pointsChange);
        playerResources.boardManager.RefreshMainPointCounters(playerResources);
    }
}