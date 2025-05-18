public class PlayAdditionalCardAbility : Ability
{
    public readonly int amount;

    public PlayAdditionalCardAbility(int amount)
    {
        this.amount = amount;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (card.wasAbilityExecuted) return;

        playerResources.CardsLeftToPlayAmmount += amount;
        if (!playerResources.isAI) playerResources.turnsLogic.uiManager.SetActionsLeftVisibility(playerResources.CardsLeftToPlayAmmount);

        card.wasAbilityExecuted = true;
    }
}