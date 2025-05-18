public class KillCardAbility : Ability
{
    private readonly bool onlyNotBosstedCards;
    private readonly bool onlyDamagedCards;

    public KillCardAbility(bool onlyNotBosstedCards = false, bool onlyDamagedCards = false)
    {
        this.onlyNotBosstedCards = onlyNotBosstedCards;
        this.onlyDamagedCards = onlyDamagedCards;
    }

    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        if (onlyNotBosstedCards && targetedCard.Power > targetedCard.BasePower) return;

        if (onlyDamagedCards && targetedCard.Power >= targetedCard.BasePower) return;
        
        CardManager.KillCard(targetedCard);
    }
}