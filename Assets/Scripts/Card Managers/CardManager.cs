using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CardManager : MonoBehaviour
{
    [SerializeField] private AudioManager audioMenager;

    public int startingHandSize;
    [SerializeField] private CardRenderer cardRenderer;
    private readonly CardTriggers cardTriggers = new();
    public Card cardToPlay;

    public bool executeCardAbility = true;
    public bool shouldRestoreExecuteCardAbility = true;
    public bool cardIsMoving = false;
    public Transform BlockedRow { set; get; }
    public static int killedCardRowIndex;

    [SerializeField] private Transform spellParent;

    [SerializeField] private TutorialLogic tutorialLogic;

    [SerializeField] private CardAnimationsManager cardAnimationsManager;

    public void PlayCard(PlayerResources playerResources, int cardInHandIndex, Transform targetRow, List<Card> row, int smallTriggerIndex = -1)
    {
        // Sprawdzanie
        cardToPlay = playerResources.HandList[cardInHandIndex];
        if (!playerResources.isAI && tutorialLogic.cardForcedToPlayId != 0 && tutorialLogic.cardForcedToPlayId != cardToPlay.Id) return;
        if (IsPlayInvalid(playerResources, cardInHandIndex)) return;
        if (!IsPlayAllowedOnRow(playerResources, cardToPlay, targetRow)) return;
        if (targetRow == BlockedRow) return;

        // Zarz¹dzenia kart¹
        AddCardToRow(cardToPlay, playerResources, targetRow, row, smallTriggerIndex);
        playerResources.HandList.Remove(cardToPlay);
        DestroyCardFromHand(cardInHandIndex, playerResources);

        // Inne
        playerResources.CardsLeftToPlayAmmount--;
        StopForcingPlayer(playerResources);
        if(!playerResources.isAI && playerResources.turnsLogic.tutorialGame) tutorialLogic.PlayTutorial();

        BlockedRow = null;

        // Umiejêtnoœci
        if (executeCardAbility)
        {
            cardTriggers.TriggerWhenCardAppearsOnMySide(playerResources, cardToPlay.DisplayCard.playerResources.Row1List);
            cardTriggers.TriggerWhenCardAppearsOnMySide(playerResources, cardToPlay.DisplayCard.playerResources.Row2List);
            cardTriggers.TriggerWhenCardAppearsOnMySide(playerResources, cardToPlay.DisplayCard.playerResources.Row3List);

            cardTriggers.TriggerWhenCardAppearsOnOppositeSide(playerResources, cardToPlay.DisplayCard.playerResources.myEnemy.Row1List);
            cardTriggers.TriggerWhenCardAppearsOnOppositeSide(playerResources, cardToPlay.DisplayCard.playerResources.myEnemy.Row2List);
            cardTriggers.TriggerWhenCardAppearsOnOppositeSide(playerResources, cardToPlay.DisplayCard.playerResources.myEnemy.Row3List);

            cardTriggers.TriggerWhenThisCardIsPlayed(playerResources, cardToPlay);
        }
        if (shouldRestoreExecuteCardAbility) executeCardAbility = true;
        shouldRestoreExecuteCardAbility = true;

        if (cardIsMoving)
        {
            cardTriggers.TriggerWhenYourCardIsMoving(playerResources, cardToPlay.DisplayCard.playerResources.Row1List);
            cardTriggers.TriggerWhenYourCardIsMoving(playerResources, cardToPlay.DisplayCard.playerResources.Row2List);
            cardTriggers.TriggerWhenYourCardIsMoving(playerResources, cardToPlay.DisplayCard.playerResources.Row3List);

            cardTriggers.TriggerWhenEnemyCardIsMoving(playerResources, cardToPlay.DisplayCard.playerResources.myEnemy.Row1List);
            cardTriggers.TriggerWhenEnemyCardIsMoving(playerResources, cardToPlay.DisplayCard.playerResources.myEnemy.Row2List);
            cardTriggers.TriggerWhenEnemyCardIsMoving(playerResources, cardToPlay.DisplayCard.playerResources.myEnemy.Row3List);
        }
        cardIsMoving = false;

        cardTriggers.TriggerWhenCardAppearsOnMyRow(playerResources, row.ToList());

        if (row == playerResources.Row1List) cardTriggers.TriggerWhenCardAppearsOnOppositeRow(playerResources, playerResources.myEnemy.Row1List);
        else if (row == playerResources.Row2List) cardTriggers.TriggerWhenCardAppearsOnOppositeRow(playerResources, playerResources.myEnemy.Row2List);
        else if (row == playerResources.Row3List) cardTriggers.TriggerWhenCardAppearsOnOppositeRow(playerResources, playerResources.myEnemy.Row3List);
        else if (row == playerResources.myEnemy.Row1List) cardTriggers.TriggerWhenCardAppearsOnOppositeRow(playerResources, playerResources.Row1List);
        else if (row == playerResources.myEnemy.Row2List) cardTriggers.TriggerWhenCardAppearsOnOppositeRow(playerResources, playerResources.Row2List);
        else if (row == playerResources.myEnemy.Row3List) cardTriggers.TriggerWhenCardAppearsOnOppositeRow(playerResources, playerResources.Row3List);

        if(playerResources.boardManager.CardThatCausedSelection != null)
        {
            Card cardThatCausedSelection = playerResources.boardManager.CardThatCausedSelection;
            if (cardThatCausedSelection.Charges > 0) cardTriggers.TriggerWhenThisCardIsPlayed(playerResources, cardThatCausedSelection);
        }

        if(!playerResources.isAI) playerResources.boardManager.MoveCardListHandler(playerResources);

        playerResources.turnsLogic.QuickSync();
    }

    public void PlaySpell(PlayerResources playerResources, int cardInHandIndex)
    {
        // Sprawdzanie
        cardToPlay = playerResources.HandList[cardInHandIndex];
        if (!playerResources.isAI && tutorialLogic.cardForcedToPlayId != 0 && tutorialLogic.cardForcedToPlayId != cardToPlay.Id) return;

        // Zarz¹dzenia kart¹
        cardRenderer.RenderCard(playerResources, cardToPlay, spellParent);
        playerResources.HandList.Remove(cardToPlay);
        DestroyCardFromHand(cardInHandIndex, playerResources);

        // Inne
        playerResources.CardsLeftToPlayAmmount--;
        StopForcingPlayer(playerResources);
        if (!playerResources.isAI && playerResources.turnsLogic.tutorialGame) tutorialLogic.PlayTutorial();

        // Umiejêtnoœci
        if (executeCardAbility)
        {
            cardTriggers.TriggerWhenThisCardIsPlayed(playerResources, cardToPlay);
        }
        if (shouldRestoreExecuteCardAbility) executeCardAbility = true;
        shouldRestoreExecuteCardAbility = true;

        Card cardThatCausedSelection = playerResources.boardManager.CardThatCausedSelection;
        if (cardThatCausedSelection?.Charges > 0)
        {
            cardTriggers.TriggerWhenThisCardIsPlayed(playerResources, cardThatCausedSelection);
        }

        playerResources.boardManager.MoveCardListHandler(playerResources);

        audioMenager.PlaySFX(AudioManager.SFX.CardPlay);

        playerResources.turnsLogic.QuickSync();
    }

    public void SummonCard(PlayerResources playerResources, Card card, bool isFromHand, List<Card> cardList = null, int selectedRow = -1, bool useThisCard = false, int exactSpot = -1)
    {
        Card selectedCard = card;
        if (cardList != null && !useThisCard) selectedCard = GetRandomMatchingCard(cardList, card);
        if (selectedCard == null) return;
        cardToPlay = selectedCard;

        if (selectedRow == -1) selectedRow = playerResources.boardManager.GetRandomRow(playerResources);
        if (selectedRow == -1) return;

        var (row, targetRow) = playerResources.boardManager.GetRowByIndex(playerResources, selectedRow);
        AddCardToRow(selectedCard, playerResources, targetRow, row, -1, exactSpot);

        if (isFromHand) DestroyCardFromHand(cardList.IndexOf(selectedCard), playerResources);

        cardList?.Remove(selectedCard);

        if (cardIsMoving)
        {
            cardTriggers.TriggerWhenYourCardIsMoving(playerResources, cardToPlay.DisplayCard.playerResources.Row1List);
            cardTriggers.TriggerWhenYourCardIsMoving(playerResources, cardToPlay.DisplayCard.playerResources.Row2List);
            cardTriggers.TriggerWhenYourCardIsMoving(playerResources, cardToPlay.DisplayCard.playerResources.Row3List);

            cardTriggers.TriggerWhenEnemyCardIsMoving(playerResources, cardToPlay.DisplayCard.playerResources.myEnemy.Row1List);
            cardTriggers.TriggerWhenEnemyCardIsMoving(playerResources, cardToPlay.DisplayCard.playerResources.myEnemy.Row2List);
            cardTriggers.TriggerWhenEnemyCardIsMoving(playerResources, cardToPlay.DisplayCard.playerResources.myEnemy.Row3List);
        }
        cardIsMoving = false;

        cardTriggers.TriggerWhenCardAppearsOnMyRow(playerResources, row.ToList());

        cardTriggers.TriggerWhenCardAppearsOnMySide(playerResources, selectedCard.DisplayCard.playerResources.Row1List);
        cardTriggers.TriggerWhenCardAppearsOnMySide(playerResources, selectedCard.DisplayCard.playerResources.Row2List);
        cardTriggers.TriggerWhenCardAppearsOnMySide(playerResources, selectedCard.DisplayCard.playerResources.Row3List);

        cardTriggers.TriggerWhenCardAppearsOnOppositeSide(playerResources, selectedCard.DisplayCard.playerResources.myEnemy.Row1List);
        cardTriggers.TriggerWhenCardAppearsOnOppositeSide(playerResources, selectedCard.DisplayCard.playerResources.myEnemy.Row2List);
        cardTriggers.TriggerWhenCardAppearsOnOppositeSide(playerResources, selectedCard.DisplayCard.playerResources.myEnemy.Row3List);

        if (row == playerResources.Row1List) cardTriggers.TriggerWhenCardAppearsOnOppositeRow(playerResources, playerResources.myEnemy.Row1List);
        else if (row == playerResources.Row2List) cardTriggers.TriggerWhenCardAppearsOnOppositeRow(playerResources, playerResources.myEnemy.Row2List);
        else if (row == playerResources.Row3List) cardTriggers.TriggerWhenCardAppearsOnOppositeRow(playerResources, playerResources.myEnemy.Row3List);
        else if (row == playerResources.myEnemy.Row1List) cardTriggers.TriggerWhenCardAppearsOnOppositeRow(playerResources, playerResources.Row1List);
        else if (row == playerResources.myEnemy.Row2List) cardTriggers.TriggerWhenCardAppearsOnOppositeRow(playerResources, playerResources.Row2List);
        else if (row == playerResources.myEnemy.Row3List) cardTriggers.TriggerWhenCardAppearsOnOppositeRow(playerResources, playerResources.Row3List);

        cardTriggers.TriggerWhenCardIsSummond(playerResources, selectedCard);
    }

    public void ForceCardToBePlayed(PlayerResources playerResources, int cardInDeckIndex, bool fromGraveyard, bool executeCardAbility = true, bool isCardMoving = false, Transform blockedRow = null)
    {
        if (playerResources.isAI) return;

        this.executeCardAbility = executeCardAbility;
        this.shouldRestoreExecuteCardAbility = isCardMoving;
        cardIsMoving = isCardMoving;
        if (blockedRow != null) BlockedRow = blockedRow;

        playerResources.CardsLeftToPlayAmmount++;
        DrawCard(playerResources, false, cardInDeckIndex, fromGraveyard);
        HighlightCardInHand(playerResources, playerResources.HandList.Count - 1);
        StartForcingPlayer(playerResources);
    }

    public void DrawCard(PlayerResources playerResources, bool executeDrawAbility, int cardIndex = 0, bool fromGraveyard = false)
    {
        if (playerResources.HandList.Count >= playerResources.maxHandSize)
        {
            Debug.Log("Hand is full!");
            return;
        }

        var cardToDraw = GetCardFromSource(playerResources, cardIndex, fromGraveyard);
        if (cardToDraw == null) return;

        playerResources.HandList.Add(cardToDraw);

        if (fromGraveyard)
        {
            if (cardToDraw.Name != "Insidious Vampire") cardToDraw.wasAbilityExecuted = false;
            cardToDraw.power = cardToDraw.BasePower;
        }

        cardRenderer.RenderCard(playerResources, cardToDraw, playerResources.handCardContainer.transform);

        playerResources.boardManager.UpdateDeckAndGraveyardImages(playerResources);
        if (executeDrawAbility) TriggerDrawEffects(playerResources);

        if(TurnsLogic.turnNumber != 0) audioMenager.PlaySFX(AudioManager.SFX.CardDraw);
    }

    private bool IsPlayInvalid(PlayerResources playerResources, int cardIndex)
    {
        return playerResources.isPlayerInMulliganFaze || playerResources.CardsLeftToPlayAmmount <= 0 ||
               playerResources.HandList.Count <= 0 || cardIndex < 0 || cardIndex >= playerResources.HandList.Count;
    }

    private bool IsPlayAllowedOnRow(PlayerResources playerResources, Card cardToPlay, Transform targetRow)
    {
        bool isSpyMatch = cardToPlay.IsSpy != targetRow.GetComponent<DropZone>().enemyRow;
        if (playerResources.isActive && isSpyMatch || !playerResources.isActive && !isSpyMatch)
        {
            Debug.Log("This card can't be played here.");
            cardRenderer.ArrangeCards(playerResources.handCardContainer.transform);
            return false;
        }
        return true;
    }

    public int position;
    public void AddCardToRow(Card card, PlayerResources playerResources, Transform targetRow, List<Card> row, int smallTriggerIndex = -1, int exactPosition = -1)
    {
        if (exactPosition != -1) position = exactPosition;
        else if (smallTriggerIndex == -1) position = row.Count;
        else position = CalculateCardPosition(row.Count, smallTriggerIndex);

        row.Insert(position, card);
        card.ParentRowList = row;

        if (!playerResources.isAI && card.DisplayCard != null)
        {
            card.DisplayCard.playerResources = (targetRow.GetComponent<DropZone>().enemyRow && playerResources.CompareTag("Player1Data")) ||
                                               (!targetRow.GetComponent<DropZone>().enemyRow && playerResources.CompareTag("Player2Data"))
            ? playerResources.turnsLogic.player2 : playerResources.turnsLogic.player1;
        }
        else if (card.DisplayCard != null)
        {
            card.DisplayCard.playerResources = targetRow.GetComponent<DropZone>().enemyRow
            ? playerResources.turnsLogic.player2 : playerResources.turnsLogic.player1;
        }

        if (card.DisplayCard != null) playerResources = card.DisplayCard.playerResources;
        cardRenderer.RenderCard(playerResources, card, targetRow);
        playerResources.boardManager.RefreshAllCounters(playerResources);

        if (TurnsLogic.turnNumber != 0) audioMenager.PlaySFX(AudioManager.SFX.CardPlay);
    }

    public Card GetRandomMatchingCard(List<Card> cardList, Card targetCard)
    {
        List<Card> matchingCards = cardList.Where(c => c.Id == targetCard.Id).ToList();

        if (matchingCards.Count == 0)
        {
            throw new KeyNotFoundException("No matching cards found.");
        }

        int randomIndex = Random.Range(0, matchingCards.Count);
        return matchingCards[randomIndex];
    }

    private void DestroyCardFromHand(int cardIndex, PlayerResources playerResources)
    {
        if (cardIndex >= 0 && cardIndex < playerResources.handCardContainer.transform.childCount)
        {
            Destroy(playerResources.handCardContainer.transform.GetChild(cardIndex).gameObject);
            cardRenderer.ArrangeCards(playerResources.handCardContainer.transform);
        }
        else
        {
            Debug.LogWarning($"Nie mo¿na usun¹æ karty z rêki: indeks {cardIndex} jest poza zakresem.");
        }
    }

    private void HighlightCardInHand(PlayerResources playerResources, int cardIndex)
    {
        if (playerResources.isAI) return;
        var cardObj = playerResources.HandList[cardIndex].DisplayCard.GetComponent<Image>();
        cardObj.color = Color.red;
        cardObj.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
    }

    private Card GetCardFromSource(PlayerResources playerResources, int cardIndex, bool fromGraveyard)
    {
        var sourceList = fromGraveyard ? playerResources.GraveyardList : playerResources.DeckList;

        if (!sourceList.Any())
        {
            Debug.Log($"{(fromGraveyard ? "Graveyard" : "Deck")} is empty!");
            return null;
        }

        var card = sourceList[cardIndex];
        sourceList.RemoveAt(cardIndex);
        return card;
    }

    private void TriggerDrawEffects(PlayerResources playerResources)
    {
        cardTriggers.TriggerWhenPlayerDrawCard(playerResources, playerResources.Row1List);
        cardTriggers.TriggerWhenPlayerDrawCard(playerResources, playerResources.Row2List);
        cardTriggers.TriggerWhenPlayerDrawCard(playerResources, playerResources.Row3List);
    }

    public void StartForcingPlayer(PlayerResources playerResources)
    {
        if(!playerResources.isAI) playerResources.endTurnButton.SetActive(false);
        playerResources.playerIsForced = true;
    }

    public void StopForcingPlayer(PlayerResources playerResources)
    {
        if (!playerResources.isAI) playerResources.endTurnButton.SetActive(true);
        playerResources.playerIsForced = false;

        if (playerResources.boardManager.CardThatCausedSelection == null) return;
        if (!playerResources.boardManager.CardThatCausedSelection.HasAbility<MoveCardAbility>()) return;
        if (playerResources.boardManager.CardThatCausedSelection.Charges <= 0) return;
        playerResources.boardManager.SetUpFromBardSelection(playerResources, null);
    }

    public static void DealDamageToCard(Card card, int damage)
    {
        if (!card.HasAbility<DefenderAbility>() && card.ParentRowList != null)
        {
            Card defender = card.ParentRowList.FirstOrDefault(c => c.HasAbility<DefenderAbility>());
            if (defender != null)
            {
                DealDamageToCard(defender, damage);
                return;
            }
        }

        if (card.Armor > 0)
        {
            int remainingDamage = damage - card.Armor;
            card.Armor = Mathf.Max(0, card.Armor - damage);

            if (remainingDamage > 0)
            {
                card.power -= remainingDamage; // Directly access the field

                if (card.ParentRowList != null && card.DisplayCard != null)
                {
                    if (card.ParentRowList == card.DisplayCard.playerResources.Row1List)
                    {
                        card.DisplayCard.playerResources.cardTriggers.TriggerOnEnemyRowWhenCardIsDamaged(card.DisplayCard.playerResources, card.DisplayCard.playerResources.myEnemy.Row1List);
                    }
                    else if (card.ParentRowList == card.DisplayCard.playerResources.Row2List)
                    {
                        card.DisplayCard.playerResources.cardTriggers.TriggerOnEnemyRowWhenCardIsDamaged(card.DisplayCard.playerResources, card.DisplayCard.playerResources.myEnemy.Row2List);
                    }
                    else if (card.ParentRowList == card.DisplayCard.playerResources.Row3List)
                    {
                        card.DisplayCard.playerResources.cardTriggers.TriggerOnEnemyRowWhenCardIsDamaged(card.DisplayCard.playerResources, card.DisplayCard.playerResources.myEnemy.Row3List);
                    }
                }
            }
        }
        else
        {
            card.power -= damage; // Directly access the field

            if(card.ParentRowList != null && card.DisplayCard != null)
            {
                if (card.ParentRowList == card.DisplayCard.playerResources.Row1List)
                {
                    card.DisplayCard.playerResources.cardTriggers.TriggerOnEnemyRowWhenCardIsDamaged(card.DisplayCard.playerResources, card.DisplayCard.playerResources.myEnemy.Row1List);
                }
                else if (card.ParentRowList == card.DisplayCard.playerResources.Row2List)
                {
                    card.DisplayCard.playerResources.cardTriggers.TriggerOnEnemyRowWhenCardIsDamaged(card.DisplayCard.playerResources, card.DisplayCard.playerResources.myEnemy.Row2List);
                }
                else if (card.ParentRowList == card.DisplayCard.playerResources.Row3List)
                {
                    card.DisplayCard.playerResources.cardTriggers.TriggerOnEnemyRowWhenCardIsDamaged(card.DisplayCard.playerResources, card.DisplayCard.playerResources.myEnemy.Row3List);
                }
            }
            
        }

        if (card.Power <= 0) KillCard(card);
    }

    public static List<Card> rowListOfDeadCard;
    public static int rowIndexOfDeadCard;
    public static int indexOfDeadCard;

    public static void KillCard(Card cardToKill, bool addCardToGraveyard = true, bool executeRevenge = true)
    {
        if (cardToKill.DisplayCard == null) return;

        PlayerResources playerResources = cardToKill.DisplayCard.playerResources;

        if (playerResources.GraveyardList.Contains(cardToKill) || playerResources.myEnemy.GraveyardList.Contains(cardToKill)) return;
        if (cardToKill.isInvulnerable) return;

        if (cardToKill.isSpell)
        {
            if(addCardToGraveyard) playerResources.graveyardManager.AddCardToGraveyard(playerResources, cardToKill);
            Destroy(cardToKill.DisplayCard.gameObject);
            return;
        }
        
        cardToKill.Power = cardToKill.BasePower;
        cardToKill.IsSpy = CardDatabase.cardList[cardToKill.Id].IsSpy;
        cardToKill.Charges = 0;
        cardToKill.Counter = 0;
        cardToKill.Armor = 0;

        Transform targetRow = cardToKill.DisplayCard.transform.parent;
        List<Card> thisRowList = new();
        if (targetRow == playerResources.row1Container.transform)
        {
            thisRowList = playerResources.Row1List;
            killedCardRowIndex = 0;
        }
        else if (targetRow == playerResources.row2Container.transform)
        {
            thisRowList = playerResources.Row2List;
            killedCardRowIndex = 1;
        }
        else if (targetRow == playerResources.row3Container.transform)
        {
            thisRowList = playerResources.Row3List;
            killedCardRowIndex = 2;
        }

        rowListOfDeadCard = cardToKill.ParentRowList.ToList();
        rowIndexOfDeadCard = cardToKill.DisplayCard.transform.parent.GetComponent<DropZone>().associatedListIndex - 1;
        indexOfDeadCard = cardToKill.ParentRowList.IndexOf(cardToKill);

        if(addCardToGraveyard && cardToKill.BasePower > 0) playerResources.graveyardManager.AddCardToGraveyard(playerResources, cardToKill);
        thisRowList.Remove(cardToKill);
        Destroy(cardToKill.DisplayCard.gameObject);
        playerResources.cardRenderer.ArrangeCards(targetRow);
        playerResources.boardManager.RefreshAllCounters(playerResources);
        cardToKill.ParentRowList = null;

        if (executeRevenge)
        {
            playerResources.cardTriggers.TriggerWhenCardIsKilled(playerResources, cardToKill);
            playerResources.cardTriggers.TriggerWhenAnyCardOnYourSideDies(playerResources, playerResources.Row1List);
            playerResources.cardTriggers.TriggerWhenAnyCardOnYourSideDies(playerResources, playerResources.Row2List);
            playerResources.cardTriggers.TriggerWhenAnyCardOnYourSideDies(playerResources, playerResources.Row3List);
            playerResources.cardTriggers.TriggerWhenAnyCardOnEnemySideDies(playerResources.myEnemy, playerResources.myEnemy.Row1List);
            playerResources.cardTriggers.TriggerWhenAnyCardOnEnemySideDies(playerResources.myEnemy, playerResources.myEnemy.Row2List);
            playerResources.cardTriggers.TriggerWhenAnyCardOnEnemySideDies(playerResources.myEnemy, playerResources.myEnemy.Row3List);
        }

        playerResources.cardInformationPanelManager.HideCardInfo();
        playerResources.myEnemy.cardInformationPanelManager.HideCardInfo();
    }

    public static int CalculateCardPosition(int cardsInRow, int smallTriggerIndex)
    {
        if (cardsInRow == 0) return 0;
        else if (cardsInRow == 1) return smallTriggerIndex >= 5 ? 1 : 0;
        else if (cardsInRow == 2)
        {
            if (smallTriggerIndex >= 5 && smallTriggerIndex <= 6) return smallTriggerIndex - 4;
            else if (smallTriggerIndex >= 7) return 2;
            return 0;
        }
        else if (cardsInRow == 3)
        {
            if (smallTriggerIndex >= 4 && smallTriggerIndex <= 6) return smallTriggerIndex - 3;
            else if (smallTriggerIndex >= 7) return 3;
            return 0;
        }
        else if (cardsInRow == 4)
        {
            if (smallTriggerIndex >= 4 && smallTriggerIndex <= 6) return smallTriggerIndex - 3;
            else if (smallTriggerIndex >= 7 && smallTriggerIndex <= 10) return 4;
            return 0;
        }
        else if (cardsInRow == 5)
        {
            if (smallTriggerIndex >= 3 && smallTriggerIndex <= 7) return smallTriggerIndex - 2;
            else if (smallTriggerIndex >= 8) return 5;
            return 0;
        }
        else if (cardsInRow == 6)
        {
            if (smallTriggerIndex >= 3 && smallTriggerIndex <= 7) return smallTriggerIndex - 2;
            else if (smallTriggerIndex >= 8 && smallTriggerIndex <= 10) return 6;
            return 0;
        }
        else if (cardsInRow == 7)
        {
            if (smallTriggerIndex >= 2 && smallTriggerIndex <= 8) return smallTriggerIndex - 1;
            else if (smallTriggerIndex >= 9) return 7;
            return 0;
        }
        else if (cardsInRow == 8)
        {
            if (smallTriggerIndex >= 2 && smallTriggerIndex <= 9) return smallTriggerIndex - 1;
            else if (smallTriggerIndex >= 10) return 8;
            return 0;
        }
        else if (cardsInRow == 9)
        {
            if (smallTriggerIndex >= 1 && smallTriggerIndex <= 9) return smallTriggerIndex;
            return 0;
        }
        else if (cardsInRow == 10)
        {
            return smallTriggerIndex;
        }
        return -1;
    }
}