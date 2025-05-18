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
    [SerializeField] private float heartbeatInterval = 15f; // Odst�p czasu mi�dzy heartbeatami (w sekundach)
    [SerializeField] private float joinPresenceInterval = 60f; // Jak cz�sto od�wie�a� obecno�� gracza (w sekundach)

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

            // Zaloguj si� anonimowo je�li jeszcze nie zalogowany
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
            Debug.LogError($"B��d inicjalizacji Unity Services: {e.Message}");
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
        // Sprawd� czy us�ugi Unity s� gotowe
        bool servicesReady = false;

        while (!servicesReady)
        {
            try
            {
                // Sprawd�, czy AuthenticationService jest dost�pny i zalogowany
                servicesReady = AuthenticationService.Instance != null && AuthenticationService.Instance.IsSignedIn;
            }
            catch (Exception)
            {
                // Je�li Unity Services nie jest zainicjalizowane, AuthenticationService.Instance wyrzuci wyj�tek
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
            // Je�eli jeste�my ju� w lobby, wystarczy aktualizowa� nasz� obecno��
            if (presenceLobby != null)
            {
                try
                {
                    presenceLobby = await LobbyService.Instance.GetLobbyAsync(presenceLobby.Id);
                    return;
                }
                catch
                {
                    // Lobby ju� nie istnieje, b�dziemy musieli stworzy� nowe
                    presenceLobby = null;
                    StopHeartbeat();
                }
            }

            // Pr�ba do��czenia do istniej�cego lobby obecno�ci
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
                            // Pr�ba do��czenia do pierwszego dost�pnego lobby
                            JoinLobbyByIdOptions joinOptions = new();
                            presenceLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, joinOptions);
                            lobbyId = presenceLobby.Id;
                            StartHeartbeat();
                            return;
                        }
                        catch
                        {
                            // Kontynuuj do nast�pnego lobby je�li wyst�pi� b��d przy do��czaniu
                            continue;
                        }
                    }
                }
            }

            // Je�li nie ma dost�pnych lobby lub nie uda�o si� do��czy� do �adnego, stw�rz nowe
            CreateLobbyOptions createOptions = new()
            {
                IsPrivate = false,
                Player = new Player(),
                Data = new Dictionary<string, DataObject>
                {
                    // Dodajemy znacznik, �e to lobby s�u�y tylko do �ledzenia obecno�ci
                    { "isPresenceLobby", new DataObject(DataObject.VisibilityOptions.Public, "true") }
                }
            };

            // Tworzymy unikalne lobby z prefiksem i losowym ID (max 100 graczy)
            string lobbyName = $"{presenceLobbyPrefix}{Guid.NewGuid().ToString().Substring(0, 8)}";
            presenceLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 100, createOptions);
            lobbyId = presenceLobby.Id;
            StartHeartbeat();

            Debug.Log($"Stworzono nowe lobby obecno�ci: {lobbyName} (ID: {lobbyId})");
        }
        catch (Exception e)
        {
            Debug.LogError($"B��d przy do��czaniu/tworzeniu lobby obecno�ci: {e.Message}");
        }
    }

    private async Task LeavePresenceLobby()
    {
        StopHeartbeat();

        if (presenceLobby != null)
        {
            try
            {
                // Je�li jeste�my hostem, sprawd� czy lobby jest puste
                if (presenceLobby.HostId == playerId)
                {
                    // Je�li jest pusty, usu� lobby
                    if (presenceLobby.Players.Count <= 1)
                    {
                        await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
                        Debug.Log($"Usuni�to puste lobby obecno�ci: {lobbyId}");
                    }
                    else
                    {
                        // Je�li s� inni gracze, opu�� lobby
                        await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
                        Debug.Log($"Opuszczono lobby obecno�ci jako host: {lobbyId}");
                    }
                }
                else
                {
                    // Je�li jeste�my klientem, po prostu opu�� lobby
                    await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
                    Debug.Log($"Opuszczono lobby obecno�ci: {lobbyId}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"B��d przy opuszczaniu lobby obecno�ci: {e.Message}");
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
                Debug.LogError($"B��d podczas wysy�ania heartbeat: {e.Message}");
                presenceLobby = null;
                break;
            }

            yield return waitTime;
        }

        heartbeatCoroutine = null;
    }
}