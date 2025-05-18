using System.Collections.Generic;
using UnityEngine;

public class ChooseBostMeByRandomNumberAbility : Ability
{
    private readonly int constantBoost;
    private readonly int minRandomBoost;
    private readonly int maxRandomBoost;

    public ChooseBostMeByRandomNumberAbility(int constantBoost, int minRandomBoost, int maxRandomBoost)
    {
        this.constantBoost = constantBoost;
        this.minRandomBoost = minRandomBoost;
        this.maxRandomBoost = maxRandomBoost;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (playerResources.selectionManager.option == 0)
        {
            card.Power += constantBoost;
            playerResources.selectionManager.option = -1;
        }
        else if (playerResources.selectionManager.option == 1)
        {
            card.Power += Random.Range(minRandomBoost, maxRandomBoost + 1);
            playerResources.selectionManager.option = -1;
        }
        else
        {
            Card option1 = CardDatabase.cardList[card.Id].Clone();
            option1.Description = "Boost me by 8.";

            Card option2 = CardDatabase.cardList[card.Id].Clone();
            option2.Description = "Boost me by a random amount from 0 to 16.";

            List<Card> cardsToShow = new() { option1, option2 };

            playerResources.selectionManager.SetUpSelection(playerResources, cardsToShow, true, true);
        }
    }
}
