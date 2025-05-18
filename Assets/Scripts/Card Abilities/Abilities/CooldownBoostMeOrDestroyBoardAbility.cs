using System.Collections.Generic;

public class CooldownBoostMeOrDestroyBoardAbility : Ability
{
    public readonly int boost;
    public readonly int turnWhenDestroingBoard;

    public CooldownBoostMeOrDestroyBoardAbility(int boost, int turnWhenDestroingBoard)
    {
        this.boost = boost;
        this.turnWhenDestroingBoard = turnWhenDestroingBoard;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        card.power += boost;
        card.DisplayCard.UpdateCardUI();

        if (TurnsLogic.turnNumber >= turnWhenDestroingBoard)
        {
            List<Card> cardsToDestroy = new();

            cardsToDestroy.AddRange(playerResources.Row1List);
            cardsToDestroy.AddRange(playerResources.Row2List);
            cardsToDestroy.AddRange(playerResources.Row3List);

            foreach (Card c in cardsToDestroy)
            {
                if (c == card) continue;
                CardManager.KillCard(c);
            }
        }
    }
}