public class SwapTwoCardPowerAbility : Ability
{
    private Card firstCard;
    private readonly bool moveBoostOnly;

    public SwapTwoCardPowerAbility(bool moveBoostOnly = false)
    {
        this.moveBoostOnly = moveBoostOnly;
    }
    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        if (playerResources.boardManager.CardThatCausedSelection.Charges > 0)
        {
            firstCard = targetedCard;
            return;
        }

        if(!moveBoostOnly) (targetedCard.Power, firstCard.Power) = (firstCard.Power, targetedCard.Power); // Ale fajne
        else
        {
            int boost = firstCard.Power - firstCard.BasePower;
            if (boost > 0) 
            {
                firstCard.Power = firstCard.BasePower;
                targetedCard.Power += boost;
            }
        }
    }
}