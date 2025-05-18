using System.Linq;

public class SetAllCardsInMyRowPowerAbility : Ability
{
    private readonly int power;

    public SetAllCardsInMyRowPowerAbility(int power = 0)
    {
        this.power = power;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        var CardList = card.ParentRowList.ToList();

        foreach (var c in CardList)
        {
            if(c == card) continue;

            if (power == 0) CardManager.KillCard(c);
            else c.Power = power;
        }
    }
}