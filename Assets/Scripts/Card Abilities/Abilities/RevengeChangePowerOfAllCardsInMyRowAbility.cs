public class RevengeChangePowerOfAllCardsInMyRowAbility : Ability
{
    public readonly int power;

    public RevengeChangePowerOfAllCardsInMyRowAbility(int power)
    {
        this.power = power;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        var cardsToChanePower = CardManager.rowListOfDeadCard;

        foreach (Card c in cardsToChanePower)
        {
            if (c == card) continue;
            c.Power += power;
        }
    }
}