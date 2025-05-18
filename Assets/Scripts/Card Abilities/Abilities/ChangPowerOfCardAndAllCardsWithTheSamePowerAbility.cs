using System.Linq;

public class ChangPowerOfCardAndAllCardsWithTheSamePowerAbility : Ability
{
    public readonly int power;

    public ChangPowerOfCardAndAllCardsWithTheSamePowerAbility(int power)
    {
        this.power = power;
    }

    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        int targetedCardPower = targetedCard.Power;

        var allRows = targetedCard.DisplayCard.playerResources.Row1List.Concat(targetedCard.DisplayCard.playerResources.Row2List).Concat(targetedCard.DisplayCard.playerResources.Row3List).ToList();

        foreach (Card card in allRows)
        {
            if (card.Power == targetedCardPower)
            {
                card.Power += power;
            }
        }
    }
}