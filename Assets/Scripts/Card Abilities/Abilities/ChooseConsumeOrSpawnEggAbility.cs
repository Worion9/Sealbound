using System.Collections.Generic;

public class ChooseConsumeOrSpawnEggAbility : Ability
{
    private readonly int cardId;

    public ChooseConsumeOrSpawnEggAbility(int cardId)
    {
        this.cardId = cardId;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (playerResources.selectionManager.option == 0)
        {
            List<Card> cardsToConsume = new();
            List<Card> currentRow = card.ParentRowList;

            if (currentRow != null)
            {
                int cardIndex = currentRow.IndexOf(card);

                if (cardIndex > 0)
                {
                    cardsToConsume.Add(currentRow[cardIndex - 1]);
                }
                if (cardIndex < currentRow.Count - 1)
                {
                    cardsToConsume.Add(currentRow[cardIndex + 1]);
                }
            }

            foreach (var c in cardsToConsume)
            {
                if (c.isInvulnerable) continue;
                card.Power += c.Power;
                CardManager.KillCard(c);
            }

            playerResources.selectionManager.option = -1;
        }

        else if (playerResources.selectionManager.option == 1)
        {
            card.power -= 4;
            card.DisplayCard.UpdateCardUI();

            Card cardToCreate = CardDatabase.cardList[cardId].Clone();
            playerResources.DeckList.Insert(0, cardToCreate);
            playerResources.cardManager.ForceCardToBePlayed(playerResources, 0, false, false);

            playerResources.selectionManager.option = -1;
        }

        else
        {
            Card option1 = CardDatabase.cardList[card.Id].Clone();
            option1.Description = "Destroy a card to your right and left and boost me with their power.";

            Card option2 = CardDatabase.cardList[card.Id].Clone();
            option2.Description = "Deal 4 damage to me, then create and play <b>Larva<b>.";

            List<Card> cardsToShow = new() { option1, option2 };

            playerResources.selectionManager.SetUpSelection(playerResources, cardsToShow, true, true);
        }
    }
}
