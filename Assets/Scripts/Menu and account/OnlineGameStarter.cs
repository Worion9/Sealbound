using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using System;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;

public class OnlineGameStarter : NetworkBehaviour
{
    public static OnlineGameStarter Instance { get; private set; }
    public string _playerId;

    [Header("Settings")]
    public ushort port = 7777;
    public int maxPlayers = 2;
    public float lobbyReconciliationInterval = 3f; // How often to check for other lobbies when hosting

    [Header("References")]
    [SerializeField] private GameObject onlineGameManagerPrefab;
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private GameObject waitingForOpponentUI;
    [SerializeField] private GameObject loadingGamePanel;

    private UnityTransport transport;
    private Lobby currentLobby;
    private const string JoinCodeKey = "joinCode";
    private readonly HashSet<ulong> readyClients = new();
    private string lobbyCreationTime = DateTime.UtcNow.ToString("o"); // Timestamp when the lobby was created (ISO 8601 format)
    private Coroutine lobbyReconciliationCoroutine;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mainMenu = FindAnyObjectByType<MainMenu>();
        if (waitingForOpponentUI == null) waitingForOpponentUI = mainMenu.waitingForOpponentUI;
        if (loadingGamePanel == null) loadingGamePanel = mainMenu.loadingGamePanel;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("Brak NetworkManager.Singleton!");
            return;
        }

        transport = FindFirstObjectByType<UnityTransport>();
        await InitializeUnityServices();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        if (IsClient) StartCoroutine(DelayedReadySignal());
    }

    private new void OnDestroy()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }

        StopLobbyReconciliation();
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Gracz {clientId} po³¹czony.");

        // When client connects, we can stop looking for other lobbies
        if (lobbyReconciliationCoroutine != null)
        {
            StopLobbyReconciliation();
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (readyClients.Remove(clientId)) Debug.Log($"Gracz {clientId} opuœci³ grê. Gotowych graczy: {readyClients.Count}");
    }

    private async Task InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn) await AuthenticationService.Instance.SignInAnonymouslyAsync();

            _playerId = AuthenticationService.Instance.PlayerId;
            Debug.Log($"Player ID: {_playerId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"B³¹d inicjalizacji Unity Services: {e.Message}");
        }
    }

    public async void PlayOnlineGame()
    {
        if(waitingForOpponentUI != null) waitingForOpponentUI.SetActive(true);

        // Try to join an existing lobby first
        currentLobby = await QuickJoinLobby();

        // If joining failed, create a new lobby
        if (currentLobby == null)
        {
            currentLobby = await CreateLobby();

            // Start periodically checking for other lobbies after creating our own
            if (currentLobby != null)
            {
                lobbyCreationTime = DateTime.UtcNow.ToString("o"); // ISO 8601 format
                StartLobbyReconciliation();
            }
        }
    }

    private void StartLobbyReconciliation()
    {
        StopLobbyReconciliation();
        lobbyReconciliationCoroutine = StartCoroutine(LobbyReconciliationCoroutine());
    }

    private void StopLobbyReconciliation()
    {
        if (lobbyReconciliationCoroutine != null)
        {
            StopCoroutine(lobbyReconciliationCoroutine);
            lobbyReconciliationCoroutine = null;
        }
    }

    private IEnumerator LobbyReconciliationCoroutine()
    {
        var waitTime = new WaitForSecondsRealtime(lobbyReconciliationInterval);

        while (currentLobby != null && NetworkManager.Singleton.ConnectedClientsList.Count < maxPlayers)
        {
            // Only search for other lobbies if we're still alone
            if (NetworkManager.Singleton.ConnectedClientsList.Count <= 1)
            {
                _ = CheckForBetterLobby();
            }
            else
            {
                // If we have connected clients, stop reconciliation
                break;
            }

            yield return waitTime;
        }

        lobbyReconciliationCoroutine = null;
    }

    private async Task CheckForBetterLobby()
    {
        try
        {
            // Get list of all available lobbies
            QueryLobbiesOptions options = new()
            {
                Count = 25 // Maximum number of lobbies to retrieve
            };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            // Filter out our own lobby and check if any remain
            var filteredLobbies = new List<Lobby>();
            foreach (var lobby in lobbies.Results)
            {
                if (lobby.HostId != _playerId && lobby.AvailableSlots > 0)
                {
                    filteredLobbies.Add(lobby);
                }
            }

            // No other lobbies found
            if (filteredLobbies.Count == 0) return;

            // Look for an older lobby to join (this prevents oscillating between lobbies)
            Lobby lobbyToJoin = null;
            string currentTime = lobbyCreationTime;

            foreach (Lobby lobby in filteredLobbies)
            {
                if (LobbyIsOlderThan(lobby, currentTime))
                {
                    lobbyToJoin = lobby;
                    currentTime = lobby.Created.ToString(); // Update to join the oldest lobby available
                }
            }

            // If we found an older lobby, join it and delete our own
            if (lobbyToJoin != null)
            {
                Debug.Log($"Found an older lobby to join. Switching from our lobby to lobby {lobbyToJoin.Id}");
                await JoinExistingLobby(lobbyToJoin);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error checking for better lobbies: {e.Message}");
        }
    }

    private bool LobbyIsOlderThan(Lobby lobby, string comparisonTime)
    {
        try
        {
            // Compare lobby creation times
            string lobbyTimeStr = lobby.Created.ToString();
            DateTime lobbyTime = DateTime.Parse(lobbyTimeStr);
            DateTime comparison = DateTime.Parse(comparisonTime);

            return lobbyTime < comparison;
        }
        catch (Exception e)
        {
            // If parsing fails, default to false
            Debug.LogError($"Error comparing lobby times: {e.Message}");
            return false;
        }
    }

    private async Task JoinExistingLobby(Lobby lobby)
    {
        try
        {
            // Clean up our current lobby and network connection
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.Shutdown();
            }

            if (currentLobby != null && currentLobby.HostId == _playerId)
            {
                await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
                Debug.Log($"Deleted our lobby {currentLobby.Id} to join another one");
            }

            // Join the new lobby
            var joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            if (!joinedLobby.Data.ContainsKey(JoinCodeKey))
            {
                Debug.LogError("Joined lobby does not contain join code");
                return;
            }

            var joinCode = joinedLobby.Data[JoinCodeKey].Value;
            var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            SetTransformAsClient(allocation);

            // Update current lobby reference and start client
            currentLobby = joinedLobby;
            NetworkManager.Singleton.StartClient();

            // Stop looking for other lobbies
            StopLobbyReconciliation();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to join existing lobby: {e.Message}");
        }
    }

    private async Task<Lobby> QuickJoinLobby()
    {
        try
        {
            var lobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            var a = await RelayService.Instance.JoinAllocationAsync(lobby.Data[JoinCodeKey].Value);
            SetTransformAsClient(a);

            NetworkManager.Singleton.StartClient();
            return lobby;
        }
        catch
        {
            Debug.Log($"No lobbies available via quick join");
            return null;
        }
    }

    private async Task<Lobby> CreateLobby()
    {
        try
        {
            var a = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

            var options = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> { { JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) } }
            };
            var lobby = await LobbyService.Instance.CreateLobbyAsync("Useless Lobby Name", maxPlayers, options);

            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);
            NetworkManager.Singleton.StartHost();
            Instantiate(onlineGameManagerPrefab).GetComponent<NetworkObject>().Spawn();

            return lobby;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed creating a lobby: {e.Message}");
            return null;
        }
    }

    private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (currentLobby != null)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    private void SetTransformAsClient(JoinAllocation a)
    {
        transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
    }

    public async Task CancelSearchAsync()
    {
        StopLobbyReconciliation();

        if (NetworkManager.Singleton != null && (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)) NetworkManager.Singleton.Shutdown();

        if (currentLobby != null)
        {
            try
            {
                if (currentLobby.HostId == _playerId) await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
                else await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, _playerId);
                Debug.Log("Anulowano szukanie przeciwnika.");
            }
            catch (Exception e)
            {
                Debug.LogError($"B³¹d przy usuwaniu z lobby: {e.Message}");
            }
        }

        currentLobby = null;
        if (waitingForOpponentUI != null) waitingForOpponentUI.SetActive(false);
        if (loadingGamePanel != null) loadingGamePanel.SetActive(false);
    }

    private IEnumerator DelayedReadySignal()
    {
        yield return new WaitForSeconds(0.5f);
        SendReadyToServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendReadyToServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        if (!readyClients.Contains(clientId))
        {
            readyClients.Add(clientId);
            Debug.Log($"Gracz {clientId} jest gotowy! ({readyClients.Count}/{maxPlayers})");
            if (readyClients.Count == maxPlayers)
            {
                HideLoadingPanelClientRpc();
                NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
            }
        }
    }

    [ClientRpc]
    private void HideLoadingPanelClientRpc()
    {
        if (loadingGamePanel != null) loadingGamePanel.SetActive(true);
    }

    public async void EndGame()
    {
        StopLobbyReconciliation();
        StopAllCoroutines();

        readyClients.Clear();

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;

            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.Shutdown();
                Debug.Log("Zakoñczono po³¹czenie sieciowe.");
            }
        }

        if (currentLobby != null)
        {
            try
            {
                if (currentLobby.HostId == _playerId)
                {
                    // Jeœli jesteœmy hostem, usuñ lobby
                    await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
                    Debug.Log($"Usuniêto lobby {currentLobby.Id}");
                }
                else
                {
                    // Jeœli jesteœmy klientem, usuñ gracza z lobby
                    await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, _playerId);
                    Debug.Log($"Opuszczono lobby {currentLobby.Id}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"B³¹d przy opuszczaniu/usuwaniu lobby: {e.Message}");
            }

            currentLobby = null;
        }

        if (waitingForOpponentUI != null) waitingForOpponentUI.SetActive(false);
        if (loadingGamePanel != null) loadingGamePanel.SetActive(false);

        SceneManager.LoadScene("MainMenu");
    }
}