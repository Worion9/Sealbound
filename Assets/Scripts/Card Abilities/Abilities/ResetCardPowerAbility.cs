public class ResetCardPowerAbility : Ability
{
    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        targetedCard.power = targetedCard.BasePower;
        targetedCard.DisplayCard.UpdateCardUI();
    }
}