public class DrawCardsToAmountAbility : Ability
{
    public int targetHandSize;

    public DrawCardsToAmountAbility(int targetHandSize)
    {
        this.targetHandSize = targetHandSize;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        int currentHandSize = playerResources.HandList.Count;
        int cardsToDraw = targetHandSize - currentHandSize;

        if (cardsToDraw > 0)
        {
            for (int i = 0; i < cardsToDraw; i++)
            {
                playerResources.cardManager.DrawCard(playerResources, true);
            }
        }
    }
}