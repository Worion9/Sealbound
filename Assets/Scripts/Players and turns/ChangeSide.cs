using UnityEngine;

public class ChangeSide : MonoBehaviour
{
    public PlayerResources player1;
    public PlayerResources player2;
    [SerializeField] private CardRenderer cardRenderer;
    [SerializeField] private TurnsLogic turnsLogic;
    [SerializeField] private GameObject player1Panels;
    [SerializeField] private GameObject player2Panels;
    [SerializeField] private GameObject playerEndTurnButton;

    public void ChangeSideButton()
    {
        SwapDisplayVariables(player1, player2);
        DestroyAllCards();
        RenderAllCards();
        ChangeEndTurnButtonVisibility();
    }

    private void ChangeEndTurnButtonVisibility()
    {
        bool isPlayerTurn = (turnsLogic.isMainPlayerTurn && player1.isActive) || (!turnsLogic.isMainPlayerTurn && player2.isActive);
        bool isMulligan = player1.isPlayerInMulliganFaze || player2.isPlayerInMulliganFaze;

        playerEndTurnButton.SetActive(isPlayerTurn && !isMulligan);
    }

    private void SwapDisplayVariables(PlayerResources p1, PlayerResources p2)
    {
        player1Panels.SetActive(!player1Panels.activeSelf);
        player2Panels.SetActive(!player2Panels.activeSelf);

        // Zmiana zmiennych interfejsu u¿ytkownika
        (p1.isActive, p2.isActive) = (p2.isActive, p1.isActive);

        (p1.row1Container, p2.row1Container) = (p2.row1Container, p1.row1Container);
        (p1.row2Container, p2.row2Container) = (p2.row2Container, p1.row2Container);
        (p1.row3Container, p2.row3Container) = (p2.row3Container, p1.row3Container);
        (p1.row1Counter, p2.row1Counter) = (p2.row1Counter, p1.row1Counter);
        (p1.row2Counter, p2.row2Counter) = (p2.row2Counter, p1.row2Counter);
        (p1.row3Counter, p2.row3Counter) = (p2.row3Counter, p1.row3Counter);
        (p1.mainCounter, p2.mainCounter) = (p2.mainCounter, p1.mainCounter);
        (p1.pointsToLoseCounter, p2.pointsToLoseCounter) = (p2.pointsToLoseCounter, p1.pointsToLoseCounter);
        (p1.endGameScreenButton, p2.endGameScreenButton) = (p2.endGameScreenButton, p1.endGameScreenButton);
        (p1.endGameScreenParent, p2.endGameScreenParent) = (p2.endGameScreenParent, p1.endGameScreenParent);
        (p1.endGameScreen, p2.endGameScreen) = (p2.endGameScreen, p1.endGameScreen);
        (p1.endGameScreenText, p2.endGameScreenText) = (p2.endGameScreenText, p1.endGameScreenText);
        (p1.handCardContainer, p2.handCardContainer) = (p2.handCardContainer, p1.handCardContainer);
        (p1.deckImage, p2.deckImage) = (p2.deckImage, p1.deckImage);
        (p1.graveyardImage, p2.graveyardImage) = (p2.graveyardImage, p1.graveyardImage);
    }

    public void DestroyAllCards()
    {
        foreach (Transform child in player1.handCardContainer.transform) Destroy(child.gameObject);
        foreach (Transform child in player1.row1Container.transform) Destroy(child.gameObject);
        foreach (Transform child in player1.row2Container.transform) Destroy(child.gameObject);
        foreach (Transform child in player1.row3Container.transform) Destroy(child.gameObject);

        foreach (Transform child in player2.handCardContainer.transform) Destroy(child.gameObject);
        foreach (Transform child in player2.row1Container.transform) Destroy(child.gameObject);
        foreach (Transform child in player2.row2Container.transform) Destroy(child.gameObject);
        foreach (Transform child in player2.row3Container.transform) Destroy(child.gameObject);
    }

    public void RenderAllCards()
    {
        RenderPlayerCards(player1);
        RenderPlayerCards(player2);
    }

    private void RenderPlayerCards(PlayerResources player)
    {
        foreach (var card in player.HandList) cardRenderer.RenderCard(player, card, player.handCardContainer.transform);

        foreach (var card in player.Row1List) cardRenderer.RenderCard(player, card, player.row1Container.transform);

        foreach (var card in player.Row2List) cardRenderer.RenderCard(player, card, player.row2Container.transform);

        foreach (var card in player.Row3List) cardRenderer.RenderCard(player, card, player.row3Container.transform);
    }
}