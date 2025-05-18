using System.Linq;

public class PlayCardFromDeckAbility : Ability
{
    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        bool containsCardWithSameId = playerResources.DeckList.Any(card => card.Id == targetedCard.Id);

        if (containsCardWithSameId)
        {
            Card cardToPlay = playerResources.cardManager.GetRandomMatchingCard(playerResources.DeckList, targetedCard);
            int index = playerResources.DeckList.IndexOf(cardToPlay);
            playerResources.cardManager.ForceCardToBePlayed(playerResources, index, false);
        }
    }
}