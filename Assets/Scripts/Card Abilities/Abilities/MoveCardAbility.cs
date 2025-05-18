using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCardAbility : Ability
{
    private readonly int needToHaveThisOrLowerPower;
    private readonly bool toOppositeRow;

    public MoveCardAbility(int needToHaveThisOrLowerPower = -1, bool toOppositeRow = false)
    {
        this.needToHaveThisOrLowerPower = needToHaveThisOrLowerPower;
        this.toOppositeRow = toOppositeRow;
    }

    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        if (needToHaveThisOrLowerPower != -1 && targetedCard.Power > needToHaveThisOrLowerPower)
        {
            Debug.Log("Selected card has too much power!");
            playerResources.boardManager.CardThatCausedSelection.Charges++;
            playerResources.boardManager.StartCoroutine(ExecuteAfterOneFrame(playerResources));
            return;
        }

        if (!toOppositeRow)
        {
            playerResources.boardManager.MoveCard(playerResources, targetedCard);
        }
        else
        {
            Transform container;
            List<Card> list;

            if (targetedCard.ParentRowList == playerResources.turnsLogic.player1.Row1List)
            {
                container = playerResources.turnsLogic.player2.row1Container.transform;
                list = playerResources.turnsLogic.player2.Row1List;
            }
            else if (targetedCard.ParentRowList == playerResources.turnsLogic.player1.Row2List)
            {
                container = playerResources.turnsLogic.player2.row2Container.transform;
                list = playerResources.turnsLogic.player2.Row2List;
            }
            else if (targetedCard.ParentRowList == playerResources.turnsLogic.player1.Row3List)
            {
                container = playerResources.turnsLogic.player2.row3Container.transform;
                list = playerResources.turnsLogic.player2.Row3List;
            }
            else if (targetedCard.ParentRowList == playerResources.turnsLogic.player2.Row1List)
            {
                container = playerResources.turnsLogic.player1.row1Container.transform;
                list = playerResources.turnsLogic.player1.Row1List;
            }
            else if (targetedCard.ParentRowList == playerResources.turnsLogic.player2.Row2List)
            {
                container = playerResources.turnsLogic.player1.row2Container.transform;
                list = playerResources.turnsLogic.player1.Row2List;
            }
            else if (targetedCard.ParentRowList == playerResources.turnsLogic.player2.Row3List)
            {
                container = playerResources.turnsLogic.player1.row3Container.transform;
                list = playerResources.turnsLogic.player1.Row3List;
            }
            else
            {
                Debug.LogError("Card not found in any row list!");
                return;
            }

            CardManager.KillCard(targetedCard, false, false);
            playerResources.cardManager.AddCardToRow(targetedCard, playerResources, container, list);
        }
    }

    private IEnumerator ExecuteAfterOneFrame(PlayerResources playerResources)
    {
        yield return null;
        playerResources.boardManager.SetUpFromBardSelection(playerResources, playerResources.boardManager.CardThatCausedSelection);
    }
}