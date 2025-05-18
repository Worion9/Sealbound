using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkResources : NetworkBehaviour
{
    [Header("Networked Values")]
    public NetworkVariable<int> points = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> equalizationPoints = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> pointsToLose = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> cardsLeftToPlayAmmount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> actionsLeft = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> playerIsForced = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> isPlayerInMulliganFaze = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> isPlayerInFromDeckSelectionFaze = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> isPlayerInSpecialSelectionFaze = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> isPlayerInFromBoardSelectionFaze = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> isSelectionFromGraveyard = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> doAlternativeAction = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> mostCommonCardInDeckCount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> bearExecutions = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("Networked Card Lists")]
    public NetworkList<SerializableCard> deckList = new();
    public NetworkList<SerializableCard> handList = new();
    public NetworkList<SerializableCard> mulliganList = new();
    public NetworkList<SerializableCard> row1List = new();
    public NetworkList<SerializableCard> row2List = new();
    public NetworkList<SerializableCard> row3List = new();
    public NetworkList<SerializableCard> mulliganAsideList = new();
    public NetworkList<SerializableCard> graveyardList = new();
    public NetworkList<int> playedCardsIdsList = new();

    /*private void Start()
    {
        points.OnValueChanged += (oldValue, newValue) => Debug.Log($"Points changed: {oldValue} -> {newValue}");
        equalizationPoints.OnValueChanged += (oldValue, newValue) => Debug.Log($"Equalization Points changed: {oldValue} -> {newValue}");
        pointsToLose.OnValueChanged += (oldValue, newValue) => Debug.Log($"Points to lose changed: {oldValue} -> {newValue}");
        cardsLeftToPlayAmmount.OnValueChanged += (oldValue, newValue) => Debug.Log($"Cards left to play changed: {oldValue} -> {newValue}");
        actionsLeft.OnValueChanged += (oldValue, newValue) => Debug.Log($"Actions left changed: {oldValue} -> {newValue}");
    }*/

    [ServerRpc(RequireOwnership = false)]
    public void InitializeFromServerRpc(PlayerResourcesState playerResources)
    {
        points.Value = playerResources.points;
        equalizationPoints.Value = playerResources.equalizationPoints;
        pointsToLose.Value = playerResources.pointsToLose;
        cardsLeftToPlayAmmount.Value = playerResources.cardsLeftToPlayAmmount;
        actionsLeft.Value = playerResources.actionsLeft;
        playerIsForced.Value = playerResources.playerIsForced;
        isPlayerInMulliganFaze.Value = playerResources.isPlayerInMulliganFaze;
        isPlayerInFromDeckSelectionFaze.Value = playerResources.isPlayerInFromDeckSelectionFaze;
        isPlayerInSpecialSelectionFaze.Value = playerResources.isPlayerInSpecialSelectionFaze;
        isPlayerInFromBoardSelectionFaze.Value = playerResources.isPlayerInFromBoardSelectionFaze;
        isSelectionFromGraveyard.Value = playerResources.isSelectionFromGraveyard;
        doAlternativeAction.Value = playerResources.doAlternativeAction;
        mostCommonCardInDeckCount.Value = playerResources.mostCommonCardInDeckCount;
        bearExecutions.Value = playerResources.bearExecutions;

        CopyCardList(playerResources.deckList, deckList);
        CopyCardList(playerResources.handList, handList);
        CopyCardList(playerResources.mulliganList, mulliganList);
        CopyCardList(playerResources.row1List, row1List);
        CopyCardList(playerResources.row2List, row2List);
        CopyCardList(playerResources.row3List, row3List);
        CopyCardList(playerResources.mulliganAsideList, mulliganAsideList);
        CopyCardList(playerResources.graveyardList, graveyardList);

        CopyIntList(playerResources.playedCardsIdsList, playedCardsIdsList);
    }

    private void CopyCardList(List<SerializableCard> source, NetworkList<SerializableCard> target)
    {
        target.Clear();
        foreach (var card in source)
        {
            target.Add(new SerializableCard(card));
        }
    }

    private void CopyIntList(List<int> source, NetworkList<int> target)
    {
        target.Clear();
        foreach (var id in source)
        {
            target.Add(id);
        }
    }

    public List<SerializableCard> GetDeck() => ToList(deckList);
    public List<SerializableCard> GetHand() => ToList(handList);
    public List<SerializableCard> GetMulligan() => ToList(mulliganList);
    public List<SerializableCard> GetRow1() => ToList(row1List);
    public List<SerializableCard> GetRow2() => ToList(row2List);
    public List<SerializableCard> GetRow3() => ToList(row3List);
    public List<SerializableCard> GetMulliganAside() => ToList(mulliganAsideList);
    public List<SerializableCard> GetGraveyard() => ToList(graveyardList);
    public List<int> GetPlayedCardIds() => ToIntList(playedCardsIdsList);

    private List<SerializableCard> ToList(NetworkList<SerializableCard> source)
    {
        List<SerializableCard> result = new();
        foreach (var item in source)
        {
            result.Add(item);
        }
        return result;
    }

    private List<int> ToIntList(NetworkList<int> source)
    {
        List<int> result = new();
        foreach (var item in source)
        {
            result.Add(item);
        }
        return result;
    }
}