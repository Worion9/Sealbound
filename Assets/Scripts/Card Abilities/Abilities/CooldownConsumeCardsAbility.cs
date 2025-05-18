using System.Collections.Generic;

public class CooldownConsumeCardsAbility : Ability
{
    public readonly int renewal;

    public CooldownConsumeCardsAbility(int renewal)
    {
        this.renewal = renewal;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        card.Counter--;
        if (card.Counter <= 0)
        {
            card.Counter = renewal;

            List<Card> currentRow = null;

            switch (card.DisplayCard.transform.parent.GetComponent<DropZone>().associatedListIndex)
            {
                case 1:
                    currentRow = playerResources.Row1List;
                    break;
                case 2:
                    currentRow = playerResources.Row2List;
                    break;
                case 3:
                    currentRow = playerResources.Row3List;
                    break;
            }

            if (currentRow != null)
            {
                int cardIndex = currentRow.IndexOf(card);

                if (cardIndex < currentRow.Count - 1)
                {
                    Card cardToConsume = currentRow[cardIndex + 1];
                    if (cardToConsume.isInvulnerable) return;

                    card.Power += cardToConsume.Power;
                    CardManager.KillCard(cardToConsume);
                }
            }
        }
    }
}