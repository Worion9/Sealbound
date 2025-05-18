using System.Collections;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnsLogic : NetworkBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private TextMeshProUGUI actionsLeftText;
    [SerializeField] private GameObject player1EndTurnButton;
    public GameObject mulliganInfoPanel;
    [SerializeField] private int pointsEveryTurnChange;
    [SerializeField] private GameInitializer initializer;
    [SerializeField] private GameObject loadingGamePanel;
    private readonly PlayerAccount playerAccount = new();
    private PlayerDataManager playerDataManager;

    public PlayerResources player1;
    public PlayerResources player2;
    public EnemyAI enemyAI;
    public TutorialLogic tutorialLogic;
    public bool tutorialGame;

    public readonly NetworkVariable<int> TurnNumber = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public readonly NetworkVariable<bool> IsMainPlayerTurn = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public readonly NetworkVariable<bool> IsMainPlayerStartingGame = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public static int turnNumber = 0;
    public bool isMainPlayerTurn;
    public bool isMainPlayerStartingGame;

    public UIManager uiManager;
    private readonly CardTriggers triggers = new();

    public static bool IsGameOnline { get; set; }

    void Start()
    {
        playerDataManager = new(playerAccount);
        playerDataManager.LoadFromLocal();

        tutorialGame = tutorialLogic.IsThisTutorial();
        uiManager = new UIManager(turnText, turnNumberText, actionsLeftText, player1EndTurnButton);

        IsGameOnline = playerAccount.IsGameOnline;
        if (IsGameOnline)
        {
            loadingGamePanel.SetActive(true);
            int playerId = NetworkManager.Singleton.IsHost ? 1 : 2;
            SetReadyServerRpc(playerId);

            StartCoroutine(WaitForPlayersThenStart());
        }
        else
        {
            Debug.Log("Offline Game detected, starting immediately.");
            StartGame();
        }

        clientDeckCode.OnValueChanged += (oldValue, newValue) =>
        {
            if (IsHost && !newValue.IsEmpty)
            {
                SetUpGame();
            }
        };
    }

    public override void OnNetworkSpawn()
    {
        isReady.OnValueChanged += OnIsReadyChanged;
        if (isReady.Value) OnIsReadyChanged(false, true);

        TurnNumber.OnValueChanged += (oldVal, newVal) =>
        {
            turnNumber = newVal;
            uiManager.UpdateRoundCounterDisplay(turnNumber);
        };

        IsMainPlayerTurn.OnValueChanged += (oldVal, newVal) =>
        {
            isMainPlayerTurn = newVal;
            uiManager.SetTurnText(IsHost ? isMainPlayerTurn : !isMainPlayerTurn);
            uiManager.SetTrunButton(this);
        };

        IsMainPlayerStartingGame.OnValueChanged += (oldVal, newVal) =>
        {
            isMainPlayerStartingGame = newVal;
        };
    }

    private new void OnDestroy()
    {
        isReady.OnValueChanged -= OnIsReadyChanged;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetReadyServerRpc(int playerId)
    {
        OnlineGameManager.Instance.SetPlayerReadyStatus(playerId, true);
        Debug.Log($"[TurnsLogic] Player {playerId} is now marked as ready.");
    }

    private IEnumerator WaitForPlayersThenStart()
    {
        while (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton == null!");
            yield return null;
        }

        SetReadyServerRpc(NetworkManager.Singleton.IsHost ? 1 : 2);

        while (!OnlineGameManager.Instance.AreBothPlayersReady()) yield return null;

        var myPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject;

        if (myPlayerObject == null)
        {
            Debug.LogError("Nie znaleziono PlayerObject dla lokalnego klienta!");
            yield break;
        }
        
        if (!myPlayerObject.TryGetComponent<PlayerNetworkResources>(out var myPlayerNetworkResources))
        {
            Debug.LogError("Nie znaleziono komponentu PlayerNetworkResources!");
            yield break;
        }

        RegisterSelfToServerServerRpc(myPlayerNetworkResources.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RegisterSelfToServerServerRpc(NetworkObjectReference playerRef)
    {
        OnlineGameManager.Instance.RegisterPlayerNetworkResources(playerRef);
    }

    private readonly NetworkVariable<FixedString512Bytes> clientDeckCode = new(new FixedString512Bytes("P0O"), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public void StartGame()
    {
        Debug.LogWarning("=============================================== Rozpoczynanie inicjalizaji!");

        if (IsGameOnline && !IsHost)
        {
            FixedString512Bytes deckCode = PlayerPrefs.GetString("CurrentDeck", "P0O");
            SendDeckCodeToServerRpc(deckCode);
        }
        else if (!IsGameOnline) SetUpGame();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendDeckCodeToServerRpc(FixedString512Bytes deckCode)
    {
        clientDeckCode.Value = deckCode;
    }

    private readonly NetworkVariable<bool> isReady = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void SetUpGame()
    {
        if (!IsGameOnline) player2.isAI = true;
        if (IsGameOnline && IsHost) TurnNumber.Value = 0;

        // inicjalizacja talii
        FixedString512Bytes hostDeckCode = PlayerPrefs.GetString("CurrentDeck", "P0O");

        initializer.InitializeSharedGameState(this);

        initializer.InitializeDeck(player1, this, hostDeckCode);
        if(IsGameOnline) initializer.InitializeDeck(player2, this, clientDeckCode.Value);
        else initializer.SetupDeck(player2, this, "");

        initializer.DrawStartingCards(player1);
        initializer.DrawStartingCards(player2);

        SpawnInitialStatue();

        if (IsGameOnline)
        {
            player1.SendData();
            player2.SendData();
            isReady.Value = true;
        }
        else StartCoroutine(DelayedOnIsReadyChanged(true));
    }

    private void OnIsReadyChanged(bool oldValue, bool newValue)
    {
        StartCoroutine(DelayedOnIsReadyChanged(newValue));
    }

    private IEnumerator DelayedOnIsReadyChanged(bool newValue)
    {
        yield return null;

        if (!newValue) yield break;

        if (IsGameOnline)
        {
            player1.ReceiveData();
            player2.ReceiveData();
        }

        player1.boardManager.RefreshAllCounters(player1);
        player2.boardManager.RefreshAllCounters(player2);

        if (IsHost || !IsGameOnline) initializer.FinalizePlayerInitialization(player1, this);
        if (!IsHost || !IsGameOnline) initializer.FinalizePlayerInitialization(player2, this);

        uiManager.UpdateRoundCounterDisplay(IsGameOnline ? TurnNumber.Value : turnNumber);

        if (IsGameOnline && !IsHost)
        {
            FindFirstObjectByType<ChangeSide>().ChangeSideButton();
        }

        if (!IsGameOnline) uiManager.SetTurnText(isMainPlayerTurn);
        else uiManager.SetTurnText(IsHost ? isMainPlayerTurn : !isMainPlayerTurn);
        loadingGamePanel.SetActive(false);

        Debug.LogWarning("=============================================== Inicjalizacja zkoañczona pomyœlnie!");

        if (tutorialGame) tutorialLogic.PlayTutorial();
    }

    public void TryChangeTurnButtonHandler()
    {
        StartCoroutine(TryChangeTurnCorutine());
    }

    public IEnumerator TryChangeTurnCorutine()
    {
        isSyncing = true;

        player1EndTurnButton.SetActive(false);
        player1.CardsLeftToPlayAmmount = 0;
        player2.CardsLeftToPlayAmmount = 0;
        uiManager.SetActionsLeftVisibility(player1.CardsLeftToPlayAmmount);

        if (player1.boardManager.CardThatCausedSelection != null) player1.boardManager.CardThatCausedSelection.Charges = 0;

        if (IsGameOnline)
        {
            player1.SendData();
            player2.SendData();

            yield return new WaitForSeconds(0.1f);

            if (IsServer) ReciveDataClientRpc();
            else RequestReciveDataServerRpc();

            yield return new WaitForSeconds(0.1f);

            StartTurnEndServerRpc();
        }
        else
        {
            player1.boardManager.CheckIfSomeoneWon(player1);
            StartTurnEnd();
        }
    }

    [ClientRpc(RequireOwnership = false)]
    private void ReciveDataClientRpc()
    {
        player1.ReceiveData();
        player2.ReceiveData();

        if(IsServer) player1.boardManager.CheckIfSomeoneWon(player1);
        else player2.boardManager.CheckIfSomeoneWon(player2);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestReciveDataServerRpc()
    {
        ReciveDataClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartTurnEndServerRpc()
    {
        if (Time.timeScale == 0f) return;
        audioManager.PlaySFX(AudioManager.SFX.TurnChange);

        if (TurnNumber.Value > 0) IsMainPlayerTurn.Value = !IsMainPlayerTurn.Value;

        if (IsNewRound())
        {
            TurnNumber.Value++;
            if (TurnNumber.Value > 1) ChangeBaseHPClientRpc(pointsEveryTurnChange);
        }

        if (IsMainPlayerTurn.Value) player1.cardManager.StopForcingPlayer(player1);
        else player2.cardManager.StopForcingPlayer(player2);

        TriggerBeginningTurnEffects(IsMainPlayerTurn.Value ? player1 : player2);

        StartCoroutine(SendData());
    }

    private IEnumerator SendData()
    {
        yield return null;

        player1.SendData();
        player2.SendData();

        FullSyncClientRpc();
    }

    private void StartTurnEnd()
    {
        if (Time.timeScale == 0f) return;
        audioManager.PlaySFX(AudioManager.SFX.TurnChange);

        if (turnNumber > 0) isMainPlayerTurn = !isMainPlayerTurn;

        if (IsNewRound())
        {
            turnNumber++;
            player1.boardManager.playersBaseHP += pointsEveryTurnChange;
        }

        player1.cardManager.StopForcingPlayer(player1);
        player2.cardManager.StopForcingPlayer(player2);

        TriggerBeginningTurnEffects(isMainPlayerTurn ? player1 : player2);

        StartCoroutine(DelayedFullSync());
    }

    public void ChangeBaseHP(int pointsChange)
    {
        if (IsGameOnline && IsHost) ChangeBaseHPClientRpc(pointsChange);
        else if (IsGameOnline) RequestChangeBaseHPServerRpc(pointsChange);
        else player1.boardManager.playersBaseHP += pointsChange;

        StartCoroutine(RefreshAllCountersCorutine());
    }

    private IEnumerator RefreshAllCountersCorutine()
    {
        if (IsGameOnline) yield return new WaitForSeconds(0.1f);
        player1.boardManager.RefreshAllCounters(player1);
        player2.boardManager.RefreshAllCounters(player2);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestChangeBaseHPServerRpc(int pointsChange)
    {
        ChangeBaseHPClientRpc(pointsChange);
    }

    [ClientRpc]
    public void ChangeBaseHPClientRpc(int pointsChange)
    {
        player1.boardManager.playersBaseHP += pointsChange;
    }

    [ClientRpc]
    private void FullSyncClientRpc()
    {
        StartCoroutine(DelayedFullSync());
    }

    private IEnumerator DelayedFullSync()
    {
        if (IsGameOnline && !IsHost)
        {
            int tries = 0;

            do
            {
                yield return new WaitForSeconds(0.1f);
                player1.ReceiveData();
                player2.ReceiveData();
                tries++;
            } while (player1.CardsLeftToPlayAmmount != 1 && player2.CardsLeftToPlayAmmount != 1 && tries < 100);

            if (tries >= 100)
            {
                Debug.LogWarning("Timeout waiting for player to receive cardsLeftToPlay.");
            }
        }

        UpdateMainCounters();
        if(!IsGameOnline) uiManager.SetTurnText(isMainPlayerTurn);
        uiManager.UpdateRoundCounterDisplay(IsGameOnline ? TurnNumber.Value : turnNumber);
        uiManager.SetTrunButton(this);
        RefreshCardDescriptions();
        player1.boardManager.UpdateDeckAndGraveyardImages(player1);
        player2.boardManager.UpdateDeckAndGraveyardImages(player2);

        isSyncing = false;

        if (!IsGameOnline && player2.isAI && !isMainPlayerTurn) enemyAI.PlayCard();
    }

    private bool IsNewRound()
    {
        if (IsGameOnline)
            return (IsMainPlayerTurn.Value && IsMainPlayerStartingGame.Value) || (!IsMainPlayerTurn.Value && !IsMainPlayerStartingGame.Value) || TurnNumber.Value == 0;
        else
            return (isMainPlayerTurn && isMainPlayerStartingGame) || (!isMainPlayerTurn && !isMainPlayerStartingGame) || turnNumber == 0;
    }

    private void UpdateMainCounters()
    {
        player1.boardManager.RefreshMainPointCounters(player1);
        player2.boardManager.RefreshMainPointCounters(player2);
    }

    private void TriggerBeginningTurnEffects(PlayerResources playerResources)
    {
        if (TurnNumber.Value > 1 || turnNumber > 1) playerResources.cardManager.DrawCard(playerResources, false);

        AutoPlayMeFromDeckOrHandAbility.alreadyUsed = false;

        triggers.TriggerOnceFromDeckOrHandAtTheBeginningOfOwnerTurn(playerResources, playerResources.DeckList);
        triggers.TriggerOnceFromDeckOrHandAtTheBeginningOfOwnerTurn(playerResources, playerResources.HandList);

        triggers.TriggerAtTheBeginningOfOwnerTurn(playerResources, playerResources.Row1List);
        triggers.TriggerAtTheBeginningOfOwnerTurn(playerResources, playerResources.Row2List);
        triggers.TriggerAtTheBeginningOfOwnerTurn(playerResources, playerResources.Row3List);

        playerResources.myEnemy.CardsLeftToPlayAmmount = 0;
        playerResources.CardsLeftToPlayAmmount = 1;
    }

    private void SpawnInitialStatue()
    {
        if (tutorialLogic.tutorialStage == 1) return;

        var startingPlayer = IsMainPlayerStartingGame.Value ? player2 : player1;
        startingPlayer.boardManager.SpawnStatue(startingPlayer);

        player1.boardManager.RefreshAllCounters(player1);
        player2.boardManager.RefreshAllCounters(player2);
    }

    public void FFGame()
    {
        if (IsGameOnline)
        {
            if (IsHost) FFGameClientRpc(false);
            else RequestFFGameServerRpc();
        }
        else player1.boardManager.EndTheGame(false, player1);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestFFGameServerRpc()
    {
        FFGameClientRpc(true);
    }

    [ClientRpc]
    private void FFGameClientRpc(bool didPlayer1Win)
    {
        if (IsHost) player1.boardManager.EndTheGame(didPlayer1Win, player1);
        else player2.boardManager.EndTheGame(!didPlayer1Win, player2);
    }

    public void EndGame()
    {
        if (IsGameOnline && OnlineGameStarter.Instance != null)
        {
            OnlineGameStarter.Instance.EndGame();
        }

        else SceneManager.LoadScene("MainMenu");
    }

    public void HideMulliganInfo()
    {
        if (IsHost) HideMulliganInfoClientRpc(false);
        else RequestHideMulliganInfoServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestHideMulliganInfoServerRpc()
    {
        HideMulliganInfoClientRpc(true);
    }

    [ClientRpc]
    public void HideMulliganInfoClientRpc(bool forHost)
    {
        if (forHost && IsHost) mulliganInfoPanel.SetActive(false);
        else if (!forHost && !IsHost) mulliganInfoPanel.SetActive(false);
    }

    public bool isSyncing = true;

    public void QuickSync()
    {
        if (!IsGameOnline || isSyncing) return;
        isSyncing = true;

        StartCoroutine(QuickSyncCorutine());
    }

    private IEnumerator QuickSyncCorutine()
    {
        yield return null;

        player1.SendData();
        player2.SendData();

        yield return new WaitForSeconds(0.1f);

        if (IsHost) QuickSyncClientRpc(true);
        else QuickSyncServerRpc();

        yield return null;
        isSyncing = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void QuickSyncServerRpc()
    {
        QuickSyncClientRpc(false);
    }

    [ClientRpc]
    public void QuickSyncClientRpc(bool sendByHost)
    {
        if (IsHost == sendByHost) return;

        player1.ReceiveData();
        player2.ReceiveData();
    }

    public void RefreshCardDescriptions()
    {
        var allCards = player1.Row1List
            .Concat(player1.Row2List)
            .Concat(player1.Row3List)
            .Concat(player1.HandList)
            .Concat(player1.DeckList)
            .Concat(player1.GraveyardList)
            .Concat(player2.Row1List)
            .Concat(player2.Row2List)
            .Concat(player2.Row3List)
            .Concat(player2.HandList)
            .Concat(player2.DeckList)
            .Concat(player2.GraveyardList);

        foreach (var card in allCards)
        {
            if (card.Name == "Aggressive Giant")
            {
                card.Description = $"Deal {Mathf.Max(0, 12 - turnNumber)} damage to a card. Damage is reduced by 1 each turn.";
            }
        }
    }
}