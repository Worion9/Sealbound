public class DealDamageToCardWhenPlayedAbility : Ability
{
    public readonly int damage;

    public DealDamageToCardWhenPlayedAbility(int damage)
    {
        this.damage = damage;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {

        if (playerResources.cardManager.cardToPlay != card)
        {
            playerResources.cardManager.cardToPlay.Power -= damage;
        }
    }
}