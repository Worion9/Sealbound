using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    public PlayerNetworkResources networkResources;

    [Header("Options")]
    public int maxHandSize;
    public int startingDeckSize;
    public bool showDeckInTrueOrder;
    public string deckCode;
    public bool useThisCodeInstead;
    public bool useRandomDeckInstead;
    public int randomCardMinIndex;
    public int randomCardMaxIndex;

    [Header("Logic")]
    public bool isActive;
    public bool isAI;
    public PlayerResources myEnemy;
    public int points;
    public int equalizationPoints;
    public int HP;
    private int cardsLeftToPlayAmmoun;
    public int CardsLeftToPlayAmmount
    {
        get => cardsLeftToPlayAmmoun;
        set
        {
            if (cardsLeftToPlayAmmoun != value)
            {
                cardsLeftToPlayAmmoun = value;
                turnsLogic.uiManager.SetActionsLeftText(cardsLeftToPlayAmmoun);
            }
        }
    }
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

    [Header("Lists")]
    public List<Card> DeckList = new();
    public List<Card> HandList = new();
    public List<Card> MulliganList = new();
    public List<Card> Row1List = new();
    public List<Card> Row2List = new();
    public List<Card> Row3List = new();
    public List<Card> MulliganAsideList = new();
    public List<Card> GraveyardList = new();
    public List<int> PlayedCardsIdsList = new();

    [Header("Hand")]
    public GameObject handCardContainer;

    [Header("Board")]
    public GameObject endTurnButton;
    public GameObject row1Container;
    public GameObject row2Container;
    public GameObject row3Container;
    public TextMeshProUGUI row1Counter;
    public TextMeshProUGUI row2Counter;
    public TextMeshProUGUI row3Counter;
    public TextMeshProUGUI mainCounter;
    public TextMeshProUGUI pointsToLoseCounter;
    public GameObject endGameScreenButton;
    public GameObject endGameScreenParent;
    public GameObject endGameScreen;
    public TextMeshProUGUI endGameScreenText;

    [Header("Mulligan")]
    public GameObject showMulliganButton;
    public GameObject mulliganScreenParent;
    public GameObject mulliganCardContainer;
    public GameObject asideCardContainer;
    public TextMeshProUGUI mulliganLeftText;

    [Header("Deck")]
    public GameObject deckImage;
    public GameObject deckScreenParent;
    public GameObject deckDisplayCardContainer;
    public TextMeshProUGUI deckScreenText;

    [Header("Graveyard")]
    public GameObject graveyardImage;
    public GameObject graveyardScreenParent;
    public GameObject graveyardCardContainer;
    public TextMeshProUGUI graveyardScreenText;

    [Header("Selection")]
    public GameObject showSelectionButton;
    public GameObject selectionScreenParent;
    public GameObject selectionCardContainer;

    [Header("Classes")]
    public TurnsLogic turnsLogic;
    public CardManager cardManager;
    public BoardManager boardManager;
    public MulliganManager mulliganManager;
    public DeckManager deckManager;
    public GraveyardManager graveyardManager;
    public SelectionManager selectionManager;
    public CardRenderer cardRenderer;
    public CardInformationPanelManager cardInformationPanelManager;
    public readonly CardTriggers cardTriggers = new();
    public ChangeSide changeSide;

    public void SendData()
    {
        Debug.Log($"{name} przesy³ane z {(networkResources.IsHost ? "hosta" : "klienta")} do sieci!");
        networkResources.InitializeFromServerRpc(ToPlayerResourcesState());
    }

    public void ReceiveData()
    {
        Debug.Log($"{name} pobrane z sieci!");
        ApplyNetworkChanges();
        RefreshUI();
    }

    private void ApplyNetworkChanges()
    {
        // Przenoszenie danych z sieci do lokalnych
        points = networkResources.points.Value;
        equalizationPoints = networkResources.equalizationPoints.Value;
        HP = networkResources.pointsToLose.Value;
        CardsLeftToPlayAmmount = networkResources.cardsLeftToPlayAmmount.Value;
        actionsLeft = networkResources.actionsLeft.Value;
        playerIsForced = networkResources.playerIsForced.Value;
        isPlayerInMulliganFaze = networkResources.isPlayerInMulliganFaze.Value;
        isPlayerInFromDeckSelectionFaze = networkResources.isPlayerInFromDeckSelectionFaze.Value;
        isPlayerInSpecialSelectionFaze = networkResources.isPlayerInSpecialSelectionFaze.Value;
        isPlayerInFromBoardSelectionFaze = networkResources.isPlayerInFromBoardSelectionFaze.Value;
        isSelectionFromGraveyard = networkResources.isSelectionFromGraveyard.Value;
        doAlternativeAction = networkResources.doAlternativeAction.Value;
        mostCommonCardInDeckCount = networkResources.mostCommonCardInDeckCount.Value;
        bearExecutions = networkResources.bearExecutions.Value;

        DeckList = networkResources.GetDeck().Select(sc => sc.ToCard()).ToList();
        HandList = networkResources.GetHand().Select(sc => sc.ToCard()).ToList();
        MulliganList = networkResources.GetMulligan().Select(sc => sc.ToCard()).ToList();
        Row1List = networkResources.GetRow1().Select(sc => sc.ToCard()).ToList();
        Row2List = networkResources.GetRow2().Select(sc => sc.ToCard()).ToList();
        Row3List = networkResources.GetRow3().Select(sc => sc.ToCard()).ToList();
        MulliganAsideList = networkResources.GetMulliganAside().Select(sc => sc.ToCard()).ToList();
        GraveyardList = networkResources.GetGraveyard().Select(sc => sc.ToCard()).ToList();
        PlayedCardsIdsList = networkResources.GetPlayedCardIds();
    }

    private void RefreshUI()
    {
        changeSide.DestroyAllCards();
        changeSide.RenderAllCards();
    }

    public PlayerResourcesState ToPlayerResourcesState()
    {
        return new PlayerResourcesState
        {
            points = points,
            equalizationPoints = equalizationPoints,
            pointsToLose = HP,
            cardsLeftToPlayAmmount = CardsLeftToPlayAmmount,
            actionsLeft = actionsLeft,
            playerIsForced = playerIsForced,
            isPlayerInMulliganFaze = isPlayerInMulliganFaze,
            isPlayerInFromDeckSelectionFaze = isPlayerInFromDeckSelectionFaze,
            isPlayerInSpecialSelectionFaze = isPlayerInSpecialSelectionFaze,
            isPlayerInFromBoardSelectionFaze = isPlayerInFromBoardSelectionFaze,
            isSelectionFromGraveyard = isSelectionFromGraveyard,
            doAlternativeAction = doAlternativeAction,
            mostCommonCardInDeckCount = mostCommonCardInDeckCount,
            bearExecutions = bearExecutions,

            deckList = DeckList.Select(c => new SerializableCard(c)).ToList(),
            handList = HandList.Select(c => new SerializableCard(c)).ToList(),
            mulliganList = MulliganList.Select(c => new SerializableCard(c)).ToList(),
            row1List = Row1List.Select(c => new SerializableCard(c)).ToList(),
            row2List = Row2List.Select(c => new SerializableCard(c)).ToList(),
            row3List = Row3List.Select(c => new SerializableCard(c)).ToList(),
            mulliganAsideList = MulliganAsideList.Select(c => new SerializableCard(c)).ToList(),
            graveyardList = GraveyardList.Select(c => new SerializableCard(c)).ToList(),
            playedCardsIdsList = PlayedCardsIdsList.ToList()
        };
    }
}