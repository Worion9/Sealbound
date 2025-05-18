using UnityEngine;

public class BoostMeIfDeckConditionsAreMetAbility : Ability
{
    private readonly int maxCardCopies;
    private readonly int boost;
    private readonly int cardsForOneBoost;

    public BoostMeIfDeckConditionsAreMetAbility(int maxCardCopies, int boost, int cardsForOneBoost)
    {
        this.maxCardCopies = maxCardCopies;
        this.boost = boost;
        this.cardsForOneBoost = cardsForOneBoost;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if(maxCardCopies < playerResources.mostCommonCardInDeckCount) return;

        int wholeBoost = Mathf.FloorToInt((float)playerResources.startingDeckSize * boost / cardsForOneBoost);

        card.power += wholeBoost;
        card.DisplayCard.UpdateCardUI();
    }
}