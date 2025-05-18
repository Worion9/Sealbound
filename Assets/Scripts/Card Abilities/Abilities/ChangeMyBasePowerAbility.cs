public class ChangeMyBasePowerAbility : Ability
{
    private readonly int reduction;

    public ChangeMyBasePowerAbility(int reduction)
    {
        this.reduction = reduction;
    }

    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        int boost = targetedCard.BasePower - reduction;
        playerResources.boardManager.CardThatCausedSelection.Power += boost;
    }
}