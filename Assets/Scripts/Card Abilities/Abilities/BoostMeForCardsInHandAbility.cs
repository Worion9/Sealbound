public class BoostMeForCardsInHandAbility : Ability
{
    public readonly int baseCards;
    public readonly int boost;

    public BoostMeForCardsInHandAbility(int baseCards, int boost)
    {
        this.baseCards = baseCards;
        this.boost = boost;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        int excessCards = playerResources.HandList.Count - baseCards;
        if (excessCards > 0) card.Power += excessCards * boost;
    }
}