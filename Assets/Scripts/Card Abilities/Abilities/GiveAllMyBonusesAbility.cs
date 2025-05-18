public class GiveAllMyBonusesAbility : Ability
{
    public readonly int basePowerChange;

    public GiveAllMyBonusesAbility(int basePowerChange)
    {
        this.basePowerChange = basePowerChange;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (playerResources.cardManager.cardToPlay == card) return;
        if (playerResources != card.DisplayCard.playerResources) return;

        Card cardToBoost = playerResources.cardManager.cardToPlay;

        if (card.DisplayCard.CardInFieldIndex == playerResources.cardManager.position - 1)
        {
            if (card.Armor > 0)
            {
                cardToBoost.Armor += card.Armor;
                card.Armor = 0;
                card.wasAbilityExecuted = true;
            }

            if (card.Power > card.BasePower)
            {
                cardToBoost.Power += card.Power - card.BasePower;
                card.Power = card.BasePower;
                card.wasAbilityExecuted = true;
            }

            if (card.wasAbilityExecuted)
            {
                card.BasePower -= basePowerChange;
                card.power -= basePowerChange;
                card.DisplayCard.UpdateCardUI();
                card.wasAbilityExecuted = false;
            }
        }
    }
}