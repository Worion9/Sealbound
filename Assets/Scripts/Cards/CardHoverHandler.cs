using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CardInformationPanelManager infoPanelManager;
    [SerializeField] private Card thisCard;
    [SerializeField] private Image hover;

    public Color originalColor;
    [SerializeField] private Color colorWhileHovering;

    public TextMeshProUGUI deckPoints;
    [SerializeField] private Transform deckPanelContainer;

    private void Start()
    {
        thisCard = GetComponent<DisplayCard>().ThisCard;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (deckPanelContainer != null) if (transform.parent == deckPanelContainer) deckPoints.gameObject.SetActive(true);

        if (!thisCard.DisplayCard.FacedDown)
        {
            if (GetComponent<DragAndDropSystem>())
            {
                if (!GetComponent<DragAndDropSystem>().cardIsHeld) originalColor = hover.color;
            }
            else originalColor = hover.color;
            hover.color = colorWhileHovering;
            infoPanelManager.ShowCardInfo(thisCard);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (deckPanelContainer != null) deckPoints.gameObject.SetActive(false);
        hover.color = originalColor;
        infoPanelManager.HideCardInfo();
    }
}