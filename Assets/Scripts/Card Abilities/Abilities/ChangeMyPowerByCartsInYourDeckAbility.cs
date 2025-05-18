using UnityEngine;

public class ChangeMyPowerByCartsInYourDeckAbility : Ability
{
    private readonly int power;
    private readonly int cardsNumber;

    public ChangeMyPowerByCartsInYourDeckAbility(int power, int cardsNumber)
    {
        this.power = power;
        this.cardsNumber = cardsNumber;
    }
    
    public override void Execute(PlayerResources playerResources, Card card)
    {
        int totalPowerChange = power * Mathf.RoundToInt(playerResources.DeckList.Count / (float)cardsNumber + 0.0001f);
        card.Power += totalPowerChange;
    }
}