using System.Collections;
using UnityEngine;

public class MulliganManager : MonoBehaviour
{
    [SerializeField] private AudioManager audioMenager;
    public int startingMulliganAmount;
    [SerializeField] private TurnsLogic turnsLogic;
    [SerializeField] private CardRenderer cardRenderer;
    [SerializeField] private CardInformationPanelManager cardInformationPanelManager;
    [SerializeField] private TutorialLogic tutorialLogic;
    private readonly CardTriggers cardTriggers = new();
    private bool drawCards;
    private Card cardToBoost;
    private int boost;
    private bool playerIsBoositngCards;
    private bool armorInstead;
    private bool boostMeByItsPower;

    public void MulliganCards(PlayerResources playerResources, int amount, bool discardUnwantedCards, bool drawCards = true, Card cardToBoost = null, int boost = 0, bool boostMeByItsPower = false)
    {
        if (playerResources.HandList.Count <= 0) return;

        playerResources.actionsLeft = amount;
        playerResources.doAlternativeAction = discardUnwantedCards;
        playerResources.mulliganLeftText.text = "Mulligan Left: " + playerResources.actionsLeft;
        playerResources.isPlayerInMulliganFaze = true;

        this.drawCards = drawCards;
        this.cardToBoost = cardToBoost;
        this.boost = boost;
        this.boostMeByItsPower = boostMeByItsPower;

        playerResources.mulliganScreenParent.SetActive(true);
        playerResources.showMulliganButton.SetActive(true);

        ShowMulliganCards(playerResources);
    }

    public void BoostCardsInHand(PlayerResources playerResources, int amount, int boost, bool armorInstead = false)
    {
        playerResources.actionsLeft = amount;
        playerIsBoositngCards = true;
        this.armorInstead = armorInstead;
        playerResources.mulliganLeftText.text = "Boosts Left: " + playerResources.actionsLeft;
        playerResources.isPlayerInMulliganFaze = true;
        playerResources.mulliganScreenParent.SetActive(true);
        playerResources.showMulliganButton.SetActive(true);
        this.boost = boost;

        ShowMulliganCards(playerResources);
    }

    private void ShowMulliganCards(PlayerResources playerResources)
    {
        ClearMulliganContainer(playerResources);
        playerResources.MulliganList.Clear();

        foreach (Card card in playerResources.HandList)
        {
            Card tempCard = card.Clone();
            playerResources.MulliganList.Add(tempCard);
            cardRenderer.RenderCard(playerResources, tempCard, playerResources.mulliganCardContainer.transform, false, false, true);
        }
    }

    public void MulliganSelectedCard(PlayerResources playerResources, int cardIndex)
    {
        if (playerResources.actionsLeft > 0 && playerResources.HandList.Count > 0)
        {
            Card cardToMulligan = playerResources.HandList[cardIndex];

            if (tutorialLogic.cardForcedToMulliganId != 0 && tutorialLogic.cardForcedToMulliganId != cardToMulligan.Id) return;

            if (boostMeByItsPower && cardToMulligan.IsSpy)
            {
                Debug.Log("You can not select syp!");
                return;
            }

            playerResources.actionsLeft--;
            audioMenager.PlaySFX(AudioManager.SFX.CardMulligan);
            if (playerResources.turnsLogic.tutorialGame) tutorialLogic.PlayTutorial();

            if (playerIsBoositngCards)
            {
                if(!armorInstead) cardToMulligan.Power += boost;
                else cardToMulligan.Armor += boost;
                playerResources.mulliganLeftText.text = "Boosts Left: " + playerResources.actionsLeft;
            }
            else
            {
                if (boostMeByItsPower)
                {
                    cardToBoost.Power += Mathf.CeilToInt(cardToMulligan.Power / 2f);
                }

                AddToMulliganList(playerResources, cardToMulligan);
                RemoveCardFromHand(playerResources, cardIndex);

                if (drawCards) playerResources.cardManager.DrawCard(playerResources, false);
                cardRenderer.RenderCard(playerResources, cardToMulligan, playerResources.asideCardContainer.transform, false, false, true);

                playerResources.mulliganLeftText.text = "Mulligan Left: " + playerResources.actionsLeft;

                cardRenderer.ArrangeCards(playerResources.handCardContainer.transform);
                ShowMulliganCards(playerResources);

                cardTriggers.TriggerWhenCardIsMulligan(playerResources, cardToMulligan);

                if (cardToBoost != null)
                {
                    cardToBoost.Power += boost;
                }
            }

            if (playerResources.actionsLeft == 0)
            {
                EndMulliganFaze(playerResources);
            }
        }
    }

    public void EndMulliganFaze(PlayerResources playerResources)
    {
        if (!playerResources.isPlayerInMulliganFaze || (playerResources.playerIsForced && playerResources.actionsLeft > 0)) return;

        playerIsBoositngCards = false;
        playerResources.isPlayerInMulliganFaze = false;
        playerResources.mulliganScreenParent.SetActive(false);
        playerResources.showMulliganButton.SetActive(false);

        bool isMyTurn = (turnsLogic.isMainPlayerTurn && (playerResources == turnsLogic.player1)) ||
                        (!turnsLogic.isMainPlayerTurn && (playerResources != turnsLogic.player1));
        bool playerFinishedMillgans = !playerResources.isPlayerInMulliganFaze && !playerResources.myEnemy.isPlayerInMulliganFaze;
        playerResources.endTurnButton.SetActive(isMyTurn && playerFinishedMillgans);

        foreach (Card card in playerResources.MulliganAsideList)
        {
            if(card.HasAbility<AutoPlayMeOnMillganOrDiscardAbility>()) continue;
            if (playerResources.doAlternativeAction) playerResources.graveyardManager.AddCardToGraveyard(playerResources, card);
            else if (tutorialLogic.tutorialStage == 1 || tutorialLogic.tutorialStage == 2) playerResources.deckManager.AddCardToTheEndOfDeck(playerResources, card);
            else playerResources.deckManager.AddCardToDeckToRandomPosition(playerResources, card);
        }

        ClearAsideContainer(playerResources);
        playerResources.MulliganAsideList.Clear();
        playerResources.doAlternativeAction = false;

        cardInformationPanelManager.HideCardInfo();
        cardToBoost = null;

        AutoPlayMeFromDeckOrHandAbility.alreadyUsed = false;

        if (TurnsLogic.IsGameOnline)
        {
            playerResources.SendData();
            playerResources.myEnemy.ReceiveData();
            turnsLogic.HideMulliganInfo();
        }

        if ((TurnsLogic.IsGameOnline && turnsLogic.TurnNumber.Value == 0) || TurnsLogic.turnNumber == 0) StartCoroutine(DelayedChangeTurn(playerResources));
    }

    private IEnumerator DelayedChangeTurn(PlayerResources playerResources)
    {
        yield return new WaitForSeconds(0.1f);

        playerResources.endTurnButton.SetActive(playerResources.CardsLeftToPlayAmmount > 0 && !playerResources.isPlayerInMulliganFaze && !playerResources.myEnemy.isPlayerInMulliganFaze);

        if (!playerResources.isPlayerInMulliganFaze && !playerResources.myEnemy.isPlayerInMulliganFaze)
        {
            StartCoroutine(turnsLogic.TryChangeTurnCorutine());
        }
    }

    private void ClearMulliganContainer(PlayerResources playerResources)
    {
        foreach (Transform child in playerResources.mulliganCardContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void AddToMulliganList(PlayerResources playerResources, Card card)
    {
        playerResources.MulliganAsideList.Add(card);
    }

    private void RemoveCardFromHand(PlayerResources playerResources, int cardIndex)
    {
        playerResources.HandList.RemoveAt(cardIndex);
        Destroy(playerResources.handCardContainer.transform.GetChild(cardIndex).gameObject);
    }

    private void ClearAsideContainer(PlayerResources playerResources)
    {
        foreach (Transform child in playerResources.asideCardContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
}