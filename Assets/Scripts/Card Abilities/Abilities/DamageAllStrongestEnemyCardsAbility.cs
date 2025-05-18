using System.Collections.Generic;
using System.Linq;

public class DamageAllStrongestEnemyCardsAbility : Ability
{
    public readonly int damage;
    public readonly int repetitions;

    public DamageAllStrongestEnemyCardsAbility(int damage, int repetitions)
    {
        this.damage = damage;
        this.repetitions = repetitions;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        List<Card> allCards = new();
        allCards.AddRange(playerResources.myEnemy.Row1List);
        allCards.AddRange(playerResources.myEnemy.Row2List);
        allCards.AddRange(playerResources.myEnemy.Row3List);

        for (int i = 0; i < repetitions; i++)
        {
            int maxPower = allCards.Max(card => card.Power);

            List<Card> strongestCards = allCards.Where(card => card.Power == maxPower).ToList();

            foreach (Card c in strongestCards)
            {
                c.Power -= damage;
            }
        }
    }
}