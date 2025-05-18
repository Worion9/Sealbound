public class RevengeResMeAbility : Ability
{
    public readonly bool reduceStrenghtByHalf;
    public readonly bool removeAbility;

    public RevengeResMeAbility(bool reduceStrenghtByHalf, bool removeAbility)
    {
        this.reduceStrenghtByHalf = reduceStrenghtByHalf;
        this.removeAbility = removeAbility;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (card.wasAbilityExecuted) return;
        
        if (reduceStrenghtByHalf)
        {
            card.BasePower /= 2;
            card.power = card.BasePower;
        }

        playerResources.cardManager.SummonCard(playerResources, card, false, playerResources.GraveyardList, CardManager.killedCardRowIndex, true, CardManager.indexOfDeadCard);
        playerResources.graveyardImage.SetActive(playerResources.GraveyardList.Count > 0);

        if (removeAbility)
        {
            card.wasAbilityExecuted = true;
            card.Description = "<i>none</i>";
        }
    }
}