public class SetCardPowerAbility : Ability
{
    public readonly int power;

    public SetCardPowerAbility(int power)
    {
        this.power = power;
    }

    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        targetedCard.Power = power;
    }
}