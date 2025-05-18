using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCard : MonoBehaviour, IPointerClickHandler
{
    [field: SerializeField] public Card ThisCard { get; set; }
    public PlayerResources playerResources;

    [field: SerializeField] public int CardInFieldIndex { get; set; }
    [field: SerializeField] public int CardInDeckIndex { get; set; }

    public bool facedDown;
    public bool FacedDown
    {
        get => facedDown;
        set
        {
            facedDown = value;
            UpdateCardUI();
        }
    }

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI influenceText;
    [SerializeField] private TextMeshProUGUI counterText;
    public TextMeshProUGUI equalizationPointsText;

    [SerializeField] private Image cardBack;
    [SerializeField] private Image armorBackground;
    [SerializeField] private Image chargesBackground;
    [SerializeField] private Image counterBackground;
    public GameObject equalizationPointsGameObject;

    [SerializeField] private Color baseFontColor;
    [SerializeField] private Color fontColorWhenBoosted;
    [SerializeField] private Color fontColorWhenWound;

    public bool isVidmo;
    [SerializeField] private Color blockedCardColor;

    [SerializeField] private bool isItDeckColection;

    public void SetCardData(PlayerResources playerResources, Card cardToDisplay, bool isVidmo)
    {
        this.isVidmo = isVidmo;
        ThisCard = cardToDisplay;
        if (playerResources != null)
        {
            this.playerResources = playerResources;
        }
        if (!isVidmo) cardToDisplay.DisplayCard = this;
        UpdateBlockedState(cardToDisplay.AreAbilitiesBlocked);
        UpdateCardUI();
    }

    public void UpdateCardUI()
    {
        image.sprite = Resources.Load<Sprite>(ThisCard.ImageName);
        cardBack.gameObject.SetActive(FacedDown);
        UpdateBackgroundVisibility();
        UpdateTextUI();
        SetTextColor();
        if(playerResources != null) playerResources.boardManager.RefreshAllCounters(playerResources);
    }

    private void UpdateBackgroundVisibility()
    {
        cardBack.gameObject.SetActive(FacedDown);
        armorBackground.gameObject.SetActive(ThisCard.Armor > 0);
        chargesBackground.gameObject.SetActive(ThisCard.Charges > 0);
        if (!isItDeckColection) counterBackground.gameObject.SetActive(ThisCard.Counter > 0);
        else counterBackground.gameObject.SetActive(ThisCard.Counter > 1);
    }

    private void UpdateTextUI()
    {
        if (ThisCard.isSpell) powerText.text = "x";
        else powerText.text = ThisCard.Power.ToString();
        armorText.text = ThisCard.Armor.ToString();
        influenceText.text = ThisCard.Charges.ToString();
        counterText.text = ThisCard.Counter.ToString();
    }

    private void SetTextColor()
    {
        if (ThisCard.Power == ThisCard.BasePower) powerText.color = baseFontColor;
        else if (ThisCard.Power > ThisCard.BasePower) powerText.color = fontColorWhenBoosted;
        else powerText.color = fontColorWhenWound;
    }

    public void UpdateBlockedState(bool isBlocked)
    {
        if (image == null)
        {
            Debug.LogWarning("Card UI Element is not assigned.");
            return;
        }

        image.color = isBlocked ? blockedCardColor : Color.white;
    }

    public void OnPointerClick(PointerEventData eventData) ///////////////////// Potencjalnie przenieœæ
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            Debug.Log("ThisCard.Name: " + ThisCard.Name);
            Debug.Log("playerResources: " + playerResources);
            Debug.Log("ThisCard.IsSpy: " + ThisCard.IsSpy);
            Debug.Log("playerResources.cardManager.executeCardAbility: " + playerResources.cardManager.executeCardAbility);
            Debug.Log("playerResources.cardManager.shouldRestoreExecuteCardAbility: " + playerResources.cardManager.shouldRestoreExecuteCardAbility);
            Debug.Log("CardInFieldIndex: " + CardInFieldIndex);
            return;
        }
        
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log(PolishAbilities.abilities.Count);
            return;
        }

        if (playerResources == null || CardInFieldIndex <= -1) return;

        if (playerResources.isPlayerInMulliganFaze)
        {
            playerResources.mulliganManager.MulliganSelectedCard(playerResources, CardInFieldIndex);
        }

        else if (playerResources.isPlayerInFromDeckSelectionFaze)
        {
            playerResources.deckManager.SelectSelectedCard(playerResources, CardInFieldIndex);
        }

        else if (playerResources.isPlayerInSpecialSelectionFaze)
        {
            playerResources.selectionManager.SelectSelectedCard(playerResources, CardInFieldIndex);
        }

        else if(playerResources.boardManager.CardThatCausedSelection != null && (playerResources.isPlayerInFromBoardSelectionFaze || playerResources.myEnemy.isPlayerInFromBoardSelectionFaze))
        {
            playerResources.boardManager.SelectCardFromBoard(playerResources, CardInFieldIndex, transform.parent);
        }

        else if(ThisCard.Charges > 0 && playerResources.isActive && !ThisCard.IsSpy) // to do: sprawdzanie czy jest tura tego gracza (nie mo¿na z tego playerResources)
        {
            playerResources.boardManager.SetUpFromBardSelection(playerResources, ThisCard);
        }

        else if (ThisCard.Charges > 0 && !playerResources.isActive && ThisCard.IsSpy) // to do: sprawdzanie czy jest tura tego gracza (nie mo¿na z tego playerResources)
        {
            playerResources.boardManager.SetUpFromBardSelection(playerResources.myEnemy, ThisCard);
        }
    }
}