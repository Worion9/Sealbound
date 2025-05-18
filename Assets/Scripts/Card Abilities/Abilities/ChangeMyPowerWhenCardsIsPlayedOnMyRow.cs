public class ChangeMyPowerWhenCardsIsPlayedOnMyRow : Ability
{
    public readonly int power;

    public ChangeMyPowerWhenCardsIsPlayedOnMyRow(int power)
    {
        this.power = power;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if(card == playerResources.cardManager.cardToPlay) return;

        card.Power += power;
    }
}