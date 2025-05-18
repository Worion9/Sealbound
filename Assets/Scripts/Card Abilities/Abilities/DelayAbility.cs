public class DelayAbility : Ability
{
    public readonly int delay;

    public DelayAbility(int delay)
    {
        this.delay = delay;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (card.wasAbilityExecuted) return;

        if (card.Counter <= 0)
        {
            card.Counter = delay;
        }
        else
        {
            card.Counter--;

            if (card.Counter <= 0)
            {
                card.wasAbilityExecuted = true;
                card.TriggerFirstAbility(playerResources, card);
            }
        }
    }
}