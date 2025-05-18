using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangePowerAbility : Ability
{
    public readonly int Power;
    public readonly bool MultiplyByTurnNumber;
    public readonly bool ApplyForAllCopies;
    public readonly bool PercentageChange;
    public readonly bool BoostMeByRemainingDamage;
    public readonly bool AlsoAdjacentCards;
    public readonly bool ReducedByCardsInRow;
    public readonly bool MultiplyByCardsInMyRow;

    public ChangePowerAbility(int power, bool multiplyByTurnNumber = false, bool applyForAllCopies = false,
                              bool percentageChange = false, bool boostMeByRemainingDamage = false,
                              bool alsoAdjacentCards = false, bool reducedByCardsInRow = false,
                              bool multiplyByCardsInMyRow = false)
    {
        Power = power;
        MultiplyByTurnNumber = multiplyByTurnNumber;
        ApplyForAllCopies = applyForAllCopies;
        PercentageChange = percentageChange;
        BoostMeByRemainingDamage = boostMeByRemainingDamage;
        AlsoAdjacentCards = alsoAdjacentCards;
        ReducedByCardsInRow = reducedByCardsInRow;
        MultiplyByCardsInMyRow = multiplyByCardsInMyRow;
    }

    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        if (PercentageChange)
        {
            ApplyPercentageChange(targetedCard);
        }
        else if (MultiplyByTurnNumber)
        {
            ApplyMultiplier(targetedCard);
        }
        else if (ApplyForAllCopies)
        {
            ApplyToAllCopies(targetedCard);
        }
        else
        {
            ApplySingleCardEffect(playerResources, targetedCard);
        }
    }

    private void ApplyPercentageChange(Card card)
    {
        card.Power += Mathf.RoundToInt(Power * card.Power / 100f + 0.001f);
    }

    private void ApplyMultiplier(Card card)
    {
        int adjustment = Power * TurnsLogic.turnNumber;
        if (ReducedByCardsInRow)
        {
            adjustment -= card.ParentRowList.Count - 1;
        }
        card.Power += adjustment;
    }

    private void ApplyToAllCopies(Card targetedCard)
    {
        int targetedCardId = targetedCard.Id;

        var allCards = GetAllCardsFromPlayer(targetedCard.DisplayCard.playerResources);
        foreach (Card card in allCards)
        {
            if (card.Id == targetedCardId)
            {
                card.Power += Power;
            }
        }
    }

    private void ApplySingleCardEffect(PlayerResources playerResources, Card targetedCard)
    {
        if (BoostMeByRemainingDamage)
        {
            BoostSelfByRemainingDamage(playerResources, targetedCard);
        }

        if (AlsoAdjacentCards)
        {
            ApplyToAdjacentCards(targetedCard);
        }
        else if (MultiplyByCardsInMyRow)
        {
            ApplyMultiplyByCardsInMyRow(targetedCard);
        }
        else
        {
            targetedCard.Power += Power;
        }
    }

    private void BoostSelfByRemainingDamage(PlayerResources playerResources, Card targetedCard)
    {
        int boost = -Power - targetedCard.Power;
        if (boost > 0)
        {
            playerResources.boardManager.CardThatCausedSelection.Power += boost;
        }
    }

    private void ApplyToAdjacentCards(Card targetedCard)
    {
        int targetedCardIndex = targetedCard.ParentRowList.IndexOf(targetedCard);

        List<Card> adjacentCards = targetedCard.ParentRowList
            .Where((card, index) =>
                index == targetedCardIndex ||
                index == targetedCardIndex - 1 ||
                index == targetedCardIndex + 1)
            .ToList();

        foreach (Card card in adjacentCards)
        {
            card.Power += Power;
        }
    }

    private void ApplyMultiplyByCardsInMyRow(Card card)
    {
        card.Power += Power * card.ParentRowList.Count;
    }

    private IEnumerable<Card> GetAllCardsFromPlayer(PlayerResources playerResources)
    {
        return playerResources.Row1List.Concat(playerResources.Row2List).Concat(playerResources.Row3List).ToList();
    }
}