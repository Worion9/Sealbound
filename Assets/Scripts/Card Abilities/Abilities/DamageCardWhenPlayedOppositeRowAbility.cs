public class DamageCardWhenPlayedOppositeRowAbility : Ability
{
    public readonly int damage;

    public DamageCardWhenPlayedOppositeRowAbility(int damage)
    {
        this.damage = damage;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        playerResources.cardManager.cardToPlay.Power -= damage;
    }
}