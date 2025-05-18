using UnityEngine;

public class ReplayCardAbility : Ability
{
    private readonly bool boostMe;

    public ReplayCardAbility(bool boostMe = false)
    {
        this.boostMe = boostMe;
    }

    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        if (playerResources.boardManager.CardThatCausedSelection.Id == targetedCard.Id) return;

        if (boostMe) playerResources.boardManager.CardThatCausedSelection.Power += Mathf.RoundToInt(targetedCard.Power * 1.5f + 0.0001f);

        int basePower = targetedCard.BasePower;
        CardManager.KillCard(targetedCard, false, false);

        targetedCard = CardDatabase.GetCard(targetedCard.Id).Clone();
        targetedCard.BasePower = basePower;
        targetedCard.power = basePower;

        playerResources.DeckList.Insert(0, targetedCard);
        playerResources.cardManager.ForceCardToBePlayed(playerResources, 0, false);
    }
}