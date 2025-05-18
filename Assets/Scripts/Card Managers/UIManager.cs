using TMPro;
using UnityEngine;

public class UIManager
{
    private readonly TextMeshProUGUI turnText;
    private readonly TextMeshProUGUI turnNumberText;
    private readonly TextMeshProUGUI actionsLeftText;
    private readonly GameObject player1EndTurnButton;

    public UIManager(TextMeshProUGUI turnText, TextMeshProUGUI turnNumberText, TextMeshProUGUI actionsLeftText, GameObject player1EndTurnButton)
    {
        this.turnText = turnText;
        this.turnNumberText = turnNumberText;
        this.actionsLeftText = actionsLeftText;
        this.player1EndTurnButton = player1EndTurnButton;
    }

    public void UpdateRoundCounterDisplay(int roundNumber)
    {
        turnNumberText.text = $"Round Number: {roundNumber}";
    }

    public void SetTurnText(bool isPlayerTurn)
    {
        turnText.text = isPlayerTurn ? "Your Turn" : "Enemy Turn";
    }

    public void SetTrunButton(TurnsLogic turnsLogic)
    {
        bool isPlayerTurn = (turnsLogic.isMainPlayerTurn && turnsLogic.player1.isActive) || (!turnsLogic.isMainPlayerTurn && turnsLogic.player2.isActive);
        bool isMulligan = turnsLogic.player1.isPlayerInMulliganFaze || turnsLogic.player2.isPlayerInMulliganFaze;

        player1EndTurnButton.SetActive(isPlayerTurn && !isMulligan);
    }

    public void SetActionsLeftVisibility(int actionsLeft)
    {
        actionsLeftText.gameObject.SetActive(actionsLeft > 0);
        SetActionsLeftText(actionsLeft);
    }

    public void SetActionsLeftText(int actionsLeft)
    {
        actionsLeftText.text = "Actions: " + actionsLeft;
    }
}