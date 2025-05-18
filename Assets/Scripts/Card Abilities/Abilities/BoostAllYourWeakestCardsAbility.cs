using System.Collections.Generic;
using System.Linq;

public class BoostAllYourWeakestCardsAbility : Ability
{
    public readonly int boost;
    public readonly int repetitions;

    public BoostAllYourWeakestCardsAbility(int boost, int repetitions)
    {
        this.boost = boost;
        this.repetitions = repetitions;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        List<Card> allCards = new();
        allCards.AddRange(playerResources.Row1List);
        allCards.AddRange(playerResources.Row2List);
        allCards.AddRange(playerResources.Row3List);

        for (int i = 0; i < repetitions; i++)
        {
            int minPower = allCards.Min(card => card.Power);

            List<Card> weakestCards = allCards.Where(card => card.Power == minPower).ToList();

            foreach (Card c in weakestCards)
            {
                c.Power += boost;
            }
        }
    }
}