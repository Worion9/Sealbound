using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChooseDamageOrBoostRowAbility : Ability
{
    private readonly int boost;
    private readonly int damage;

    public ChooseDamageOrBoostRowAbility(int boost, int damage)
    {
        this.boost = boost;
        this.damage = damage;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (playerResources.selectionManager.option == 0)
        {
            List<Card> targetRow = card.ParentRowList.ToList();

            if (targetRow != null)
            {
                foreach (Card c in targetRow)
                {
                    if (c == card) continue;
                    c.Power += boost;
                }
            }
            else
            {
                Debug.LogWarning("Card not found in any row.");
            }

            playerResources.selectionManager.option = -1;
        }
        else if (playerResources.selectionManager.option == 1)
        {
            List<Card> targetRow = playerResources.Row1List.Contains(card) ? playerResources.myEnemy.Row1List.ToList()
                           : playerResources.Row2List.Contains(card) ? playerResources.myEnemy.Row2List.ToList()
                           : playerResources.Row3List.Contains(card) ? playerResources.myEnemy.Row3List.ToList()
                           : null;

            if (targetRow != null)
            {
                List<Card> targetRowCopy = new(targetRow);

                foreach (Card c in targetRowCopy)
                {
                    c.Power -= damage;
                }
            }
            else
            {
                Debug.LogWarning("Card not found in any row.");
            }

            playerResources.selectionManager.option = -1;
        }
        else
        {
            Card option1 = CardDatabase.cardList[card.Id].Clone();
            option1.Description = "Boost all other cards in this row by 1.";

            Card option2 = CardDatabase.cardList[card.Id].Clone();
            option2.Description = "Deal 1 damage to all cards in the opposite row.";

            List<Card> cardsToShow = new() { option1, option2 };

            playerResources.selectionManager.SetUpSelection(playerResources, cardsToShow, true, true);
        }
    }
}
