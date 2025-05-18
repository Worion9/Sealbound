public class WhenEnemyCardMovesDamageItAbility : Ability
{
    private readonly int damage;

    public WhenEnemyCardMovesDamageItAbility(int damage)
    {
        this.damage = damage;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (playerResources.cardManager.cardToPlay == card) return;
        playerResources.cardManager.cardToPlay.Power -= damage;
    }
}