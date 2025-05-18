public class DamageCardAndBoostMeIfKilledAbility : Ability
{
    public readonly int damage;
    public readonly int boost;
    public readonly bool ifItSurvivesInstead;

    public DamageCardAndBoostMeIfKilledAbility(int damage, int boost, bool ifItSurvivesInstead = false)
    {
        this.damage = damage;
        this.boost = boost;
        this.ifItSurvivesInstead = ifItSurvivesInstead;
    }

    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        if (ifItSurvivesInstead && damage < targetedCard.Power) playerResources.boardManager.CardThatCausedSelection.Power += boost;
        else if (!ifItSurvivesInstead && damage >= targetedCard.Power) playerResources.boardManager.CardThatCausedSelection.Power += boost;

        targetedCard.Power -= damage;
    }
}