using System.Collections.Generic;

public class CooldownBoostCardAbility : Ability
{
    public readonly int boost;
    public readonly int renewal;

    public CooldownBoostCardAbility(int boost, int renewal)
    {
        this.boost = boost;
        this.renewal = renewal;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (renewal > 1)
        {
            card.Counter--;

            if (card.Counter <= 0)
            {
                card.Counter = renewal;
            }
            else return;
        }

        List<Card> currentRow = card.ParentRowList;
        if (currentRow == null) return;
        int cardIndex = currentRow.IndexOf(card);

        if (cardIndex < currentRow.Count - 1)
        {
            currentRow[cardIndex + 1].Power += boost;
        }
    }
}