public class AutoPlayMeOnMillganOrDiscardAbility : Ability
{
    public override void Execute(PlayerResources playerResources, Card card)
    {
        playerResources.cardManager.SummonCard(playerResources, card, false);
    }
}