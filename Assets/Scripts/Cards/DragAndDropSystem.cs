using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class DragAndDropSystem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private AudioManager audioMenager;

    [SerializeField] private Transform dragCanvas;
    public bool isItDeckColection;
    [SerializeField] private CardRenderer cardRenderer;
    [SerializeField] private TurnsLogic turnsLogic;
    [SerializeField] private CardInformationPanelManager infoPanelManager;
    private DisplayCard displayCard;
    private RectTransform rectTransform;
    private Canvas canvas;

    [SerializeField] private Transform playerHandContainer;

    private Vector2 originalPosition;
    private Color originalColor;
    private Vector3 originalScale;
    private Transform originalParent;
    private int originalSiblingIndex;
    public bool cardIsHeld;
    private Vector2 dragOffset;
    public static int newPosition;

    private Transform currentDropZone;

    private PlayerResources playerResources;

    [SerializeField] private Color colorWhileInZone;
    [SerializeField] private Vector2 scaleWhileDraggingMultiplier;
    private Vector2 scaleWhileDragging;

    [SerializeField] private CardCollection cardCollection;

    private void Awake()
    {
        scaleWhileDragging = new Vector2(transform.localScale.x * scaleWhileDraggingMultiplier.x, transform.localScale.y * scaleWhileDraggingMultiplier.y);
        rectTransform = GetComponent<RectTransform>();
        displayCard = GetComponent<DisplayCard>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsDraggable()) PrepareForDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (cardIsHeld) MoveCard(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (cardIsHeld) HandleDrop();
    }

    private bool IsDraggable()
    {
        if (playerResources == null) playerResources = displayCard.playerResources;

        if (isItDeckColection) return true;

        bool isInHand = transform.parent == playerHandContainer;
        bool isNotInMulliganPhase = !playerResources.isPlayerInMulliganFaze;
        bool isEnemyNotInMulliganPhase = !playerResources.myEnemy.isPlayerInMulliganFaze;
        bool hasCardsLeftToPlay = playerResources.CardsLeftToPlayAmmount > 0;

        //Debug.Log("Is this card in hand: " + isInHand);
        //Debug.Log("Is not this player in mulligan faze: " + isNotInMulliganPhase);
        //Debug.Log("Is not enemy player in mulligan faze: " + isEnemyNotInMulliganPhase);
        //Debug.Log("Can you play cards: " + hasCardsLeftToPlay);

        return isInHand && isNotInMulliganPhase && isEnemyNotInMulliganPhase && hasCardsLeftToPlay;
    }

    private void PrepareForDrag(PointerEventData eventData)
    {
        originalSiblingIndex = transform.GetSiblingIndex();
        originalParent = transform.parent;
        originalColor = GetComponent<Image>().color;

        if(playerResources != null) if (playerResources.playerIsForced && originalSiblingIndex != originalParent.childCount - 1) return;

        originalScale = transform.localScale;
        originalPosition = rectTransform.anchoredPosition;
        cardIsHeld = true;

        SetParentForDragging();
        CalculateDragOffset(eventData);

        audioMenager.PlaySFX(AudioManager.SFX.CardPickUp);
    }

    private void SetParentForDragging()
    {
        Vector3 savedLocalPosition = transform.position;
        transform.SetParent(dragCanvas, true);
        transform.position = savedLocalPosition;
        transform.localScale = scaleWhileDragging;
    }

    private void CalculateDragOffset(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out var globalMousePos))
        {
            dragOffset = globalMousePos - rectTransform.anchoredPosition;
        }
    }

    private void MoveCard(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out var globalMousePos))
        {
            rectTransform.anchoredPosition = globalMousePos - dragOffset;
        }
    }

    private void HandleDrop()
    {
        cardIsHeld = false;
        transform.localScale = originalScale;
        transform.SetParent(originalParent, true);
        transform.SetSiblingIndex(originalSiblingIndex);
        if (!isItDeckColection)
        {
            cardRenderer.DestroyVidmoCard();
            if (currentDropZone != null) if (currentDropZone.GetComponent<DropZone>().myCardContainer != null)
                    cardRenderer.ArrangeCards(currentDropZone.GetComponent<DropZone>().myCardContainer.transform);
        }
        infoPanelManager.HideCardInfo();

        if (currentDropZone != null)
        {
            ProcessDrop();
        }
        else
        {
            ResetPosition();
        }

        currentDropZone = null;
    }

    private void ProcessDrop()
    {
        DropZone smallDropZone = currentDropZone.GetComponent<DropZone>();

        if (isItDeckColection)
        {
            if (smallDropZone.associatedListIndex == 1)
            {
                if (TryGetThisCard(out Card thisCard))
                {
                    cardCollection.RemoveCardFromDeck(thisCard.Id);
                }
            }
            else if (smallDropZone.associatedListIndex == 2)
            {
                if (TryGetThisCard(out Card thisCard))
                {
                    cardCollection.AddCardToDeck(thisCard.Id);
                }
            }

            ResetPosition();
        }
        else if (displayCard.ThisCard.isSpell)
        {
            ResetPosition();
            playerResources.cardManager.PlaySpell(playerResources, originalSiblingIndex);
        }
        else if (smallDropZone.myCardContainer.cardCount < playerResources.boardManager.maxRowSize)
        {
            List<Card> targetList = GetTargetList(smallDropZone);
            ResetPosition();
            playerResources.cardManager.PlayCard(playerResources, originalSiblingIndex, smallDropZone.myCardContainer.transform, targetList, smallDropZone.slotIndex);
        }
        else
        {
            ResetPosition();
        }
    }

    private bool TryGetThisCard(out Card thisCard)
    {
        thisCard = null;
        if (TryGetComponent<DisplayCard>(out var displayCard))
        {
            thisCard = displayCard.ThisCard;
            return thisCard != null;
        }
        return false;
    }

    private List<Card> GetTargetList(DropZone dropZone)
    {
        return dropZone.enemyRow
            ? GetEnemyTargetList(dropZone)
            : GetPlayerTargetList(dropZone);
    }

    private List<Card> GetPlayerTargetList(DropZone dropZone)
    {
        return dropZone.associatedListIndex switch
        {
            1 => playerResources.Row1List,
            2 => playerResources.Row2List,
            _ => playerResources.Row3List,
        };
    }

    private List<Card> GetEnemyTargetList(DropZone dropZone)
    {
        return dropZone.associatedListIndex switch
        {
            1 => playerResources.myEnemy.Row1List,
            2 => playerResources.myEnemy.Row2List,
            _ => playerResources.myEnemy.Row3List,
        };
    }

    private void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
        if (!isItDeckColection) cardRenderer.ArrangeCards(playerResources.handCardContainer.transform);
        ChangeCardColor(GetComponent<CardHoverHandler>().originalColor);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!cardIsHeld || !collider.TryGetComponent(out DropZone detectedDropZone)) return;
        if (!isItDeckColection && detectedDropZone.spellDropZone != displayCard.ThisCard.isSpell) return;

        // Jeśli karta jest już nad inną strefą, wychodzimy z poprzedniej
        if (currentDropZone != null && currentDropZone != collider.transform)
        {
            OnTriggerExitFromDropZone();
        }

        currentDropZone = collider.transform;

        if (isItDeckColection)
        {
            ChangeCardColor(colorWhileInZone);
            return;
        }

        if (displayCard.ThisCard.isSpell)
        {
            ChangeCardColor(colorWhileInZone);
            return;
        }

        if (displayCard.ThisCard.IsSpy == detectedDropZone.enemyRow &&
            playerResources.cardManager.BlockedRow != detectedDropZone.myCardContainer.transform &&
            detectedDropZone.myCardContainer.cardCount < playerResources.boardManager.maxRowSize)
        {
            ChangeCardColor(colorWhileInZone);
            newPosition = CardManager.CalculateCardPosition(detectedDropZone.myCardContainer.cardCount, detectedDropZone.slotIndex);
            cardRenderer.RenderCard(playerResources, displayCard.ThisCard, detectedDropZone.myCardContainer.transform, false, true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out DropZone _) && currentDropZone == other.transform)
        {
            OnTriggerExitFromDropZone();
        }
    }

    private void OnTriggerExitFromDropZone()
    {
        if (cardIsHeld)
        {
            ChangeCardColor(originalColor);
            if (!isItDeckColection && !displayCard.ThisCard.isSpell)
            {
                cardRenderer.DestroyVidmoCard();
                cardRenderer.ArrangeCards(currentDropZone.GetComponent<DropZone>().myCardContainer.transform);
            }
        }
        currentDropZone = null;
    }

    private void ChangeCardColor(Color color)
    {
        GetComponent<Image>().color = color;
    }
}