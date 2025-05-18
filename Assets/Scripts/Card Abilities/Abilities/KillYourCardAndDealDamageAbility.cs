public class KillYourCardAndDealDamageAbility : Ability
{
    int damage;
    
    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        if (playerResources.boardManager.CardThatCausedSelection.Charges > 0)
        {
            damage = targetedCard.Power;
            CardManager.KillCard(targetedCard);

            playerResources.boardManager.canSelectYourCards = true;
            playerResources.boardManager.canSelectEnemyCards = true;
            return;
        }

        targetedCard.Power -= damage;
    }
}