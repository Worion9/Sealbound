using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private AudioManager audioMenager;
    public int playersBaseHP;
    public int maxRowSize;
    [SerializeField] private TurnsLogic turnsLogic;
    public Slider pointsSlider;

    [SerializeField] private Color victoryColor;
    [SerializeField] private Color defeatColor;

    [SerializeField] private Image selectionLine;
    [SerializeField] private int lineOffset;

    public bool canSelectYourCards;
    public bool canSelectEnemyCards;
    public bool canSelectTheSameCards;

    [SerializeField] private TextMeshProUGUI pointsToWinText;

    [SerializeField] private TutorialLogic tutorialLogic;
    [SerializeField] private ChangeSide changeSide;

    private void Start()
    {
        selectionLine.enabled = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (CardThatCausedSelection == null) return;
            if(turnsLogic.player1.isActive) StopFromBoardSelection(turnsLogic.player1);
            if(turnsLogic.player2.isActive) StopFromBoardSelection(turnsLogic.player2);
        }

        if (selectionLine.enabled)
        {
            UpdateSelectionLine(CardThatCausedSelection.DisplayCard.GetComponent<RectTransform>());
        }
    }

    public void RefreshAllCounters(PlayerResources playerResources)
    {
        if (playerResources == null)
        {
            Debug.LogError("playerResources is null in RefreshAllCounters");
            return;
        }

        if (playerResources.row1Counter == null || playerResources.row2Counter == null || playerResources.row3Counter == null)
        {
            Debug.LogError("One of the row counters is null");
            return;
        }

        if (playerResources.Row1List == null || playerResources.Row2List == null || playerResources.Row3List == null)
        {
            Debug.LogError("One of the row lists is null");
            return;
        }

        RefreshRowPointCounter(playerResources, playerResources.row1Counter, playerResources.Row1List);
        RefreshRowPointCounter(playerResources, playerResources.row2Counter, playerResources.Row2List);
        RefreshRowPointCounter(playerResources, playerResources.row3Counter, playerResources.Row3List);

        RefreshMainPointCounters(playerResources);

        if (playerResources.myEnemy == null)
        {
            Debug.LogError("playerResources.myEnemy is null in RefreshAllCounters");
            return;
        }

        RefreshMainPointCounters(playerResources.myEnemy);
    }

    public void RefreshRowPointCounter(PlayerResources playerResources, TextMeshProUGUI counter, List<Card> row)
    {
        if (playerResources == null)
        {
            Debug.LogError("playerResources is null in RefreshRowPointCounter");
            return;
        }

        if (counter == null)
        {
            Debug.LogError("counter is null in RefreshRowPointCounter");
            return;
        }

        if (row == null)
        {
            Debug.LogError("row is null in RefreshRowPointCounter");
            return;
        }

        int points = CalculateRowPoints(row);
        counter.text = points.ToString();

        if (playerResources != null)
        {
            RefreshMainPointCounters(playerResources);
        }

        if (playerResources.myEnemy != null)
        {
            RefreshMainPointCounters(playerResources.myEnemy);
        }
    }

    private int CalculateRowPoints(List<Card> row)
    {
        if (row == null)
        {
            Debug.LogError("row is null in CalculateRowPoints");
            return 0;
        }

        int points = 0;
        foreach (Card card in row)
        {
            points += card.Power;
        }
        return points;
    }

    public void RefreshMainPointCounters(PlayerResources playerResources)
    {
        if (playerResources == null)
        {
            Debug.LogError("playerResources is null in RefreshMainPointCounters");
            return;
        }

        if (playerResources.mainCounter == null)
        {
            Debug.LogError("playerResources.mainCounter is null in RefreshMainPointCounters");
            return;
        }

        if (pointsSlider == null)
        {
            Debug.LogError("pointsSlider is null in RefreshMainPointCounters");
            return;
        }

        if (pointsToWinText == null)
        {
            Debug.LogError("pointsToWinText is null in RefreshMainPointCounters");
            return;
        }

        playerResources.points = CalculateTotalPoints(playerResources.Row1List, playerResources.Row2List, playerResources.Row3List) + playerResources.equalizationPoints;

        playerResources.mainCounter.text = playerResources.points.ToString();

        pointsSlider.maxValue = playersBaseHP;
        pointsSlider.minValue = -playersBaseHP;
        pointsToWinText.text = playersBaseHP.ToString();

        if (playerResources.isActive)
        {
            pointsSlider.value = CalculatePointsDifference(playerResources);
        }
        else
        {
            pointsSlider.value = -CalculatePointsDifference(playerResources);
        }

        playerResources.HP = playersBaseHP + CalculatePointsDifference(playerResources);
        playerResources.myEnemy.HP = playersBaseHP - CalculatePointsDifference(playerResources);

        UpdateVictoryConditions(playerResources);
    }

    public int CalculatePointsDifference(PlayerResources playerResources)
    {
        return playerResources.points - playerResources.myEnemy.points;
    }

    private int CalculateTotalPoints(params List<Card>[] rows)
    {
        int totalPoints = 0;
        foreach (var row in rows)
        {
            totalPoints += CalculateRowPoints(row);
        }
        return totalPoints;
    }

    private void UpdateVictoryConditions(PlayerResources playerResources)
    {
        playerResources.pointsToLoseCounter.text = playerResources.HP.ToString();
        playerResources.myEnemy.pointsToLoseCounter.text = playerResources.myEnemy.HP.ToString();
    }

    public void CheckIfSomeoneWon(PlayerResources playerResources)
    {
        if (playerResources.HP <= 0) EndTheGame(false, playerResources);
        else if (playerResources.myEnemy.HP <= 0) EndTheGame(true, playerResources);
    }

    public void EndTheGame(bool didPlayer1Win, PlayerResources playerResources)
    {
        Time.timeScale = 0f;

        playerResources.CardsLeftToPlayAmmount = 0;
        StopFromBoardSelection(playerResources);
        playerResources.cardRenderer.alwaysShowAllCards = true;
        changeSide.DestroyAllCards();
        changeSide.RenderAllCards();
        playerResources.endTurnButton.SetActive(false);
        playerResources.deckScreenParent.SetActive(false);
        playerResources.showMulliganButton.SetActive(false);
        playerResources.showSelectionButton.SetActive(false);
        playerResources.graveyardScreenParent.SetActive(false);
        playerResources.selectionScreenParent.SetActive(false);
        playerResources.mulliganScreenParent.SetActive(false);
        playerResources.endGameScreenButton.SetActive(true);
        playerResources.endGameScreenParent.SetActive(true);

        if (didPlayer1Win)
        {
            Debug.Log("Player Wins!");
            playerResources.endGameScreenText.text = "Victory!";
            playerResources.endGameScreen.GetComponent<Image>().color = victoryColor;
            audioMenager.PlaySFX(AudioManager.SFX.GameWon);
            if (tutorialLogic.tutorialStage == 3) tutorialLogic.CompleteTutorialStage(3);
        }
        else
        {
            Debug.Log("Enemy Wins!");
            playerResources.endGameScreenText.text = "Defeat!";
            playerResources.endGameScreen.GetComponent<Image>().color = defeatColor;
            audioMenager.PlaySFX(AudioManager.SFX.GameLost);
        }
    }

    public void SpawnStatue(PlayerResources playerResources)
    {
        playerResources.cardManager.SummonCard(playerResources, CardDatabase.cardList[0].Clone(), false, null, 1, true);
    }

    public (List<Card>, Transform) GetRowByIndex(PlayerResources playerResources, int index)
    {
        return index switch
        {
            0 => (playerResources.Row1List, playerResources.row1Container.transform),
            1 => (playerResources.Row2List, playerResources.row2Container.transform),
            2 => (playerResources.Row3List, playerResources.row3Container.transform),
            _ => (null, null)
        };
    }

    public void UpdateDeckAndGraveyardImages(PlayerResources playerResources)
    {
        playerResources.deckImage.SetActive(playerResources.DeckList.Count > 0);
        playerResources.graveyardImage.SetActive(playerResources.GraveyardList.Count > 0);
    }

    public int GetRandomRow(PlayerResources playerResources)
    {
        System.Random rng = new();

        bool row1IsFull = playerResources.row1Container.GetComponent<DropZone>().cardCount >= maxRowSize;
        bool row2IsFull = playerResources.row2Container.GetComponent<DropZone>().cardCount >= maxRowSize;
        bool row3IsFull = playerResources.row3Container.GetComponent<DropZone>().cardCount >= maxRowSize;

        List<int> availableRows = new();
        if (!row1IsFull) availableRows.Add(0);
        if (!row2IsFull) availableRows.Add(1);
        if (!row3IsFull) availableRows.Add(2);

        if (availableRows.Count == 0)
        {
            Debug.Log("All rows are full.");
            return -1;
        }

        int randomIndex = rng.Next(availableRows.Count);
        return availableRows[randomIndex];
    }

    private int cardsLeftToMoveCount;
    private List<Card> cardsToMove = new();

    public void MoveCard(PlayerResources playerResources, List<Card> cardsToMove)
    {
        if(playerResources.isAI) return;
        cardsLeftToMoveCount = cardsToMove.Count;
        this.cardsToMove = cardsToMove;
    }

    public void MoveCardListHandler(PlayerResources playerResources)
    {
        if (cardsLeftToMoveCount > 0)
        {
            MoveCard(playerResources, cardsToMove[cardsLeftToMoveCount - 1]);
            cardsLeftToMoveCount--;
        }
    }

    public void MoveCard(PlayerResources playerResources, Card card)
    {
        PlayerResources cardOwner = card.DisplayCard.playerResources;

        Transform targetRow = card.DisplayCard.transform.parent;

        playerResources.DeckList.Add(card);
        CardManager.KillCard(card, false, false);

        if (cardOwner == playerResources) card.IsSpy = false;
        else card.IsSpy = true;

        playerResources.cardManager.ForceCardToBePlayed(playerResources, playerResources.DeckList.Count - 1, false, false, true, targetRow);

        playerResources.cardRenderer.ArrangeCards(targetRow);
        RefreshAllCounters(playerResources);
    }

    public void MoveCardToNextRow(PlayerResources playerResources, Card card)
    {
        bool row1IsFull = playerResources.Row1List.Count >= maxRowSize;
        bool row2IsFull = playerResources.Row2List.Count >= maxRowSize;
        bool row3IsFull = playerResources.Row3List.Count >= maxRowSize;

        Transform oldCardParent = card.DisplayCard.transform.parent;
        List<Card> previousRow = null;
        int targetRowIndex = -1;

        if (playerResources.Row1List.Contains(card))
        {
            previousRow = playerResources.Row1List;
            if (!row2IsFull) targetRowIndex = 1;
            else if (!row3IsFull) targetRowIndex = 2;
        }
        else if (playerResources.Row2List.Contains(card))
        {
            previousRow = playerResources.Row2List;
            if (!row3IsFull) targetRowIndex = 2;
            else if (!row1IsFull) targetRowIndex = 0;
        }
        else if (playerResources.Row3List.Contains(card))
        {
            previousRow = playerResources.Row3List;
            if (!row1IsFull) targetRowIndex = 0;
            else if (!row2IsFull) targetRowIndex = 1;
        }

        Destroy(card.DisplayCard.gameObject);

        playerResources.cardManager.cardIsMoving = true;
        playerResources.cardManager.SummonCard(playerResources, card, false, null, targetRowIndex, true);
        previousRow.Remove(card);

        playerResources.cardRenderer.ArrangeCards(oldCardParent);
        playerResources.cardRenderer.ArrangeCards(card.DisplayCard.transform.parent);
        playerResources.boardManager.RefreshAllCounters(playerResources);
    }

    public Card CardThatCausedSelection { private set; get; }
    private bool canSelectCardWithSameIdAsThisCard;

    public void SetUpFromBardSelection(PlayerResources playerResources, Card card, bool canSelectCardWithSameIdAsThisCard = true)
    {
        if (playerResources.isAI) return;

        playerResources.isPlayerInFromBoardSelectionFaze = true;
        this.canSelectCardWithSameIdAsThisCard = canSelectCardWithSameIdAsThisCard;
        if (card != null) CardThatCausedSelection = card;

        selectionLine.enabled = true;
        UpdateSelectionLine(CardThatCausedSelection.DisplayCard.GetComponent<RectTransform>());
    }

    public void SelectCardFromBoard(PlayerResources playerResources, int cardInRowIndex, Transform row)
    {
        //Debug.LogError("cardInRowIndex: " + cardInRowIndex);
        //Debug.LogError("playerResources: " + playerResources.name);
        //LogError("row: " + row.name);

        Card targetedCard;
        if (row == playerResources.row1Container.transform) targetedCard = playerResources.Row1List[cardInRowIndex];
        else if (row == playerResources.row2Container.transform) targetedCard = playerResources.Row2List[cardInRowIndex];
        else if (row == playerResources.row3Container.transform) targetedCard = playerResources.Row3List[cardInRowIndex];

        else
        {
            Debug.LogWarning("Row does not match any row container!");
            return;
        }

        if (targetedCard == CardThatCausedSelection)
        {
            Debug.Log("Targeted card is the same as card that caused selection!");
            return;
        }

        if (targetedCard.isInvulnerable)
        {
            Debug.Log("Targeted card is invulnerable!");
            return;
        }

        if(targetedCard.DisplayCard.playerResources == CardThatCausedSelection.DisplayCard.playerResources && !playerResources.boardManager.canSelectYourCards)
        {
            Debug.Log("You can not select your own card!");
            return;
        }

        if(targetedCard.DisplayCard.playerResources != CardThatCausedSelection.DisplayCard.playerResources && !playerResources.boardManager.canSelectEnemyCards)
        {
            Debug.Log("You can not select enemy card!");
            return;
        }

        if (!canSelectTheSameCards && StartBoardSelectionAbility.selectedCards.Contains(targetedCard))
        {
            Debug.Log("This card has already been selected!");
            return;
        }

        if (!canSelectCardWithSameIdAsThisCard && targetedCard.Id == CardThatCausedSelection.Id)
        {
            Debug.Log("Targeted card has the same id as card that caused selection!");
            return;
        }

        if (tutorialLogic.cardForcedToSelectId != 0 && tutorialLogic.cardForcedToSelectId != targetedCard.Id) return;

        audioMenager.PlaySFX(AudioManager.SFX.CardHover);

        StartBoardSelectionAbility.selectedCards.Add(targetedCard);
        if (!playerResources.isPlayerInFromBoardSelectionFaze) playerResources = playerResources.myEnemy;

        if (!playerResources.isAI && playerResources.turnsLogic.tutorialGame) tutorialLogic.PlayTutorial();

        if (CardThatCausedSelection != null && CardThatCausedSelection.Charges > 0) CardThatCausedSelection.Charges--;

        CardThatCausedSelection.TriggerFirstAbility(playerResources, targetedCard);

        if (CardThatCausedSelection.HasAbility<MoveCardAbility>()) StopFromBoardSelection(playerResources);

        if (CardThatCausedSelection.Charges <= 0) EndFromBoardSelection(playerResources);
    }

    private void StopFromBoardSelection(PlayerResources playerResources)
    {
        if (CardThatCausedSelection == null) return;

        playerResources.isPlayerInFromBoardSelectionFaze = false;
        selectionLine.enabled = false;
    }

    public void EndFromBoardSelection(PlayerResources playerResources)
    {
        if (CardThatCausedSelection == null) return;

        StartBoardSelectionAbility.selectedCards.Clear();

        if (CardThatCausedSelection.Power == 0)
        {
            CardManager.KillCard(CardThatCausedSelection);
        }

        StopFromBoardSelection(playerResources);

        CardThatCausedSelection = null;
    }

    private void UpdateSelectionLine(RectTransform cardRectTransform)
    {
        // Ustal po³o¿enie kursora w przestrzeni ekranu
        Vector2 mousePosition = Input.mousePosition;

        // Ustal po³o¿enie startowe na podstawie pozycji karty w przestrzeni œwiata
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            selectionLine.rectTransform.parent.GetComponent<RectTransform>(),
            cardRectTransform.position,
            null, // U¿yj g³ównej kamery
            out Vector2 startPoint
        );

        // Ustal po³o¿enie koñcowe (pozycja kursora)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            selectionLine.rectTransform.parent.GetComponent<RectTransform>(),
            mousePosition,
            null, // U¿yj g³ównej kamery
            out Vector2 endPoint
        );

        // Ustaw po³o¿enie linii na pocz¹tku
        selectionLine.rectTransform.anchoredPosition = startPoint;

        // Ustal d³ugoœæ i k¹t linii
        float distance = Vector2.Distance(startPoint, endPoint) - lineOffset; // Zmniejsz d³ugoœæ o 50 px
        distance = Mathf.Max(distance, 0); // Upewnij siê, ¿e d³ugoœæ nie jest ujemna
        selectionLine.rectTransform.sizeDelta = new Vector2(distance, selectionLine.rectTransform.sizeDelta.y);

        // Oblicz k¹t linii
        float angle = Vector2.SignedAngle(Vector2.right, endPoint - startPoint);
        selectionLine.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public int ReturnFirstValidRow(PlayerResources playerResources, int targetedRow)
    {
        // SprawdŸ, czy targetedRow jest poza zakresem
        if (targetedRow < 0 || targetedRow > 2) return -1;

        // Tablica z referencjami do wierszy
        var rows = new List<List<Card>> { playerResources.Row1List, playerResources.Row2List, playerResources.Row3List };

        for (int i = 0; i < 3; i++)
        {
            if (rows[targetedRow].Count < playerResources.boardManager.maxRowSize)
            {
                return targetedRow;
            }
            else
            {
                targetedRow++;
                if (targetedRow > 2) targetedRow = 0;
            }
        }

        // Jeœli ¿adna opcja nie jest dostêpna, zwróæ -1
        return -1;
    }
}