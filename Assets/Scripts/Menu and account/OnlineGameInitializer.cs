using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineGameInitializer : NetworkBehaviour
{
    private readonly HashSet<ulong> readyClients = new();

    private int maxPlayers;
    public GameObject loadingGamePanel;

    private void Start()
    {
        maxPlayers = gameObject.GetComponent<OnlineGameStarter>().maxPlayers;
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
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Gracz {clientId} po³¹czony.");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (readyClients.Remove(clientId)) Debug.Log($"Gracz {clientId} opuœci³ grê. Gotowych graczy: {readyClients.Count}");
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
}