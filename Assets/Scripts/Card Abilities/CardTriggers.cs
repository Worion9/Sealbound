using System;
using System.Collections.Generic;
using System.Linq;

public class CardTriggers
{
    private void ExecuteAbility<T>(PlayerResources playerResources, Card card) where T : Ability
    {
        if (card.AreAbilitiesBlocked) return;
        T ability = card.Abilities.OfType<T>().FirstOrDefault();
        ability?.Execute(playerResources, card);
        playerResources.turnsLogic.QuickSync();
    }

    public void TriggerWhenThisCardIsPlayed(PlayerResources playerResources, Card card)
    {
        Type[] abilityTypes =
        {
            typeof(CooldownMoveMeToNextRowAbility),
            typeof(DrawCardsAbility),
            typeof(DrawCardsToAmountAbility),
            typeof(PlayAdditionalCardAbility),
            typeof(MulliganCardsAbility),
            typeof(SelectCardFromDecksAbility),
            typeof(SelectCardFromGraveyardAbility),
            typeof(ShowRandomCardsAndPlayOneAbility),
            typeof(IncreaseOrDecreasePointsToWinAbility),
            typeof(ChangePointsToWinAbility),
            typeof(StartBoardSelectionAbility),
            typeof(CooldownDrawCardsAbility),
            typeof(CooldownBoostMeOrDestroyBoardAbility),
            typeof(CooldownBoostMeOrDealDamageToMeAbility),
            typeof(DiscrardCardsToBoostMeAbility),
            typeof(ChangeMyPowerByCartsInYourDeckAbility),
            typeof(PlayRandomCardsAbility),
            typeof(ShowAndPlayCardsFromDeckAbility),
            typeof(CooldownBoostMeAbility),
            typeof(BoostMeIfTurnNumberEqualsAbility),
            typeof(BoostAllCardsOnMyRowAbility),
            typeof(DamageAllCardsOnOppositeRowAbility),
            typeof(CooldownDamageAllCardsOnThisRowAbility),
            typeof(BoostMeByTurnNumberAndDagameMeByEnemyCardsOnBoardAbility),
            typeof(CooldownGainChargeAbility),
            typeof(ChangePowerOfAllCardsOnBoardAbility),
            typeof(SummonMyCopyFromDeckAbility),
            typeof(DelayAbility),
            typeof(SpawnCardAbility),
            typeof(BoostEveryOtherMeAbility),
            typeof(ConsumeCardsAbility),
            typeof(DestroyBoardAbility),
            typeof(CooldownConsumeCardsAbility),
            typeof(DamageAllStrongestEnemyCardsAbility),
            typeof(CooldownDamageStrongestEnemyCardsAbility),
            typeof(CooldownBoostCardAbility),
            typeof(BoostCardInHandAbility),
            typeof(BoostAllYourWeakestCardsAbility),
            typeof(ChangeBasePowerOfAllCardsAbility),
            typeof(BoostMeForCardsInHandAbility),
            typeof(ShowCardsPlayOneAbility),
            typeof(ChooseBostMeByRandomNumberAbility),
            typeof(BoostMeForHpAbility),
            typeof(MoveAllCardsInRowAbility),
            typeof(SetAllCardsInMyRowPowerAbility),
            typeof(ChooseDamageOrBoostRowAbility),
            typeof(CooldownBoostAllYourWeakestCardsAbility),
            typeof(StrengthenMeAbility),
            typeof(CreateAndPlayCardAbility),
            typeof(ChooseConsumeOrSpawnEggAbility),
            typeof(KillAllStrongestOrWeakestCardsAbility),
            typeof(KillAllEnemyWoundedCardsAbility),
            typeof(BoostMeForEveryBoostedCardOnYourSideAbility),
            typeof(DealDamageWhenIAmBoostedAbility),
            typeof(BoostMeForEveryWoundedCardOnEnemySideAbility),
            typeof(BoostMeIfDeckConditionsAreMetAbility),
            typeof(CooldownBoostAllCardsOnThisRowAbility),
            typeof(BoostMeByTurnNumberReductedByCardsInRowAbility),
            typeof(BoostMeByTurnNumberAbility),
            typeof(SummonAllMyCopiesFromGraveyardAbility),
            typeof(DiscardCardsAbility)
        };

        foreach (Type type in abilityTypes)
        {
            var method = typeof(CardTriggers).GetMethod("ExecuteAbility", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var genericMethod = method.MakeGenericMethod(type);
            genericMethod.Invoke(this, new object[] { playerResources, card });
        }
    }

    public void TriggerAtTheBeginningOfOwnerTurn(PlayerResources playerResources, List<Card> row)
    {
        List<Card> rowCopy = new(row);
        foreach (Card card in rowCopy)
        {
            ExecuteAbility<CooldownMoveMeToNextRowAbility>(playerResources, card);
            ExecuteAbility<CooldownDrawCardsAbility>(playerResources, card);
            ExecuteAbility<CooldownBoostMeOrDestroyBoardAbility>(playerResources, card);
            ExecuteAbility<CooldownBoostMeOrDealDamageToMeAbility>(playerResources, card);
            ExecuteAbility<CooldownBoostMeAbility>(playerResources, card);
            ExecuteAbility<CooldownDamageAllCardsOnThisRowAbility>(playerResources, card);
            ExecuteAbility<CooldownGainChargeAbility>(playerResources, card);
            ExecuteAbility<DelayAbility>(playerResources, card);
            ExecuteAbility<CooldownConsumeCardsAbility>(playerResources, card);
            ExecuteAbility<CooldownDamageStrongestEnemyCardsAbility>(playerResources, card);
            ExecuteAbility<CooldownBoostCardAbility>(playerResources, card);
            ExecuteAbility<CooldownBoostAllYourWeakestCardsAbility>(playerResources, card);
            ExecuteAbility<CooldownBoostAllCardsOnThisRowAbility>(playerResources, card);
        }
    }

    public void TriggerWhenCardIsSummond(PlayerResources playerResources, Card card)
    {
        ExecuteAbility<DelayAbility>(playerResources, card);
    }

    public void TriggerOnceFromDeckOrHandAtTheBeginningOfOwnerTurn(PlayerResources playerResources, List<Card> cardList)
    {
        foreach (Card card in cardList)
        {
            if (card.HasAbility<AutoPlayMeFromDeckOrHandAbility>())
            {
                ExecuteAbility<AutoPlayMeFromDeckOrHandAbility>(playerResources, card);
                return;
            }
        }
    }

    public void TriggerWhenPlayerDrawCard(PlayerResources playerResources, List<Card> row)
    {
        foreach (Card card in row)
        {
            ExecuteAbility<BoostMeOnCardDrawAbility>(playerResources, card);
        }
    }

    public void TriggerWhenCardIsMulligan(PlayerResources playerResources, Card card)
    {
        ExecuteAbility<AutoPlayMeOnMillganOrDiscardAbility>(playerResources, card);
    }

    public void TriggerWhenCardAppearsOnMyRow(PlayerResources playerResources, List<Card> row)
    {
        foreach (Card card in row)
        {
            ExecuteAbility<BoostCardWhenPlayedOnMyRowAbility>(playerResources, card);
            ExecuteAbility<GiveAllMyBonusesAbility>(playerResources, card);
            ExecuteAbility<ChangeMyPowerWhenCardsIsPlayedOnMyRow>(playerResources, card);
        }
    }

    public void TriggerWhenCardAppearsOnOppositeRow(PlayerResources playerResources, List<Card> row)
    {
        foreach (Card card in row)
        {
            ExecuteAbility<DamageCardWhenPlayedOppositeRowAbility>(playerResources, card);
        }
    }

    public void TriggerWhenCardAppearsOnMySide(PlayerResources playerResources, List<Card> row)
    {
        foreach (Card card in row)
        {
            ExecuteAbility<BoostCardWhenPlayedOnMySideAbility>(playerResources, card);
        }
    }

    public void TriggerWhenCardAppearsOnOppositeSide(PlayerResources playerResources, List<Card> row)
    {
        foreach (Card card in row)
        {
            ExecuteAbility<DealDamageToCardWhenPlayedAbility>(playerResources, card);
        }
    }

    public void TriggerWhenCardIsKilled(PlayerResources playerResources, Card card)
    {
        ExecuteAbility<RevengeResMeAbility>(playerResources, card);
        ExecuteAbility<RevengeBoostTheWeakestCardOnEnemySideAbility>(playerResources, card);
        ExecuteAbility<RevengeChangePowerOfAllCardsInMyRowAbility>(playerResources, card);
        ExecuteAbility<RevengeSpawnCardAbility>(playerResources, card);
        ExecuteAbility<RevengeSummonMyCopyFromDeckAbility>(playerResources, card);
        ExecuteAbility<RevengeDamageStrongestEnemyCardAbility>(playerResources, card);
        ExecuteAbility<RevengeCreateCardInHandAbility>(playerResources, card);
    }

    public void TriggerOnEnemyRowWhenCardIsDamaged(PlayerResources playerResources, List<Card> row)
    {
        foreach (Card card in row)
        {
            ExecuteAbility<BoostMeWhenEnemyCardIsDamagedAbility>(playerResources, card);
        }
    }

    public void TriggerWhenIAmBoosted(PlayerResources playerResources, Card card)
    {
        ExecuteAbility<DealDamageWhenIAmBoostedAbility>(playerResources, card);
    }

    public void TriggerWhenYourCardIsMoving(PlayerResources playerResources, List<Card> row)
    {
        foreach (Card card in row)
        {
            ExecuteAbility<WhenYourCardMovesBoostItAbility>(playerResources, card);
        }
    }

    public void TriggerWhenEnemyCardIsMoving(PlayerResources playerResources, List<Card> row)
    {
        foreach (Card card in row)
        {
            ExecuteAbility<WhenEnemyCardMovesDamageItAbility>(playerResources, card);
        }
    }

    public void TriggerWhenAnyCardOnYourSideDies(PlayerResources playerResources, List<Card> row)
    {
        row = row.ToList();
        foreach (Card card in row)
        {
            ExecuteAbility<WhenOtherCardDieSpawnCardAbility>(playerResources, card);
        }
    }

    public void TriggerWhenAnyCardOnEnemySideDies(PlayerResources playerResources, List<Card> row)
    {
        row = row.ToList();
        foreach (Card card in row)
        {
            ExecuteAbility<BoostMeWhenEnemyCardDiesAbility>(playerResources, card);
        }
    }
}