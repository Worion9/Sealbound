using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class OnlineGameManager : NetworkBehaviour
{
    public static OnlineGameManager Instance;
    public static bool isGameOnline;

    private readonly List<PlayerNetworkResources> registeredPlayers = new();

    [SerializeField] private PlayerResources player1InitalDataReference;
    [SerializeField] private PlayerResources player2InitalDataReference;
    [SerializeField] private TurnsLogic turnsLogic;

    public NetworkVariable<bool> Player1Ready = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> Player2Ready = new(writePerm: NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerReadyStatus(int playerId, bool isReady)
    {
        if (playerId == 1) Player1Ready.Value = isReady;
        else if (playerId == 2) Player2Ready.Value = isReady;
    }

    public void RegisterPlayerNetworkResources(NetworkObjectReference playerRef)
    {
        if (!IsServer) return;

        if (!playerRef.TryGet(out NetworkObject netObj))
        {
            Debug.LogWarning("[RegisterPlayerNetworkResources] Failed to resolve NetworkObjectReference!");
            return;
        }

        var playerNetworkResources = netObj.GetComponent<PlayerNetworkResources>();

        if (registeredPlayers.Contains(playerNetworkResources))
        {
            Debug.Log("[RegisterPlayerNetworkResources] Player already registered.");
            return;
        }

        registeredPlayers.Add(playerNetworkResources);
        Debug.Log($"[RegisterPlayerNetworkResources] Player registered successfully. Count: {registeredPlayers.Count}");

        if (registeredPlayers.Count == 2)
        {
            var player1NetObj = registeredPlayers[0].GetComponent<NetworkObject>();
            var player2NetObj = registeredPlayers[1].GetComponent<NetworkObject>();
            AssignPlayerReferencesClientRpc(player1NetObj, player2NetObj);
        }
    }

    [ClientRpc]
    private void AssignPlayerReferencesClientRpc(NetworkObjectReference player1Ref, NetworkObjectReference player2Ref)
    {
        if (player1InitalDataReference == null || player2InitalDataReference == null)
        {
            FindPlayers();
        }

        if (!player1Ref.TryGet(out var player1Obj) || !player2Ref.TryGet(out var player2Obj))
        {
            Debug.LogError("Nie uda³o siê rozwi¹zaæ NetworkObjectReference.");
            return;
        }

        var player1Res = player1Obj.GetComponent<PlayerNetworkResources>();
        var player2Res = player2Obj.GetComponent<PlayerNetworkResources>();

        player1InitalDataReference.networkResources = player1Res;
        player2InitalDataReference.networkResources = player2Res;

        if (IsServer) player1Res.InitializeFromServerRpc(player1InitalDataReference.ToPlayerResourcesState());
        if (IsServer) player2Res.InitializeFromServerRpc(player2InitalDataReference.ToPlayerResourcesState());

        StartCoroutine(DelayedStartGame());
    }

    private IEnumerator DelayedStartGame()
    {
        yield return new WaitForSeconds(1f);
        FindFirstObjectByType<TurnsLogic>().StartGame();
    }

    private void FindPlayers()
    {
        player1InitalDataReference = GameObject.FindGameObjectWithTag("Player1Data").GetComponent<PlayerResources>();
        player2InitalDataReference = GameObject.FindGameObjectWithTag("Player2Data").GetComponent<PlayerResources>();
    }

    public bool IsPlayerReady(int playerId)
    {
        return playerId switch
        {
            1 => Player1Ready.Value,
            2 => Player2Ready.Value,
            _ => false
        };
    }

    public bool AreBothPlayersReady()
    {
        return IsPlayerReady(1) && IsPlayerReady(2);
    }
}