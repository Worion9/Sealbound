public class CooldownGainChargeAbility : Ability
{
    public readonly int charges;
    public readonly int renewal;

    public CooldownGainChargeAbility(int charges, int renewal)
    {
        this.charges = charges;
        this.renewal = renewal;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (renewal > 1)
        {
            card.Counter--;

            if (card.Counter <= 0)
            {
                card.Counter = renewal;
            }
            else return;
        }
        card.Charges += charges;
    }
}