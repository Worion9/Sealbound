using UnityEngine;
using TMPro;

public class CardInformationPanelManager : MonoBehaviour
{
    [SerializeField] private bool itItDeckColection;
    [SerializeField] private CardRenderer cardRenderer;
    [SerializeField] private CardCollection cardCollection;

    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject infoPanelCardSlot;
    [SerializeField] private TextMeshProUGUI cardTitleText;
    [SerializeField] private TextMeshProUGUI cardDescriptionText;

    public void ShowCardInfo(Card card)
    {
        RemovePreviousCard();

        // Utw�rz kopi� karty zamiast u�ywa� orygina�u
        Card cardCopy = CardDatabase.cardList[card.Id].Clone();
        cardCopy.BasePower = card.BasePower;
        cardCopy.power = card.BasePower;
        cardCopy.Description = card.Description;

        // Renderuj kopi� karty na infoPanel
        if(!itItDeckColection) cardRenderer.RenderCard(card.DisplayCard.playerResources, cardCopy, infoPanelCardSlot.transform, true);
        else cardCollection.RenderCard(cardCopy, infoPanelCardSlot.transform, true);

        cardTitleText.text = cardCopy.Name;
        cardDescriptionText.text = cardCopy.Description;
        infoPanel.SetActive(true);
    }

    public void HideCardInfo()
    {
        infoPanel.SetActive(false);
    }

    private void RemovePreviousCard()
    {
        foreach (Transform child in infoPanelCardSlot.transform)
        {
            Destroy(child.gameObject);
        }
    }
}