using System.Collections.Generic;
using System.Linq;

public class MoveAllCardsInRowAbility : Ability
{
    private readonly bool enemyRow;

    public MoveAllCardsInRowAbility(bool enemyRow)
    {
        this.enemyRow = enemyRow;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        List<Card> cartsToMove = new();
        if (enemyRow)
        {
            if (card.ParentRowList == playerResources.Row1List) cartsToMove = playerResources.myEnemy.Row1List.ToList();
            else if (card.ParentRowList == playerResources.Row2List) cartsToMove = playerResources.myEnemy.Row2List.ToList();
            else if (card.ParentRowList == playerResources.Row3List) cartsToMove = playerResources.myEnemy.Row3List.ToList();
        }
        else
        {
            if (card.ParentRowList == playerResources.Row1List)
                cartsToMove = playerResources.Row1List.Where(c => c != card).ToList();
            else if (card.ParentRowList == playerResources.Row2List)
                cartsToMove = playerResources.Row2List.Where(c => c != card).ToList();
            else if (card.ParentRowList == playerResources.Row3List)
                cartsToMove = playerResources.Row3List.Where(c => c != card).ToList();
        }

        playerResources.boardManager.MoveCard(playerResources, cartsToMove);
    }
}