using System.Collections.Generic;

public class StartBoardSelectionAbility : Ability
{
    private readonly bool canSelectYourCards;
    private readonly bool canSelectEnemyCards;
    private readonly int charges;
    public readonly bool canSelectTheSameCards;
    public readonly bool canSelectCardWithSameIdAsThisCard;
    public static HashSet<Card> selectedCards;

    public StartBoardSelectionAbility(bool canSelectYourCards, bool canSelectEnemyCards, int charges = 1, bool canSelectTheSameCards = true, bool canSelectCardWithSameIdAsThisCard = true)
    {
        this.canSelectYourCards = canSelectYourCards;
        this.canSelectEnemyCards = canSelectEnemyCards;
        this.charges = charges;
        this.canSelectTheSameCards = canSelectTheSameCards;
        this.canSelectCardWithSameIdAsThisCard = canSelectCardWithSameIdAsThisCard;
        selectedCards = new();
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (!card.wasAbilityExecuted)
        {
            card.Charges = charges;
            card.wasAbilityExecuted = true;
        }

        playerResources.boardManager.canSelectYourCards = canSelectYourCards;
        playerResources.boardManager.canSelectEnemyCards = canSelectEnemyCards;
        playerResources.boardManager.canSelectTheSameCards = canSelectTheSameCards;
        playerResources.boardManager.SetUpFromBardSelection(playerResources, card, canSelectCardWithSameIdAsThisCard);
    }
}
