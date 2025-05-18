using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardCollection : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private MainMenu mainMenu;
    private OnlineGameStarter onlineGameStarter;

    private PlayerAccount playerAccount;
    private PlayerDataManager playerDataManager;
    public List<Card> currentDeck;
    public string currentDeckCode;
    public string currentDeckName;

    [SerializeField] private int selectedDeckIndex;
    [SerializeField] private TMP_Dropdown decksDropdown;
    [SerializeField] private TextMeshProUGUI decksDropdownLable;

    [SerializeField] private Transform collectionCardsContainer;
    [SerializeField] private RectTransform collectionCardsContainerRectTransform;
    [SerializeField] private Transform deckCardsContainer;
    [SerializeField] private RectTransform deckCardsContainerRectTransform;
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private float cardSpacing = 100f;
    [SerializeField] private float cardYPosition = 0f;
    [SerializeField] private int maxCardsPerRowInColection = 6;
    [SerializeField] private int maxCardsPerRowInDeck = 4;
    [SerializeField] private float rowSpacing = 150f;

    [SerializeField] private int maxDecksCount;
    [SerializeField] private int maxTotalCardCount;
    [SerializeField] private int minTotalCardCount;
    [SerializeField] private int maxSingleCardCount;
    [SerializeField] private int initialDeckPoints;
    [SerializeField] private int currentDeckPoints;
    [SerializeField] private TextMeshProUGUI deckPointsText;
    [SerializeField] private TextMeshProUGUI cardCountText;

    [SerializeField] private TMP_InputField deckCodeTextField;
    [SerializeField] private TMP_InputField enemyDeckCodeTextField;

    private readonly DeckToCodeConverter deckToCodeConverter = new();
    [SerializeField] private GameObject replaceDeckPanelParent;

    [SerializeField] private TMP_InputField renameDeckInputField;

    [SerializeField] private int maxDeckNameLength;
    [SerializeField] private Color invalidDeckColor;
    private string invalidColorHex;

    [SerializeField] private GameObject invalidCardPanelParent;
    [SerializeField] private TextMeshProUGUI invalidCardPanelText;

    private void Start()
    {
        InitializePlayerData();
        LoadDeck();
        SetUpDeckDropdown();
        SetupCollectionView();
        SetupDeckView();

        if (onlineGameStarter == null) onlineGameStarter = FindFirstObjectByType<OnlineGameStarter>(); 
    }

    private void InitializePlayerData()
    {
        playerAccount = new PlayerAccount();
        playerDataManager = new PlayerDataManager(playerAccount);
        playerDataManager.LoadFromLocal();
        invalidColorHex = ColorUtility.ToHtmlStringRGB(invalidDeckColor);
    }

    private void LoadDeck()
    {
        if (playerAccount.DecksList.Count > 0)
        {
            currentDeckCode = playerAccount.DecksList[selectedDeckIndex];

            while(playerAccount.DeckNamesList.Count < playerAccount.DecksList.Count)
            {
                playerAccount.DeckNamesList.Add("New Deck");
            }

            while (playerAccount.DeckNamesList.Count > playerAccount.DecksList.Count)
            {
                playerAccount.DeckNamesList.RemoveAt(playerAccount.DeckNamesList.Count - 1);
            }

            playerDataManager.SaveToLocal();
        }

        else
        {
            currentDeckCode = "";
            currentDeckName = "New Deck";
            playerAccount.DecksList.Add(currentDeckCode);
            playerAccount.DeckNamesList.Add(currentDeckName);
            playerDataManager.SaveToLocal();
        }

        currentDeckName = playerAccount.DeckNamesList[selectedDeckIndex];
        currentDeck = deckToCodeConverter.ConvertStringToDeck(currentDeckCode);
        if (deckCodeTextField != null) deckCodeTextField.text = currentDeckCode;
    }

    private void SetUpDeckDropdown()
    {
        decksDropdown.options.Clear();

        for (int i = 0; i < playerAccount.DecksList.Count; i++)
        {
            decksDropdown.options.Add(new TMP_Dropdown.OptionData(playerAccount.DeckNamesList[i]));
            if (!ChcekIfDeckIsValid(playerAccount.DecksList[i])) decksDropdown.options[i].text = $"<color=#{invalidColorHex}>{decksDropdown.options[i].text}</color>";
        }

        decksDropdownLable.text = decksDropdown.options[selectedDeckIndex].text;
        decksDropdown.value = selectedDeckIndex;

        if (deckCodeTextField != null) deckCodeTextField.text = currentDeckCode;
        if (enemyDeckCodeTextField != null) enemyDeckCodeTextField.text = playerAccount.CurrentEnemyDeck;
    }

    public void DropdownHandler(int option)
    {
        selectedDeckIndex = option;
        LoadDeck();
        SetupDeckView();
    }

    public void RenameCurrentDeckButtonHandler()
    {
        renameDeckInputField.gameObject.SetActive(true);
        renameDeckInputField.text = currentDeckName;
        renameDeckInputField.Select();
    }

    public void RenameCurrentDeck(string newName)
    {
        if (newName.Length < 1) return;
        if (newName.Length > maxDeckNameLength) newName = newName[..maxDeckNameLength];

        playerAccount.DeckNamesList[selectedDeckIndex] = newName;
        currentDeckName = newName;

        playerDataManager.SaveToLocal();
        SetUpDeckDropdown();
    }

    public void AddNewDeck()
    {
        if (playerAccount.DecksList.Count >= maxDecksCount)
        {
            Debug.Log($"You can not have more than {maxDecksCount} decks!");
            return;
        }

        playerAccount.DecksList.Add("");
        playerAccount.DeckNamesList.Add("New Deck");
        selectedDeckIndex = playerAccount.DecksList.Count - 1;
        playerDataManager.SaveToLocal();
        SetUpDeckDropdown();
        LoadDeck();
        SetupDeckView();
    }

    public void RemoveCurrentDeck()
    {
        if (playerAccount.DecksList.Count <= 1) return;
        playerAccount.DecksList.RemoveAt(selectedDeckIndex);
        playerAccount.DeckNamesList.RemoveAt(selectedDeckIndex);
        playerDataManager.SaveToLocal();
        decksDropdown.options.RemoveAt(selectedDeckIndex);
        if (selectedDeckIndex != 0) selectedDeckIndex--;
        SetUpDeckDropdown();
        LoadDeck();
        SetupDeckView();
    }

    public void DeckCodeInputHandler(string deckCode)
    {
        if (deckCode == currentDeckCode) return;

        if (deckCode == "" || !deckToCodeConverter.CheckIfDeckCodeIsValid(deckCode))
        {
            Debug.Log($"This deck code is invalid!");
            deckCodeTextField.text = currentDeckCode;
            return;
        }

        if(currentDeckCode != "")
        {
            replaceDeckPanelParent.SetActive(true);
            return;
        }

        playerAccount.DecksList[selectedDeckIndex] = deckCode;
        SetUpDeckDropdown();
        LoadDeck();
        SetupDeckView();
        playerDataManager.SaveToLocal();

        audioManager.PlaySFX(AudioManager.SFX.CardPlay);
    }

    public void ReplaceDeckYesButtonHandler()
    {
        playerAccount.DecksList[selectedDeckIndex] = deckCodeTextField.text;
        SetUpDeckDropdown();
        LoadDeck();
        SetupDeckView();
        playerDataManager.SaveToLocal();

        audioManager.PlaySFX(AudioManager.SFX.CardPlay);
    }

    public void ReplaceDeckNoButtonHandler()
    {
         deckCodeTextField.text = playerAccount.DecksList[selectedDeckIndex];
    }

    public void AddCardToDeck(int cardId)
    {
        var cardToAdd = CardDatabase.cardList.FirstOrDefault(c => c.Id == cardId);
        if (cardToAdd == null) return;

        if (cardToAdd.CardRarity == Card.Rarity.None)
        {
            invalidCardPanelParent.SetActive(true);
            invalidCardPanelText.text = "This card is a minion, so it cannot be directly added to the deck!";
            return;
        }
        
        if (currentDeck.Count(c => c.Id == cardId) >= maxSingleCardCount)
        {
            invalidCardPanelParent.SetActive(true);
            invalidCardPanelText.text = $"You cannot have more than {maxSingleCardCount} copies of the same card in your deck!";
            Debug.Log($"You cannot have more than {maxSingleCardCount} copies of the same card in your deck!");
            return;
        }

        currentDeck.Add(cardToAdd);
        currentDeckCode = deckToCodeConverter.ConvertDeckToString(currentDeck);
        playerAccount.DecksList[selectedDeckIndex] = currentDeckCode;
        playerDataManager.SaveToLocal();
        SetupDeckView();
        SetUpDeckDropdown();

        audioManager.PlaySFX(AudioManager.SFX.CardAddToDeck);
    }

    public void RemoveCardFromDeck(int cardId)
    {
        var cardToRemove = currentDeck.FirstOrDefault(c => c.Id == cardId);
        if (cardToRemove == null) return;
        currentDeck.Remove(cardToRemove);
        currentDeckCode = deckToCodeConverter.ConvertDeckToString(currentDeck);
        playerAccount.DecksList[selectedDeckIndex] = currentDeckCode;
        playerDataManager.SaveToLocal();
        SetupDeckView();
        SetUpDeckDropdown();

        audioManager.PlaySFX(AudioManager.SFX.CardRemoveFromDeck);
    }

    public void ExitColection(string sceneName)
    {
        playerDataManager.SaveToLocal();
        SceneManager.LoadScene(sceneName);
    }

    public void StartGame()
    {
        if(!CheckIfDeckIsValid(currentDeck))
        {
            Debug.Log("Selected deck is invalid!");
            return;
        }

        string enemyDeckCode = enemyDeckCodeTextField.text;

        if (!deckToCodeConverter.CheckIfDeckCodeIsValid(enemyDeckCode))
        {
            Debug.Log("Enemy deck code is invalid!");
            //return;
        }

        if (!CheckIfDeckIsValid(deckToCodeConverter.ConvertStringToDeck(enemyDeckCode)))
        {
            Debug.Log("Enemy deck is invalid!");
            //return;
        }

        playerAccount.CurrentDeck = currentDeckCode;
        playerAccount.CurrentEnemyDeck = enemyDeckCode;
        playerAccount.CurrentTutorialStage = 0;
        playerAccount.IsGameOnline = mainMenu.isItOnlineGame;
        playerDataManager.SaveToLocal();

        if(mainMenu.isItOnlineGame) onlineGameStarter.PlayOnlineGame();

        else SceneManager.LoadScene("Game");
    }

    private void SetupCollectionView()
    {
        if(collectionCardsContainer == null) return;

        ClearContainer(collectionCardsContainer);

        foreach (Card card in CardDatabase.cardList)
        {
            RenderCard(card, collectionCardsContainer);
        }

        int cardCount = CardDatabase.cardList.Count;
        int numberOfRows = Mathf.CeilToInt((float)cardCount / maxCardsPerRowInColection);
        float totalHeight = numberOfRows * rowSpacing;
        collectionCardsContainerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);

        float newYPosition = -(totalHeight / 2f) + cardYPosition;
        collectionCardsContainerRectTransform.localPosition = new Vector3(collectionCardsContainerRectTransform.localPosition.x, newYPosition, collectionCardsContainerRectTransform.localPosition.z);
    }

    private void SetupDeckView()
    {
        ClearContainer(deckCardsContainer);

        var cardCountDictionary = new Dictionary<int, int>();

        foreach (var card in currentDeck)
        {
            if (cardCountDictionary.ContainsKey(card.Id))
            {
                cardCountDictionary[card.Id]++;
            }
            else
            {
                cardCountDictionary[card.Id] = 1;
            }
        }

        var sortedCards = cardCountDictionary
            .Select(entry =>
            {
                var templateCard = CardDatabase.cardList.FirstOrDefault(c => c.Id == entry.Key);
                return new { Card = templateCard, Count = entry.Value };
            })
            .Where(item => item.Card != null)
            .OrderByDescending(item => item.Card.Power)
            .ThenBy(item => item.Card.Id);

        foreach (var item in sortedCards)
        {
            Card cardToRender = item.Card.Clone();
            cardToRender.Counter = item.Count;

            RenderCard(cardToRender, deckCardsContainer);
        }

        int cardCount = cardCountDictionary.Count;
        int numberOfRows = Mathf.CeilToInt((float)cardCount / maxCardsPerRowInDeck);
        float totalHeight = numberOfRows * rowSpacing;
        deckCardsContainerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);

        float newYPosition = -(totalHeight / 2f) + cardYPosition;
        deckCardsContainerRectTransform.localPosition = new Vector3(deckCardsContainerRectTransform.localPosition.x, newYPosition, deckCardsContainerRectTransform.localPosition.z);

        currentDeckPoints = CalculateDeckPoints(cardCountDictionary);

        if (cardCountText == null) return;
        cardCountText.text = $"Cards: {currentDeck.Count} / {maxTotalCardCount}";
        cardCountText.color = (currentDeck.Count <= maxTotalCardCount && currentDeck.Count >= minTotalCardCount) ? Color.white : Color.red;
        deckPointsText.text = $"Deck Points: {currentDeckPoints}";
        deckPointsText.color = currentDeckPoints >= 0 ? Color.white : Color.red;
    }

    private bool ChcekIfDeckIsValid(string deckCode)
    {
        var deck = deckToCodeConverter.ConvertStringToDeck(deckCode);
        return CheckIfDeckIsValid(deck);
    }

    private bool CheckIfDeckIsValid(List<Card> deck)
    {
        if (deck.Count < minTotalCardCount) return false;
        if (deck.Count > maxTotalCardCount) return false;

        var cardCountDictionary = new Dictionary<int, int>();

        foreach (var card in deck)
        {
            if (cardCountDictionary.ContainsKey(card.Id))
            {
                cardCountDictionary[card.Id]++;
            }
            else
            {
                cardCountDictionary[card.Id] = 1;
            }
        }

        var sortedCards = cardCountDictionary
            .Select(entry =>
            {
                var templateCard = CardDatabase.cardList.FirstOrDefault(c => c.Id == entry.Key);
                return new { Card = templateCard, Count = entry.Value };
            })
            .Where(item => item.Card != null)
            .OrderByDescending(item => item.Card.Power);

        if(CalculateDeckPoints(cardCountDictionary) < 0) return false;

        return true;
    }

    private int CalculateCardDeckPoints(Card card)
    {
        int totalPoints = 0;

        for (int i = 1; i <= card.Counter; i++) totalPoints += 2 - i;

        return totalPoints;
    }

    private int CalculateDeckPoints(Dictionary<int, int> cardCountDictionary)
    {
        int totalPoints = initialDeckPoints;

        foreach (var entry in cardCountDictionary)
        {
            int copies = entry.Value;

            for (int i = 1; i <= copies; i++) totalPoints += 2 - i;
        }

        return totalPoints;
    }

    private void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

    public void RenderCard(Card card, Transform container, bool isBig = false)
    {
        GameObject cardInstance = Instantiate(cardPrefab, container);
        if (isBig) cardInstance.transform.localScale = new Vector3(1.25f, 1.25f, 1f);

        if (cardInstance.TryGetComponent<DisplayCard>(out var displayCard))
        {
            displayCard.SetCardData(null, card, false);
            displayCard.ThisCard = card;
        }

        cardInstance.GetComponent<CardHoverHandler>().deckPoints.text = CalculateCardDeckPoints(card).ToString();
        ArrangeCard(cardInstance, container);
    }

    private void ArrangeCard(GameObject cardInstance, Transform container)
    {
        int cardIndex = container.childCount - 1;
        float newXPosition = cardIndex * cardSpacing;
        cardInstance.transform.localPosition = new Vector3(newXPosition, 0f, 0f);

        StartCoroutine(DelayedArrange(container));
    }

    private IEnumerator DelayedArrange(Transform container)
    {
        yield return new WaitForEndOfFrame();

        List<Transform> cards = GetCardsInContainer(container);
        if (cards.Count > 0)
        {
            ArrangeCardPositions(cards, container);
        }
    }

    private List<Transform> GetCardsInContainer(Transform container)
    {
        return container.Cast<Transform>().Where(child => child.CompareTag("Card")).ToList();
    }

    private void ArrangeCardPositions(List<Transform> cards, Transform container)
    {
        float halfRowAdjustment = rowSpacing / 2;
        int maxCardsPerRow = container == collectionCardsContainer ? maxCardsPerRowInColection : maxCardsPerRowInDeck;

        int cardCount = cards.Count;
        int numberOfRows = Mathf.CeilToInt((float)cardCount / maxCardsPerRow);
        float verticalAdjustment = (numberOfRows - 1) * halfRowAdjustment;

        float totalWidth = (Mathf.Min(cardCount, maxCardsPerRow) - 1) * cardSpacing;
        float startX = -(totalWidth / 2);

        for (int index = 0; index < cards.Count; index++)
        {
            Transform card = cards[index];
            int currentRow = index / maxCardsPerRow;

            float newXPosition = startX + (index % maxCardsPerRow) * cardSpacing;
            float newYPosition = 0f -(currentRow * rowSpacing) + verticalAdjustment;

            card.localPosition = new Vector3(newXPosition, newYPosition, 0);
        }
    }
}