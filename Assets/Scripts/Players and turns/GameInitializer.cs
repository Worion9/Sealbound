using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class GameInitializer : NetworkBehaviour
{
    public void InitializeSharedGameState(TurnsLogic turnsLogic)
    {
        TurnsLogic.turnNumber = 0;

        if (turnsLogic.tutorialGame)
        {
            turnsLogic.isMainPlayerTurn = false;
            turnsLogic.isMainPlayerStartingGame = false;
        }
        else
        {
            var random = Random.value > 0.5f;

            turnsLogic.IsMainPlayerTurn.Value = random;
            turnsLogic.isMainPlayerTurn = random;

            turnsLogic.IsMainPlayerStartingGame.Value = random;
            turnsLogic.isMainPlayerStartingGame = random;
        }
    }

    public void InitializeDeck(PlayerResources player, TurnsLogic turnsLogic, FixedString512Bytes deckCode)
    {
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        SetupDeck(player, turnsLogic, deckCode);
    }

    public void DrawStartingCards(PlayerResources player)
    {
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        for (int i = 0; i < player.cardManager.startingHandSize; i++)
        {
            if (player.DeckList.Count > 0) player.cardManager.DrawCard(player, false);
        }
    }

    public void FinalizePlayerInitialization(PlayerResources player, TurnsLogic turnsLogic)
    {
        if (player == null)
        {
            Debug.LogWarning("Player not found!");
            return;
        }

        int mulliganAmount = Mathf.Min(player.mulliganManager.startingMulliganAmount, player.DeckList.Count);
        if (mulliganAmount > 0 && !player.isAI)
        {
            player.mulliganManager.MulliganCards(player, mulliganAmount, false);
            if (!player.myEnemy.isAI) turnsLogic.mulliganInfoPanel.SetActive(true);
        }
        else player.isPlayerInMulliganFaze = false;

        AdjustDropZones(player);
    }

    public void SetupDeck(PlayerResources player, TurnsLogic turnsLogic, FixedString512Bytes deckCode)
    {
        if (deckCode != "") player.deckCode = deckCode.ToString();

        else if (!player.useThisCodeInstead)
        {
            player.deckCode = player.isAI ? PlayerPrefs.GetString("CurrentEnemyDeck", "P0O") : PlayerPrefs.GetString("CurrentDeck", "P0O");
        }

        Debug.Log($"Setting up {player.name} deck with code: {player.deckCode}");

        if (player.useRandomDeckInstead) player.deckManager.FillDeckWithRandomCards(player);
        else player.deckManager.FillDeckWithCardsFromCode(player.deckCode, player);

        if (turnsLogic.tutorialGame) turnsLogic.tutorialLogic.SetUpDecks(player);
    }

    private void AdjustDropZones(PlayerResources player)
    {
        player.row1Container.GetComponent<DropZone>().AjustRowTriggers();
        player.row2Container.GetComponent<DropZone>().AjustRowTriggers();
        player.row3Container.GetComponent<DropZone>().AjustRowTriggers();
    }
}