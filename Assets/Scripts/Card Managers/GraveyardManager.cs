using System.Collections.Generic;
using UnityEngine;

public class GraveyardManager : MonoBehaviour
{
    [SerializeField] private CardRenderer cardRenderer;
    public bool selectAllCopies;
    [SerializeField] private PlayerResources player1;
    [SerializeField] private PlayerResources player2;

    public void SetupGraveyardViewButton(bool ofActivePlayer)
    {
        if ((player1.isActive && ofActivePlayer) || (!player1.isActive && !ofActivePlayer)) SetupGraveyardView(player1);
        else SetupGraveyardView(player2);
    }

    public void SetupGraveyardView(PlayerResources playerResources)
    {
        foreach (Transform child in playerResources.graveyardCardContainer.transform)
        {
            Destroy(child.gameObject);
        }

        List<Card> graveyardList = new(playerResources.GraveyardList);

        for (int i = 0; i < graveyardList.Count; i++)
        {
            Card card = graveyardList[i];
            cardRenderer.RenderCard(playerResources, card, playerResources.graveyardCardContainer.transform, false, false, true);
        }

        if(playerResources.isActive) playerResources.graveyardScreenText.text = $"Your Graveyard ({playerResources.GraveyardList.Count})";
        else playerResources.graveyardScreenText.text = $"Enemy Graveyard ({playerResources.GraveyardList.Count})";
    }

    public bool banishSelectedCard = false;
    public bool canSelectOnlyEntities = false;

    public void SelectCardsFromGraveyard(PlayerResources playerResources, int amount, bool toDeck, bool allCopies, bool toBanish = false, bool onlyEntities = false)
    {
        if (!playerResources.isActive) return;

        selectAllCopies = allCopies;
        playerResources.actionsLeft = amount;
        playerResources.doAlternativeAction = toDeck;
        playerResources.graveyardScreenText.text = "Cards Left: " + playerResources.actionsLeft;
        playerResources.isPlayerInFromDeckSelectionFaze = true;
        playerResources.isSelectionFromGraveyard = true;
        playerResources.graveyardScreenParent.SetActive(true);
        banishSelectedCard = toBanish;
        canSelectOnlyEntities = onlyEntities;

        SetupGraveyardView(playerResources);
    }

    public void AddCardToGraveyard(PlayerResources playerResources, Card card)
    {
        playerResources.GraveyardList.Add(card);
        playerResources.graveyardImage.SetActive(playerResources.GraveyardList.Count > 0);
    }
}