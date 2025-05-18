using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private PlayerResources AIPlayer;
    [SerializeField] private PlayerResources humanPlayer;
    [SerializeField] private TurnsLogic turnsLogic;
    [SerializeField] private float minThinkingTime;
    [SerializeField] private float maxThinkingTime;
    [SerializeField] private float minTimeToEndTurn;
    [SerializeField] private float maxTimeToEndTurn;

    public void PlayCard()
    {
        if (humanPlayer.isPlayerInMulliganFaze || turnsLogic.isMainPlayerTurn) return;
        StartCoroutine(PlayCardWithDelay());
    }

    private IEnumerator PlayCardWithDelay()
    {
        while (AIPlayer.CardsLeftToPlayAmmount > 0 && AIPlayer.HandList.Count > 0)
        {
            float thinkingTime = Random.Range(minThinkingTime, maxThinkingTime);
            yield return new WaitForSeconds(thinkingTime);

            if (AIPlayer.HandList.Count == 0)
            {
                Debug.Log("AI hand is empty.");
                break;
            }

            Card cardToPlay = AIPlayer.HandList[0];
            int selectedRow = GetTargetRow(cardToPlay);

            if (selectedRow == -1)
            {
                Debug.Log("All rows are full. AI cannot play any card.");
                break;
            }

            PlayCardOnBoard(cardToPlay, selectedRow);
            yield return new WaitForSeconds(0.05f);
        }

        if (AIPlayer.CardsLeftToPlayAmmount > 0 && AIPlayer.HandList.Count > 0)
        {
            StartCoroutine(PlayCardWithDelay());
            yield break;
        }

        float timeToEndTurn = Random.Range(minTimeToEndTurn, maxTimeToEndTurn);
        yield return new WaitForSeconds(timeToEndTurn);
        StartCoroutine(turnsLogic.TryChangeTurnCorutine());
    }

    private int GetTargetRow(Card card)
    {
        return card.IsSpy ? humanPlayer.boardManager.GetRandomRow(humanPlayer) : AIPlayer.boardManager.GetRandomRow(AIPlayer);
    }

    private void PlayCardOnBoard(Card card, int rowIndex)
    {
        var targetPlayer = card.IsSpy ? humanPlayer : AIPlayer;
        var targetRowList = GetRowList(targetPlayer, rowIndex);
        var targetContainer = GetRowContainer(targetPlayer, rowIndex);

        AIPlayer.cardManager.PlayCard(AIPlayer, 0, targetContainer, targetRowList);
    }

    private Transform GetRowContainer(PlayerResources player, int rowIndex)
    {
        return rowIndex switch
        {
            0 => player.row1Container.transform,
            1 => player.row2Container.transform,
            2 => player.row3Container.transform,
            _ => null
        };
    }

    private List<Card> GetRowList(PlayerResources player, int rowIndex)
    {
        return rowIndex switch
        {
            0 => player.Row1List,
            1 => player.Row2List,
            2 => player.Row3List,
            _ => null
        };
    }
}