public class StrengthenMeAbility : Ability
{
    private readonly int boost;

    public StrengthenMeAbility(int boost)
    {
        this.boost = boost;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        card.BasePower += boost;
        card.power += boost;
        card.DisplayCard.UpdateCardUI();
    }
}
