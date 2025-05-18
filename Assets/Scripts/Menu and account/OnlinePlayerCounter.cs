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
    [SerializeField] private float refreshInterval = 10f; // Jak cz�sto od�wie�a� licznik (w sekundach)

    private int currentPlayerCount = 0;
    private Coroutine refreshCoroutine;

    private void OnEnable()
    {
        // Rozpocznij od�wie�anie licznika gdy obiekt jest aktywny
        StartRefreshingPlayerCount();
    }

    private void OnDisable()
    {
        // Zatrzymaj od�wie�anie gdy obiekt jest nieaktywny
        StopRefreshingPlayerCount();
    }

    private void Start()
    {
        // Uruchom licznik
        StartRefreshingPlayerCount();
    }

    public void StartRefreshingPlayerCount()
    {
        // Zatrzymaj istniej�c� korutyn� je�li istnieje
        StopRefreshingPlayerCount();

        // Uruchom now� korutyn�
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
        // Poczekaj na inicjalizacj� Unity Services
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
            yield return GetOnlinePlayerCount();

            // Odczekaj okre�lony czas przed kolejnym od�wie�eniem
            yield return new WaitForSeconds(refreshInterval);
        }
    }

    private async System.Threading.Tasks.Task GetOnlinePlayerCount()
    {
        try
        {
            // Pobierz list� wszystkich dost�pnych lobby
            QueryLobbiesOptions options = new()
            {
                Count = 100 // Maksymalna liczba lobby do pobrania
            };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            // Oblicz sum� graczy we wszystkich lobby
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
            Debug.LogError($"B��d podczas pobierania liczby graczy online: {e.Message}");

            // W przypadku b��du, ukryj licznik
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

            // Poka� kontener licznika
            if (counterContainer != null && !counterContainer.activeSelf) counterContainer.SetActive(true);
        }
    }
}