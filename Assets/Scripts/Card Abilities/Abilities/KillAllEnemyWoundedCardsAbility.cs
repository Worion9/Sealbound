using System.Collections.Generic;

public class KillAllEnemyWoundedCardsAbility : Ability
{
    public override void Execute(PlayerResources playerResources, Card card)
    {
        List<Card> allCards = new();
        allCards.AddRange(playerResources.myEnemy.Row1List);
        allCards.AddRange(playerResources.myEnemy.Row2List);
        allCards.AddRange(playerResources.myEnemy.Row3List);

        foreach (Card c in allCards)
        {
            if(c == card) continue;
            if(c.Power >= c.BasePower) continue;
            CardManager.KillCard(c);
        }
    }
}