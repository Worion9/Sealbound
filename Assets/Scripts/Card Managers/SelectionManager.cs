using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    List<Card> optionsList = new();
    private bool copySelectedCard; // AKTUALNIE NIEU¯YWANE PRZEZ RZADN¥ KARTÊ
    private bool justToReturnSelectedOption;
    public int option = -1;

    public void SetUpSelection(PlayerResources playerResources, List<Card> optionsList, bool copySelectedCard = false, bool justToReturnSelectedOption = false)
    {
        if (!playerResources.isActive) return;

        this.optionsList = optionsList;
        this.copySelectedCard = copySelectedCard; // AKTUALNIE NIEU¯YWANE PRZEZ RZADN¥ KARTÊ
        this.justToReturnSelectedOption = justToReturnSelectedOption;
        option = -1;

        playerResources.isPlayerInSpecialSelectionFaze = true;
        playerResources.isSelectionFromGraveyard = false;
        playerResources.selectionScreenParent.SetActive(true);
        playerResources.showSelectionButton.SetActive(true);

        foreach (Transform child in playerResources.selectionCardContainer.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < optionsList.Count; i++)
        {
            Card card = optionsList[i];
            playerResources.cardRenderer.RenderCard(playerResources, card, playerResources.selectionCardContainer.transform);

            card.DisplayCard.CardInFieldIndex = i;
        }
    }

    public void SelectSelectedCard(PlayerResources playerResources, int cardInListIndex)
    {
        if (justToReturnSelectedOption)
        {
            option = cardInListIndex;
            playerResources.cardManager.cardToPlay.TriggerFirstAbility(playerResources, playerResources.cardManager.cardToPlay);
            EndSelectionFaze(playerResources);
            return;
        }
        else if (copySelectedCard)
        {
            Card selectedCard = CardDatabase.cardList[optionsList[cardInListIndex].Id].Clone();
            playerResources.DeckList.Insert(0, selectedCard);
        }
        else
        {
            Card selectedCard = optionsList[cardInListIndex];
            int cardIndex = playerResources.DeckList.IndexOf(selectedCard);

            if (cardIndex != -1) playerResources.DeckList.RemoveAt(cardIndex);
            else Debug.LogError("Karta nie zosta³a znalezniona w talii!");

            playerResources.DeckList.Insert(0, selectedCard);
        }

        playerResources.cardManager.ForceCardToBePlayed(playerResources, 0, false);
        EndSelectionFaze(playerResources);
    }

    public void EndSelectionFaze(PlayerResources playerResources)
    {
        playerResources.isPlayerInSpecialSelectionFaze = false;
        playerResources.selectionScreenParent.SetActive(false);
        playerResources.showSelectionButton.SetActive(false);

        playerResources.cardInformationPanelManager.HideCardInfo();
    }
}
