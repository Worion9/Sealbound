using System.Collections.Generic;

public class ShowCardsPlayOneAbility : Ability
{
    private readonly int[] optionCardsIds;

    public ShowCardsPlayOneAbility(int[] optionCardsIds)
    {
        this.optionCardsIds = optionCardsIds;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        List<Card> cardsToShow = new();

        for (int i = 0; i < optionCardsIds.Length; i++)
        {
            Card newCard = CardDatabase.cardList[optionCardsIds[i]].Clone();
            cardsToShow.Add(newCard);
        }

        playerResources.selectionManager.SetUpSelection(playerResources, cardsToShow, true);
    }
}
