public class CooldownBoostMeOrDealDamageToMeAbility : Ability
{
    public readonly int boostOrDamage;
    public readonly int turnsToStartDealingDamage;

    public CooldownBoostMeOrDealDamageToMeAbility(int boostOrDamage, int turnsToStartDealingDamage)
    {
        this.boostOrDamage = boostOrDamage;
        this.turnsToStartDealingDamage = turnsToStartDealingDamage;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (card.ParentRowList == playerResources.GraveyardList || card.ParentRowList == playerResources.myEnemy.GraveyardList) return;

        if (!card.wasAbilityExecuted)
        {
            card.Counter = turnsToStartDealingDamage;
            card.wasAbilityExecuted = true;
        }

        if (card.Counter > 0)
        {
            card.Power += boostOrDamage;
        }
        else
        {
            card.Power -= boostOrDamage;
        }

        card.Counter--;
    }
}