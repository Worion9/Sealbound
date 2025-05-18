using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class CardRenderer : MonoBehaviour
{
    [Header("Card Rendering Settings")]
    public bool alwaysShowAllCards;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private float rowsCardSpacing = 100f;
    [SerializeField] private float panelsCardSpacing = 130f;
    [SerializeField] private float rowSpacing = 175f;
    [SerializeField] private int maxCardsPerRow = 12;
    [SerializeField] private float cardYPosition = 0f;
    [SerializeField] private float vidmoCardTransparency = 0.75f;

    private GameObject vidmoCardInstance;
    private GameObject cardInstance;
    private bool isPrefabVidmo;

    public void RenderCard(PlayerResources playerResources, Card card, Transform container, bool isBig = false, bool isCardVidmo = false, bool isPanel = false)
    {
        if (!ValidateCardRenderingParameters(container)) return;

        isPrefabVidmo = isCardVidmo;
        cardInstance = Instantiate(cardPrefab, container.position, new Quaternion(), container);
        if (card.Name == "Bear") card.Description = $"Boost me by {3 * playerResources.bearExecutions}. The boost increases by 3 for each <b>Bear</b> you play this game.";
        else if (card.Name == "Insidious Vampire" && card.wasAbilityExecuted) card.Description = "<i>none</i>";

        ConfigureCardDisplay(playerResources, container);
        if (isBig) cardInstance.transform.localScale = new Vector3(1.25f, 1.25f, 1f);
        if (isCardVidmo) ApplyVidmoTransparency();

        ParentRowListEnum parentRowListEnum = ParentRowListEnum.none;
        if (container.GetComponent<DropZone>() != null) parentRowListEnum = container.GetComponent<DropZone>().parentRowListEnum;
        if (parentRowListEnum != ParentRowListEnum.none) card.parentRowListEnum = parentRowListEnum;
        SetCardData(playerResources, card);
        SetParentRowList(playerResources, card);

        ArrangeCards(container, isCardVidmo, isPanel);

        if (!isCardVidmo && container.TryGetComponent(out DropZone dropZone))
        {
            dropZone.AjustRowTriggers();
        }
    }

    private bool ValidateCardRenderingParameters(Transform container)
    {
        if (cardPrefab == null || container == null)
        {
            Debug.LogError("CardPrefab or Container is not assigned.");
            return false;
        }
        return true;
    }

    private void ConfigureCardDisplay(PlayerResources playerResources, Transform container)
    {
        if (cardInstance.TryGetComponent<DisplayCard>(out var displayCard))
        {
            bool isInHandOrDeck = container == playerResources.handCardContainer.transform || container == playerResources.deckDisplayCardContainer.transform;

            bool shouldFaceDown = !playerResources.isActive && !alwaysShowAllCards && isInHandOrDeck;

            displayCard.FacedDown = shouldFaceDown;
        }
    }

    private void ApplyVidmoTransparency()
    {
        foreach (var renderer in cardInstance.GetComponentsInChildren<CanvasRenderer>())
        {
            renderer.SetAlpha(vidmoCardTransparency);
        }
        vidmoCardInstance = cardInstance;
    }

    private void SetCardData(PlayerResources playerResources, Card card)
    {
        if (cardInstance.TryGetComponent<DisplayCard>(out var displayCard))
        {
            displayCard.SetCardData(playerResources, card, isPrefabVidmo);
        }
    }

    private void SetParentRowList(PlayerResources playerResources, Card card)
    {
        var player1 = playerResources.turnsLogic.player1;
        var player2 = playerResources.turnsLogic.player2;
        if (player2.isActive) (player1, player2) = (player2, player1);

        if(card.parentRowListEnum == ParentRowListEnum.Player1Row1) card.ParentRowList = player1.Row1List;
        else if(card.parentRowListEnum == ParentRowListEnum.Player1Row2) card.ParentRowList = player1.Row2List;
        else if(card.parentRowListEnum == ParentRowListEnum.Player1Row3) card.ParentRowList = player1.Row3List;
        else if(card.parentRowListEnum == ParentRowListEnum.Player2Row1) card.ParentRowList = player2.Row1List;
        else if(card.parentRowListEnum == ParentRowListEnum.Player2Row2) card.ParentRowList = player2.Row2List;
        else if(card.parentRowListEnum == ParentRowListEnum.Player2Row3) card.ParentRowList = player2.Row3List;
    }

    public void ArrangeCards(Transform container, bool isCardVidmo = false, bool isPanel = false)
    {
        //RectTransform containerRect = container.GetComponent<RectTransform>();
        //LayoutRebuilder.ForceRebuildLayoutImmediate(containerRect);
        StartCoroutine(DelayedArrange(container, isCardVidmo, isPanel));
    }

    private IEnumerator DelayedArrange(Transform container, bool isCardVidmo, bool isPanel)
    {
        yield return new WaitForEndOfFrame();

        List<Transform> cards = GetCardsInContainer(container);

        if (container.GetComponent<DropZone>() != null)
        {
            container.GetComponent<DropZone>().cardCount = isCardVidmo ? cards.Count - 1 : cards.Count;
        }

        if (cards.Count == 0) yield break;

        if (cards[0].GetComponent<DisplayCard>().ThisCard.ParentRowList != null) SortCardsByParentRowList(cards);
        ArrangeCardPositions(cards, isPanel);
    }

    private List<Transform> GetCardsInContainer(Transform container)
    {
        List<Transform> cards = new();
        for (int i = 0; i < container.childCount; i++)
        {
            Transform child = container.GetChild(i);
            if (child != null && child.CompareTag("Card"))
            {
                cards.Add(child);
            }
        }
        return cards;
    }

    private void SortCardsByParentRowList(List<Transform> cards)
    {
        cards.Sort((a, b) =>
        {
            Card cardA = null;
            Card cardB = null;
            if (a.GetComponent<DisplayCard>() != null && a.GetComponent<DisplayCard>().ThisCard != null) cardA = a.GetComponent<DisplayCard>().ThisCard;
            if (b.GetComponent<DisplayCard>() != null && b.GetComponent<DisplayCard>().ThisCard != null) cardB = b.GetComponent<DisplayCard>().ThisCard;

            if (cardA == null || cardB == null)
            {
                return cardA == null ? 1 : -1;
            }

            if (cardA.ParentRowList == null || cardB.ParentRowList == null)
            {
                return 0;
            }

            return cardA.ParentRowList.IndexOf(cardA).CompareTo(cardB.ParentRowList.IndexOf(cardB));
        });

        for (int i = 0; i < cards.Count; i++)
        {
            if (vidmoCardInstance != cards[i].gameObject)
            {
                cards[i].SetSiblingIndex(i);
                cards[i].GetComponent<DisplayCard>().CardInFieldIndex = i;
            }
        }
    }

    private void ArrangeCardPositions(List<Transform> cards, bool isPanel)
    {
        float halfRowAdjustment = rowSpacing / 2;
        float cardSpacing = isPanel ? panelsCardSpacing : rowsCardSpacing;

        int cardCount = cards.Count;
        int numberOfRows = Mathf.CeilToInt((float)cardCount / maxCardsPerRow);
        float verticalAdjustment = (numberOfRows - 1) * halfRowAdjustment;

        float totalWidth = (Mathf.Min(cardCount, maxCardsPerRow) - 1) * cardSpacing;
        float startX = -(totalWidth / 2);

        if (vidmoCardInstance != null && cards.Contains(vidmoCardInstance.transform))
        {
            int newVidmoPosition = Mathf.Clamp(DragAndDropSystem.newPosition, 0, cards.Count - 1);
            cards.Remove(vidmoCardInstance.transform);
            cards.Insert(newVidmoPosition, vidmoCardInstance.transform);
        }

        for (int index = 0; index < cards.Count; index++)
        {
            Transform card = cards[index];
            int currentRow = index / maxCardsPerRow;

            float newXPosition = startX + (index % maxCardsPerRow) * cardSpacing;
            float newYPosition = cardYPosition - (currentRow * rowSpacing) + verticalAdjustment;

            card.localPosition = new Vector3(newXPosition, newYPosition, 0);

            if (vidmoCardInstance != card.gameObject)
            {
                card.GetComponent<DisplayCard>().CardInFieldIndex = index;
            }
        }
    }

    public void DestroyVidmoCard()
    {
        if (vidmoCardInstance != null) Destroy(vidmoCardInstance);
        vidmoCardInstance = null;
    }
}