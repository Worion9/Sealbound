using UnityEngine;

public class SpawnCardAbility : Ability
{
    private readonly int cardIdModifier;
    private readonly bool onEnemeyRow;
    private readonly bool exactCopyOfMe;

    public SpawnCardAbility(int cardIdModifier, bool onEnemeyRow = false, bool exactCopyOfMe = false)
    {
        this.cardIdModifier = cardIdModifier;
        this.onEnemeyRow = onEnemeyRow;
        this.exactCopyOfMe = exactCopyOfMe;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        Card cardToSummon;
        if (exactCopyOfMe)
        {
            cardToSummon = card.Clone();
        }
        else
        {
            cardToSummon = CardDatabase.cardList[card.Id + cardIdModifier].Clone();
        }

        int targetRowIndex = -1;

        if (playerResources.Row1List.Contains(card))
        {
            targetRowIndex = 0;
        }
        else if (playerResources.Row2List.Contains(card))
        {
            targetRowIndex = 1;
        }
        else if (playerResources.Row3List.Contains(card))
        {
            targetRowIndex = 2;
        }
        else
        {
            Debug.LogError("Card not fount in row!");
        }

        if (!onEnemeyRow)
        {
            targetRowIndex = playerResources.boardManager.ReturnFirstValidRow(playerResources, targetRowIndex);
            playerResources.cardManager.SummonCard(playerResources, cardToSummon, false, null, targetRowIndex, true);
        }
        else
        {
            targetRowIndex = playerResources.boardManager.ReturnFirstValidRow(playerResources.myEnemy, targetRowIndex);
            playerResources.cardManager.SummonCard(playerResources.myEnemy, cardToSummon, false, null, targetRowIndex, true);
        }
    }
}
