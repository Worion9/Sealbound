using UnityEngine;

public class CreateACopyOfCardInDeckAbility : Ability
{
    public readonly int ammount;
    public readonly bool createInHandInstead;

    public CreateACopyOfCardInDeckAbility(int ammount, bool createInHandInstead = false)
    {
        this.ammount = ammount;
        this.createInHandInstead = createInHandInstead;
    }

    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        for (int i = 0; i < ammount; i++)
        {
            Card newCard = CardDatabase.cardList[targetedCard.Id].Clone();

            if (!createInHandInstead)
            {
                int randomIndex = Random.Range(0, playerResources.DeckList.Count + 1);
                playerResources.DeckList.Insert(randomIndex, newCard);
            }
            else
            {
                playerResources.DeckList.Insert(0, newCard);
                playerResources.cardManager.ForceCardToBePlayed(playerResources, 0, false);
            }
        }
        playerResources.boardManager.UpdateDeckAndGraveyardImages(playerResources);
    }
}