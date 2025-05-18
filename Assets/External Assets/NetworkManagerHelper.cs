using Unity.Netcode;
using UnityEngine;

public class NetworkManagerHelper : MonoBehaviour
{
    private void Awake()
    {
        if (FindObjectsByType<NetworkManager>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }
    }
}
