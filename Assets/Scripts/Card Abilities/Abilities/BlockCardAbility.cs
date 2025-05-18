public class BlockCardAbility : Ability
{
    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        targetedCard.BlockAllAbilities();
    }
}