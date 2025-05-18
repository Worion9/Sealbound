using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using TMPro;
using System.Collections;
using System;
using Unity.Services.Authentication;

public class OnlinePlayerCounter : MonoBehaviour
{
    [Header("UI")]
    public GameObject counterContainer;
    public TextMeshProUGUI playerCountText;

    [Header("Settings")]
    [SerializeField] private float refreshInterval = 10f; // Jak czêsto odœwie¿aæ licznik (w sekundach)

    private int currentPlayerCount = 0;
    private Coroutine refreshCoroutine;

    private void OnEnable()
    {
        // Rozpocznij odœwie¿anie licznika gdy obiekt jest aktywny
        StartRefreshingPlayerCount();
    }

    private void OnDisable()
    {
        // Zatrzymaj odœwie¿anie gdy obiekt jest nieaktywny
        StopRefreshingPlayerCount();
    }

    private void Start()
    {
        // Uruchom licznik
        StartRefreshingPlayerCount();
    }

    public void StartRefreshingPlayerCount()
    {
        // Zatrzymaj istniej¹c¹ korutynê jeœli istnieje
        StopRefreshingPlayerCount();

        // Uruchom now¹ korutynê
        refreshCoroutine = StartCoroutine(RefreshPlayerCountCoroutine());
    }

    public void StopRefreshingPlayerCount()
    {
        if (refreshCoroutine != null)
        {
            StopCoroutine(refreshCoroutine);
            refreshCoroutine = null;
        }
    }

    private IEnumerator RefreshPlayerCountCoroutine()
    {
        // Poczekaj na inicjalizacjê Unity Services
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
            yield return GetOnlinePlayerCount();

            // Odczekaj okreœlony czas przed kolejnym odœwie¿eniem
            yield return new WaitForSeconds(refreshInterval);
        }
    }

    private async System.Threading.Tasks.Task GetOnlinePlayerCount()
    {
        try
        {
            // Pobierz listê wszystkich dostêpnych lobby
            QueryLobbiesOptions options = new()
            {
                Count = 100 // Maksymalna liczba lobby do pobrania
            };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            // Oblicz sumê graczy we wszystkich lobby
            int totalPlayers = 0;
            foreach (var lobby in lobbies.Results)
            {
                totalPlayers += lobby.Players.Count;
            }

            // Aktualizuj licznik
            UpdatePlayerCountDisplay(totalPlayers);
        }
        catch (Exception e)
        {
            Debug.LogError($"B³¹d podczas pobierania liczby graczy online: {e.Message}");

            // W przypadku b³êdu, ukryj licznik
            if (counterContainer != null) counterContainer.SetActive(false);
        }
    }

    private void UpdatePlayerCountDisplay(int count)
    {
        currentPlayerCount = count;

        // Aktualizuj tekst UI
        if (playerCountText != null)
        {
            playerCountText.text = $"Gracze online: {currentPlayerCount}";

            // Poka¿ kontener licznika
            if (counterContainer != null && !counterContainer.activeSelf) counterContainer.SetActive(true);
        }
    }
}