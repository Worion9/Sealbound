using System.Collections.Generic;

public class ChangeBasePowerOfAllCardsAbility : Ability
{
    public readonly int power;

    public ChangeBasePowerOfAllCardsAbility(int power)
    {
        this.power = power;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        List<Card> allCards = new();
        allCards.AddRange(playerResources.Row1List);
        allCards.AddRange(playerResources.Row2List);
        allCards.AddRange(playerResources.Row3List);
        allCards.AddRange(playerResources.HandList);
        allCards.AddRange(playerResources.DeckList);
        allCards.AddRange(playerResources.GraveyardList);

        foreach (Card c in allCards)
        {
            if (c == card) continue;
            if (c.IsSpy) continue;
            if (c.isSpell) continue;
            if (c.isInvulnerable) continue;
            c.BasePower += power;
            c.power += power;
            if(c.DisplayCard != null) c.DisplayCard.UpdateCardUI();
        }
    }
}