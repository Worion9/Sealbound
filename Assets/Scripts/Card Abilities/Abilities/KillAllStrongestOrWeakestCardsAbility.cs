using System.Collections.Generic;
using System.Linq;

public class KillAllStrongestOrWeakestCardsAbility : Ability
{
    private readonly bool strongest;
    private readonly bool includeThisCards;

    public KillAllStrongestOrWeakestCardsAbility(bool strongest, bool includeThisCards)
    {
        this.strongest = strongest;
        this.includeThisCards = includeThisCards;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        List<Card> allCards = new();
        allCards.AddRange(playerResources.Row1List);
        allCards.AddRange(playerResources.Row2List);
        allCards.AddRange(playerResources.Row3List);
        allCards.AddRange(playerResources.myEnemy.Row1List);
        allCards.AddRange(playerResources.myEnemy.Row2List);
        allCards.AddRange(playerResources.myEnemy.Row3List);

        if (!includeThisCards)
        {
            allCards = allCards.Where(c => c != card).ToList();
        }

        if (allCards.Count == 0) return;

        int targetedPower = strongest ? allCards.Max(c => c.Power) : allCards.Min(c => c.Power);

        List<Card> targetedCards = allCards.Where(c => c.Power == targetedPower).ToList();

        foreach (Card c in targetedCards)
        {
            CardManager.KillCard(c);
        }
    }
}