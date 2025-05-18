using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private CardManager cardManager;
    [SerializeField] private CardRenderer cardRenderer;
    [SerializeField] private CardInformationPanelManager cardInformationPanelManager;
    private List<Card> sortedDeck;
    private readonly DeckToCodeConverter deckToCodeConverter = new();
    public PlayerResources player1;
    public PlayerResources player2;

    [SerializeField] private TutorialLogic tutorialLogic;

    public void SelectCardsFromDeck(PlayerResources playerResources, int amount, bool toDiscard)
    {
        playerResources.actionsLeft = amount;
        playerResources.doAlternativeAction = toDiscard;
        playerResources.deckScreenText.text = "Cards Left: " + playerResources.actionsLeft;
        playerResources.isPlayerInFromDeckSelectionFaze = true;
        playerResources.isSelectionFromGraveyard = false;
        playerResources.deckScreenParent.SetActive(true);

        SetupDeckView(playerResources);
    }

    public void SetupDeckViewButton(bool ofActivePlayer)
    {
        if ((player1.isActive && ofActivePlayer) || (!player1.isActive && !ofActivePlayer))
        {
            SetupDeckView(player1);
        }
        else
        {
            SetupDeckView(player2);
        }
    }

    public void SetupDeckView(PlayerResources playerResources)
    {
        Debug.Log(playerResources.name);

        foreach (Transform child in playerResources.deckDisplayCardContainer.transform)
        {
            Destroy(child.gameObject);
        }

        List<Card> originalDeckList = new(playerResources.DeckList);
        sortedDeck = playerResources.showDeckInTrueOrder ? originalDeckList : SortDeckByPowerAndName(originalDeckList);

        for (int i = 0; i < sortedDeck.Count; i++)
        {
            Card card = sortedDeck[i];
            cardRenderer.RenderCard(playerResources, card, playerResources.deckDisplayCardContainer.transform, false, false, true);

            card.DisplayCard.CardInDeckIndex = originalDeckList.IndexOf(card);
        }

        if (playerResources.isActive) playerResources.deckScreenText.text = $"Your Deck ({playerResources.DeckList.Count})";
        else playerResources.deckScreenText.text = $"Enemy Deck ({playerResources.DeckList.Count})";
    }

    private List<Card> SortDeckByPowerAndName(List<Card> deckList)
    {
        System.Random random = new();
        return deckList
            .OrderByDescending(card => card.Power)
            .ThenBy(card => card.Name)
            .ThenBy(card => random.Next())
            .ToList();
    }

    public void SelectSelectedCard(PlayerResources playerResources, int cardInListIndex)
    {
        if (playerResources.actionsLeft <= 0) return;

        List<Card> cardList = playerResources.isSelectionFromGraveyard ? playerResources.GraveyardList : playerResources.DeckList;

        if (cardList.Count <= 0) return;

        playerResources.actionsLeft--;

        if (!playerResources.isSelectionFromGraveyard)
        {
            int cardInDeckIndex = sortedDeck[cardInListIndex].DisplayCard.CardInDeckIndex;

            if (!playerResources.doAlternativeAction)
            {
                cardManager.ForceCardToBePlayed(playerResources, cardInDeckIndex, false);
            }
            else
            {
                playerResources.graveyardManager.AddCardToGraveyard(playerResources, cardList[cardInDeckIndex]);
                playerResources.DeckList.RemoveAt(cardInDeckIndex);
            }

            playerResources.deckScreenText.text = "Cards Left: " + playerResources.actionsLeft;
            SetupDeckView(playerResources);
        }
        else
        {
            if (playerResources.graveyardManager.banishSelectedCard)
            {
                if(playerResources.GraveyardList[cardInListIndex].IsSpy)
                {
                    playerResources.actionsLeft++;
                    Debug.Log("You can not select syp!");
                    return;
                }
                else if (playerResources.graveyardManager.canSelectOnlyEntities && playerResources.GraveyardList[cardInListIndex].isSpell)
                {
                    playerResources.actionsLeft++;
                    Debug.Log("You can not select spell!");
                    return;
                }

                playerResources.cardManager.cardToPlay.Power += Mathf.RoundToInt(playerResources.GraveyardList[cardInListIndex].Power / 2f);
                playerResources.GraveyardList.RemoveAt(cardInListIndex);
            }
            else if (!playerResources.doAlternativeAction)
            {
                if (tutorialLogic.cardForcedToSelectFromGraveyardId != 0 &&
                    tutorialLogic.cardForcedToSelectFromGraveyardId != playerResources.GraveyardList[cardInListIndex].Id)
                {
                    playerResources.actionsLeft++;
                    Debug.Log("You have to select an Egg!");
                    return;
                }
                cardManager.ForceCardToBePlayed(playerResources, cardInListIndex, true);
            }
            else
            {
                List<Card> cardsToRemove = new();

                if (playerResources.graveyardManager.selectAllCopies)
                {
                    int cardId = cardList[cardInListIndex].Id;
                    foreach (Card c in cardList)
                    {
                        if (c.Id == cardId)
                        {
                            playerResources.deckManager.AddCardToDeckToRandomPosition(playerResources, c);
                            cardsToRemove.Add(c);
                        }
                    }
                }
                else
                {
                    playerResources.deckManager.AddCardToDeckToRandomPosition(playerResources, cardList[cardInListIndex]);
                    cardsToRemove.Add(cardList[cardInListIndex]);
                }

                foreach (Card c in cardsToRemove)
                {
                    playerResources.GraveyardList.Remove(c);
                }
            }

            playerResources.graveyardScreenText.text = "Cards Left: " + playerResources.actionsLeft;
            playerResources.graveyardManager.SetupGraveyardView(playerResources);
        }

        playerResources.boardManager.UpdateDeckAndGraveyardImages(playerResources);

        if (playerResources.actionsLeft <= 0)
        {
            EndSelectionFaze(playerResources);
        }
    }

    public void EndSelectionFazeButtonHandler()
    {
        if (player1.isPlayerInFromDeckSelectionFaze) EndSelectionFaze(player1);
        if (player2.isPlayerInFromDeckSelectionFaze) EndSelectionFaze(player2);
    }

    private void EndSelectionFaze(PlayerResources playerResources)
    {
        playerResources.isPlayerInFromDeckSelectionFaze = false;
        playerResources.deckScreenParent.SetActive(false);
        playerResources.graveyardScreenParent.SetActive(false);

        playerResources.doAlternativeAction = false;
        playerResources.graveyardManager.banishSelectedCard = false;
        playerResources.graveyardManager.canSelectOnlyEntities = false;

        playerResources.deckScreenText.text = $"Your Deck ({playerResources.DeckList.Count})";
        playerResources.graveyardScreenText.text = $"Your Graveyard ({playerResources.GraveyardList.Count})";

        cardInformationPanelManager.HideCardInfo();
    }

    public void FillDeckWithRandomCards(PlayerResources playerResources)
    {
        System.Random rng = new();
        if (playerResources.randomCardMinIndex < 0) playerResources.randomCardMaxIndex = 0;
        if (playerResources.randomCardMaxIndex > CardDatabase.cardList.Count) playerResources.randomCardMaxIndex = CardDatabase.cardList.Count - 1;

        for (int i = 0; i < playerResources.startingDeckSize; i++)
        {
            // Nie pozwalamy na wylosowanie karty o indexie 0 - Statue
            int randomIndex = rng.Next(playerResources.randomCardMinIndex, playerResources.randomCardMaxIndex + 1);
            Card newCard = CardDatabase.cardList[randomIndex].Clone();
            playerResources.DeckList.Add(newCard);
        }

        ShuffleDeck(playerResources);
    }

    public void FillDeckWithCardsFromCode(string deckCode, PlayerResources playerResources)
    {
        playerResources.DeckList = deckToCodeConverter.ConvertStringToDeck(deckCode);

        ShuffleDeck(playerResources);
    }

    public void ShuffleDeck(PlayerResources playerResources)
    {
        if (playerResources == null) return;
        if (playerResources.DeckList == null) return;
        if (playerResources.DeckList.Count < 1) return;

        // ZnajdŸ liczbê powtórzeñ najczêstszej karty
        playerResources.mostCommonCardInDeckCount = playerResources.DeckList
            .GroupBy(card => card.Id)               // Grupuj po unikalnym ID karty
            .Max(group => group.Count());           // ZnajdŸ najwiêksz¹ grupê i jej rozmiar

        // Tasowanie talii
        System.Random rng = new();
        int n = playerResources.DeckList.Count;

        for (int i = n - 1; i > 0; i--)
        {
            int randomIndex = rng.Next(i + 1);

            (playerResources.DeckList[randomIndex], playerResources.DeckList[i]) = (playerResources.DeckList[i], playerResources.DeckList[randomIndex]);
        }

        playerResources.startingDeckSize = playerResources.DeckList.Count;
    }

    public void AddCardToDeckToRandomPosition(PlayerResources playerResources, Card card)
    {
        int randomIndex = new System.Random().Next(0, playerResources.DeckList.Count + 1);
        playerResources.DeckList.Insert(randomIndex, card);

        if (!playerResources.deckImage.activeSelf)
        {
            playerResources.deckImage.SetActive(true);
        }
    }

    public void AddCardToTheEndOfDeck(PlayerResources playerResources, Card card)
    {
        playerResources.DeckList.Add(card);

        if (!playerResources.deckImage.activeSelf)
        {
            playerResources.deckImage.SetActive(true);
        }
    }

    readonly Dictionary<int, int[]> playerDecks = new()
        {
            { 1, new int[] { 24, 124, 25, 124, 104, 24, 27, 36, 104, 31, 25, 24, 124, 25, 124, 104, 24, 27, 36, 104 } },
            { 2, new int[] { 24, 95, 106, 173, 138, 85, 124, 39, 44, 103, 126, 116, 128, 45, 89, 103, 24, 95, 106, 173, 138, 39, 124, 85 } },
        };

    readonly Dictionary<int, int[]> enemyDecks = new()
        {
            { 1, new int[] { 124, 124, 124, 124, 124, 124, 124, 124, 124, 124, 124, 124, 124, 124, 124, 124, 124, 124, 124, 124 } },
            { 2, new int[] { 106, 106, 104, 124, 24, 106, 104, 106, 24, 106, 124, 106, 106, 104, 124, 24, 106, 104, 106, 104, 106, 124, 106, 106, 104 } },
            { 3, new int[] { 25, 106, 104, 104, 104, 104, 24, 24, 24, 24, 25, 104, 26, 26, 25, 24, 25, 26, 110, 26, 110, 26, 110, 26, 110, 26, 110, 26, 110, 26} }
        };

    public void SetUpDeckForTutorial(PlayerResources playerResources, int tutorialStage)
    {
        playerResources.DeckList.Clear();

        if(playerResources == player1)
        {
            if (playerDecks.TryGetValue(tutorialStage, out int[] playerCards))
            {
                foreach (int cardId in playerCards)
                {
                    AddCardToTheEndOfDeck(playerResources, CardDatabase.cardList[cardId].Clone());
                }
                playerResources.mostCommonCardInDeckCount = 3;
            }
        }

        else if (enemyDecks.TryGetValue(tutorialStage, out int[] enemyCards))
        {
            foreach (int cardId in enemyCards)
            {
                AddCardToTheEndOfDeck(playerResources, CardDatabase.cardList[cardId].Clone());
            }
        }

        playerResources.startingDeckSize = playerResources.DeckList.Count;
    }
}