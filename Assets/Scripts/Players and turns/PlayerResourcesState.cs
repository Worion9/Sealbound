using System.Collections.Generic;
using Unity.Netcode;

[System.Serializable]
public struct PlayerResourcesState : INetworkSerializable
{
    public int points;
    public int equalizationPoints;
    public int pointsToLose;
    public int cardsLeftToPlayAmmount;
    public int actionsLeft;
    public bool playerIsForced;
    public bool isPlayerInMulliganFaze;
    public bool isPlayerInFromDeckSelectionFaze;
    public bool isPlayerInSpecialSelectionFaze;
    public bool isPlayerInFromBoardSelectionFaze;
    public bool isSelectionFromGraveyard;
    public bool doAlternativeAction;
    public int mostCommonCardInDeckCount;
    public int bearExecutions;

    public List<SerializableCard> deckList;
    public List<SerializableCard> handList;
    public List<SerializableCard> mulliganList;
    public List<SerializableCard> row1List;
    public List<SerializableCard> row2List;
    public List<SerializableCard> row3List;
    public List<SerializableCard> mulliganAsideList;
    public List<SerializableCard> graveyardList;

    public List<int> playedCardsIdsList;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref points);
        serializer.SerializeValue(ref equalizationPoints);
        serializer.SerializeValue(ref pointsToLose);
        serializer.SerializeValue(ref cardsLeftToPlayAmmount);
        serializer.SerializeValue(ref actionsLeft);
        serializer.SerializeValue(ref playerIsForced);
        serializer.SerializeValue(ref isPlayerInMulliganFaze);
        serializer.SerializeValue(ref isPlayerInFromDeckSelectionFaze);
        serializer.SerializeValue(ref isPlayerInSpecialSelectionFaze);
        serializer.SerializeValue(ref isPlayerInFromBoardSelectionFaze);
        serializer.SerializeValue(ref isSelectionFromGraveyard);
        serializer.SerializeValue(ref doAlternativeAction);
        serializer.SerializeValue(ref mostCommonCardInDeckCount);
        serializer.SerializeValue(ref bearExecutions);

        SerializeList(serializer, ref deckList);
        SerializeList(serializer, ref handList);
        SerializeList(serializer, ref mulliganList);
        SerializeList(serializer, ref row1List);
        SerializeList(serializer, ref row2List);
        SerializeList(serializer, ref row3List);
        SerializeList(serializer, ref mulliganAsideList);
        SerializeList(serializer, ref graveyardList);

        SerializeIntList(serializer, ref playedCardsIdsList);
    }

    private static void SerializeList<T, TValue>(BufferSerializer<T> serializer, ref List<TValue> list)
        where T : IReaderWriter
        where TValue : INetworkSerializable, new()
    {
        if (serializer.IsWriter)
        {
            int count = list?.Count ?? 0;
            serializer.SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                var item = list[i];
                item.NetworkSerialize(serializer);
            }
        }
        else
        {
            int count = 0;
            serializer.SerializeValue(ref count);
            list = new List<TValue>(count);
            for (int i = 0; i < count; i++)
            {
                var item = new TValue();
                item.NetworkSerialize(serializer);
                list.Add(item);
            }
        }
    }

    private static void SerializeIntList<T>(BufferSerializer<T> serializer, ref List<int> list)
        where T : IReaderWriter
    {
        if (serializer.IsWriter)
        {
            int count = list?.Count ?? 0;
            serializer.SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                int val = list[i];
                serializer.SerializeValue(ref val);
            }
        }
        else
        {
            int count = 0;
            serializer.SerializeValue(ref count);
            list = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                int val = 0;
                serializer.SerializeValue(ref val);
                list.Add(val);
            }
        }
    }
}
