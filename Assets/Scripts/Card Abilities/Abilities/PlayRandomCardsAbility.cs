using UnityEngine;

public class PlayRandomCardsAbility : Ability
{
    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        if (playerResources.DeckList.Count <= 0) return;
        int randomCardIndex = Random.Range(0, playerResources.DeckList.Count);
        playerResources.cardManager.ForceCardToBePlayed(playerResources, randomCardIndex, false);
    }
}