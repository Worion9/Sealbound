using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public PlayerDataManager playerDataManager;
    public PlayerAccount playerAccount;

    [SerializeField] private Image playerAvatar;
    [SerializeField] private TextMeshProUGUI playerNickname;
    [SerializeField] private TextMeshProUGUI playerLevel;
    [SerializeField] private TextMeshProUGUI playerRank;
    [SerializeField] private TextMeshProUGUI packagesAmmountText;
    [SerializeField] private TextMeshProUGUI goldAmmountText;
    [SerializeField] private TextMeshProUGUI scrapsAmmountText;

    [SerializeField] private GameObject newPlayerPanel;
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private RectTransform playerInfoContainer;

    [SerializeField] private GameObject tutorial2Blocker;
    [SerializeField] private GameObject tutorial3Blocker;
    [SerializeField] private GameObject offlineBlocker;
    [SerializeField] private GameObject onlineBlocker;
    [SerializeField] private GameObject cardManagerBlocker;
    public GameObject waitingForOpponentUI;
    public GameObject loadingGamePanel;
    [SerializeField] private GameObject onlinePlayerCounterContainer;
    [SerializeField] private TextMeshProUGUI onlinePlayerCountText;

    public bool isItOnlineGame;
    private OnlinePlayerCounter playerCounter;

    private void OnEnable()
    {
        playerCounter = FindAnyObjectByType<OnlinePlayerCounter>();
        
        if (playerCounter != null)
        {
            playerCounter.StartRefreshingPlayerCount();
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;

        playerAccount = new PlayerAccount();
        playerDataManager = new PlayerDataManager(playerAccount);

        playerDataManager.LoadFromLocal();

        LoadPlayerResources();
        LoadTutorialProgress();

        if (string.IsNullOrEmpty(playerAccount.Nickname) || playerAccount.Nickname == "New Player")
        {
            newPlayerPanel.SetActive(true);
        }

        if (onlinePlayerCounterContainer != null && onlinePlayerCountText != null)
        {
            var counter = gameObject.AddComponent<OnlinePlayerCounter>();
            counter.counterContainer = onlinePlayerCounterContainer;
            counter.playerCountText = onlinePlayerCountText;
        }

        StartCoroutine(RebuildLayout());
    }

    public void LoadPlayerResources()
    {
        // Jeœli w przysz³oœci dodasz awatary jako listê, za³aduj sprite z indeksu
        if(playerNickname != null) playerNickname.text = playerAccount.Nickname;
        if (playerLevel != null) playerLevel.text = playerAccount.Level.ToString();
        if (playerRank != null) playerRank.text = playerAccount.Rank.ToString();
        if (packagesAmmountText != null) packagesAmmountText.text = playerAccount.Packages.ToString();
        if (goldAmmountText != null) goldAmmountText.text = playerAccount.Gold.ToString();
        if (scrapsAmmountText != null) scrapsAmmountText.text = playerAccount.Scraps.ToString();
    }

    public void LoadTutorialProgress()
    {
        if(tutorial2Blocker == null || tutorial3Blocker == null || offlineBlocker == null || onlineBlocker == null || cardManagerBlocker == null) return;

        tutorial2Blocker.SetActive(playerAccount.TutorialProgress < 1);
        tutorial3Blocker.SetActive(playerAccount.TutorialProgress < 2);
        offlineBlocker.SetActive(playerAccount.TutorialProgress < 3);
        onlineBlocker.SetActive(playerAccount.TutorialProgress < 3);
        cardManagerBlocker.SetActive(playerAccount.TutorialProgress < 3);
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SaveNickname()
    {
        string newNickname = nicknameInputField.text.Trim();

        if (string.IsNullOrWhiteSpace(newNickname))
        {
            Debug.LogWarning("Nickname is too short.");
        }
        else if (newNickname.Length > 16)
        {
            Debug.LogWarning("Nickname is too long.");
        }
        else
        {
            playerAccount.Nickname = newNickname;

            playerDataManager.SaveToLocal();

            newPlayerPanel.SetActive(false);
            LoadPlayerResources();
            StartCoroutine(RebuildLayout());
        }
    }

    private IEnumerator RebuildLayout()
    {
        if (playerInfoContainer == null) yield break;
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(playerInfoContainer);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartTutorial(int stage)
    {
        playerAccount.CurrentTutorialStage = stage;
        playerDataManager.SaveToLocal();

        SceneManager.LoadScene("Game");
    }

    public void ToggleOnlineGame(bool isGameOnline)
    {
        isItOnlineGame = isGameOnline;
    }

    public void CancelSearch()
    {
        _ = OnlineGameStarter.Instance.CancelSearchAsync();
    }
}