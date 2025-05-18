using System.Collections.Generic;

public class DestroyBoardAbility : Ability
{
    public readonly bool killOnlyNotBoostedCards;
    public readonly bool killOnlyWoondedCards;

    public DestroyBoardAbility(bool killOnlyBoostedCards = false, bool killOnlyWoondedCards = false)
    {
        this.killOnlyNotBoostedCards = killOnlyBoostedCards;
        this.killOnlyWoondedCards = killOnlyWoondedCards;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        List<Card> cardsToDestroy = new();

        cardsToDestroy.AddRange(playerResources.Row1List);
        cardsToDestroy.AddRange(playerResources.Row2List);
        cardsToDestroy.AddRange(playerResources.Row3List);
        cardsToDestroy.AddRange(playerResources.myEnemy.Row1List);
        cardsToDestroy.AddRange(playerResources.myEnemy.Row2List);
        cardsToDestroy.AddRange(playerResources.myEnemy.Row3List);

        foreach (Card c in cardsToDestroy)
        {
            if (c == card) continue;
            if (killOnlyNotBoostedCards && c.Power > c.BasePower) continue;
            if (killOnlyWoondedCards && c.Power >= c.BasePower) continue;
            CardManager.KillCard(c);
        }
    }
}