using System.Collections.Generic;

public class ChangePowerOfAllCardsOnBoardAbility : Ability
{
    public readonly int power;
    public readonly bool triggerOnYourCards;
    public readonly bool triggerOnEnemyCards;
    public readonly bool onlyWoundedCards;
    public readonly bool onlyBoostedCards;

    public ChangePowerOfAllCardsOnBoardAbility(int power, bool triggerOnYourCards, bool triggerOnEnemyCards, bool onlyWoundedCards = false, bool onlyBoostedCards = false)
    {
        this.power = power;
        this.triggerOnYourCards = triggerOnYourCards;
        this.triggerOnEnemyCards = triggerOnEnemyCards;
        this.onlyWoundedCards = onlyWoundedCards;
        this.onlyBoostedCards = onlyBoostedCards;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        List<List<Card>> targetRows = new();

        if (triggerOnYourCards)
        {
            targetRows.Add(playerResources.Row1List);
            targetRows.Add(playerResources.Row2List);
            targetRows.Add(playerResources.Row3List);
        }

        if (triggerOnEnemyCards)
        {
            targetRows.Add(playerResources.myEnemy.Row1List);
            targetRows.Add(playerResources.myEnemy.Row2List);
            targetRows.Add(playerResources.myEnemy.Row3List);
        }

        foreach (var row in targetRows)
        {
            for (int i = row.Count - 1; i >= 0; i--)
            {
                if (row[i] == card) continue;
                if (onlyWoundedCards && row[i].Power >= row[i].BasePower) continue;
                if (onlyBoostedCards && row[i].Power <= row[i].BasePower) continue;
                row[i].Power += power;
            }
        }
    }
}