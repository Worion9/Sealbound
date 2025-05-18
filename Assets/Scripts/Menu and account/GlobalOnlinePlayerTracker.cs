using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using System;

public class GlobalOnlinePlayerTracker : MonoBehaviour
{
    public static GlobalOnlinePlayerTracker Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private string presenceLobbyPrefix = "PRESENCE_";
    [SerializeField] private float heartbeatInterval = 15f; // Odstêp czasu miêdzy heartbeatami (w sekundach)
    [SerializeField] private float joinPresenceInterval = 60f; // Jak czêsto odœwie¿aæ obecnoœæ gracza (w sekundach)

    private string playerId;
    private Lobby presenceLobby;
    private string lobbyId;
    private Coroutine heartbeatCoroutine;
    private Coroutine presenceCoroutine;

    private void Awake()
    {
        // Implementacja Singletona
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
        await InitializeUnityServices();
        StartPresenceTracking();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        _ = LeavePresenceLobby();
    }

    private void OnApplicationQuit()
    {
        _ = LeavePresenceLobby();
    }

    private async Task InitializeUnityServices()
    {
        try
        {
            // Inicjalizacja Unity Services
            InitializationOptions options = new InitializationOptions();
            await UnityServices.InitializeAsync(options);

            // Zaloguj siê anonimowo jeœli jeszcze nie zalogowany
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            // Zapisz ID gracza
            playerId = AuthenticationService.Instance.PlayerId;
            Debug.Log($"Gracz zalogowany z ID: {playerId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"B³¹d inicjalizacji Unity Services: {e.Message}");
        }
    }

    public void StartPresenceTracking()
    {
        if (presenceCoroutine != null)
        {
            StopCoroutine(presenceCoroutine);
        }
        presenceCoroutine = StartCoroutine(PresenceTrackingCoroutine());
    }

    private IEnumerator PresenceTrackingCoroutine()
    {
        // SprawdŸ czy us³ugi Unity s¹ gotowe
        bool servicesReady = false;

        while (!servicesReady)
        {
            try
            {
                // SprawdŸ, czy AuthenticationService jest dostêpny i zalogowany
                servicesReady = AuthenticationService.Instance != null && AuthenticationService.Instance.IsSignedIn;
            }
            catch (Exception)
            {
                // Jeœli Unity Services nie jest zainicjalizowane, AuthenticationService.Instance wyrzuci wyj¹tek
                servicesReady = false;
            }

            if (!servicesReady)
                yield return new WaitForSeconds(1f);
        }

        while (true)
        {
            yield return JoinOrCreatePresenceLobby();
            yield return new WaitForSeconds(joinPresenceInterval);
        }
    }

    private async Task JoinOrCreatePresenceLobby()
    {
        try
        {
            // Je¿eli jesteœmy ju¿ w lobby, wystarczy aktualizowaæ nasz¹ obecnoœæ
            if (presenceLobby != null)
            {
                try
                {
                    presenceLobby = await LobbyService.Instance.GetLobbyAsync(presenceLobby.Id);
                    return;
                }
                catch
                {
                    // Lobby ju¿ nie istnieje, bêdziemy musieli stworzyæ nowe
                    presenceLobby = null;
                    StopHeartbeat();
                }
            }

            // Próba do³¹czenia do istniej¹cego lobby obecnoœci
            QueryLobbiesOptions options = new()
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.Name,
                        op: QueryFilter.OpOptions.CONTAINS,
                        value: presenceLobbyPrefix
                    )
                },
                Count = 10
            };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            if (lobbies.Results.Count > 0)
            {
                foreach (Lobby lobby in lobbies.Results)
                {
                    if (lobby.AvailableSlots > 0)
                    {
                        try
                        {
                            // Próba do³¹czenia do pierwszego dostêpnego lobby
                            JoinLobbyByIdOptions joinOptions = new();
                            presenceLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, joinOptions);
                            lobbyId = presenceLobby.Id;
                            StartHeartbeat();
                            return;
                        }
                        catch
                        {
                            // Kontynuuj do nastêpnego lobby jeœli wyst¹pi³ b³¹d przy do³¹czaniu
                            continue;
                        }
                    }
                }
            }

            // Jeœli nie ma dostêpnych lobby lub nie uda³o siê do³¹czyæ do ¿adnego, stwórz nowe
            CreateLobbyOptions createOptions = new()
            {
                IsPrivate = false,
                Player = new Player(),
                Data = new Dictionary<string, DataObject>
                {
                    // Dodajemy znacznik, ¿e to lobby s³u¿y tylko do œledzenia obecnoœci
                    { "isPresenceLobby", new DataObject(DataObject.VisibilityOptions.Public, "true") }
                }
            };

            // Tworzymy unikalne lobby z prefiksem i losowym ID (max 100 graczy)
            string lobbyName = $"{presenceLobbyPrefix}{Guid.NewGuid().ToString().Substring(0, 8)}";
            presenceLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 100, createOptions);
            lobbyId = presenceLobby.Id;
            StartHeartbeat();

            Debug.Log($"Stworzono nowe lobby obecnoœci: {lobbyName} (ID: {lobbyId})");
        }
        catch (Exception e)
        {
            Debug.LogError($"B³¹d przy do³¹czaniu/tworzeniu lobby obecnoœci: {e.Message}");
        }
    }

    private async Task LeavePresenceLobby()
    {
        StopHeartbeat();

        if (presenceLobby != null)
        {
            try
            {
                // Jeœli jesteœmy hostem, sprawdŸ czy lobby jest puste
                if (presenceLobby.HostId == playerId)
                {
                    // Jeœli jest pusty, usuñ lobby
                    if (presenceLobby.Players.Count <= 1)
                    {
                        await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
                        Debug.Log($"Usuniêto puste lobby obecnoœci: {lobbyId}");
                    }
                    else
                    {
                        // Jeœli s¹ inni gracze, opuœæ lobby
                        await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
                        Debug.Log($"Opuszczono lobby obecnoœci jako host: {lobbyId}");
                    }
                }
                else
                {
                    // Jeœli jesteœmy klientem, po prostu opuœæ lobby
                    await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
                    Debug.Log($"Opuszczono lobby obecnoœci: {lobbyId}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"B³¹d przy opuszczaniu lobby obecnoœci: {e.Message}");
            }

            presenceLobby = null;
            lobbyId = null;
        }
    }

    private void StartHeartbeat()
    {
        StopHeartbeat();

        if (!string.IsNullOrEmpty(lobbyId))
        {
            heartbeatCoroutine = StartCoroutine(HeartbeatCoroutine());
        }
    }

    private void StopHeartbeat()
    {
        if (heartbeatCoroutine != null)
        {
            StopCoroutine(heartbeatCoroutine);
            heartbeatCoroutine = null;
        }
    }

    private IEnumerator HeartbeatCoroutine()
    {
        var waitTime = new WaitForSecondsRealtime(heartbeatInterval);

        while (presenceLobby != null && !string.IsNullOrEmpty(lobbyId))
        {
            try
            {
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            }
            catch (Exception e)
            {
                Debug.LogError($"B³¹d podczas wysy³ania heartbeat: {e.Message}");
                presenceLobby = null;
                break;
            }

            yield return waitTime;
        }

        heartbeatCoroutine = null;
    }
}