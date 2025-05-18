public class BoostMeForEveryBoostedCardOnYourSideAbility : Ability
{
    public readonly int boost;

    public BoostMeForEveryBoostedCardOnYourSideAbility(int boost)
    {
        this.boost = boost;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        int boostedCardsCount = 0;

        foreach (var row in new[] { playerResources.Row1List, playerResources.Row2List, playerResources.Row3List })
        {
            foreach (var c in row)
            {
                if (c.Power > c.BasePower)
                {
                    if (c == card) continue;
                    boostedCardsCount++;
                }
            }
        }

        card.Power += boostedCardsCount * boost;
    }
}