public class BoostMeForEveryWoundedCardOnEnemySideAbility : Ability
{
    public readonly int boost;

    public BoostMeForEveryWoundedCardOnEnemySideAbility(int boost)
    {
        this.boost = boost;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        int boostedCardsCount = 0;

        foreach (var row in new[] { playerResources.myEnemy.Row1List, playerResources.myEnemy.Row2List, playerResources.myEnemy.Row3List })
        {
            foreach (var c in row)
            {
                if (c.Power < c.BasePower)
                {
                    if (c == card) continue;
                    boostedCardsCount++;
                }
            }
        }

        card.Power += boostedCardsCount * boost;
    }
}