using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DealDamageWhenIAmBoostedAbility : Ability
{
    private readonly int damage;
    private readonly bool needBecomeBoosted;
    private readonly bool useThisBoostInstead;
    private readonly int whenMyPowerReaches;
    public static int lastBoostValue;

    public DealDamageWhenIAmBoostedAbility(int damage, bool needBecomeBoosted = false, bool useThisBoostInstead = false, int whenMyPowerReaches = 0)
    {
        this.damage = damage;
        this.needBecomeBoosted = needBecomeBoosted;
        this.useThisBoostInstead = useThisBoostInstead;
        this.whenMyPowerReaches = whenMyPowerReaches;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (!card.wasAbilityExecuted)
        {
            card.wasAbilityExecuted = true;
            if (whenMyPowerReaches == 0) return;
            else card.Charges = 1;
        }

        if (needBecomeBoosted && ((lastBoostValue < (card.Power - card.BasePower)) || (card.Power <= card.BasePower))) return;
        if (whenMyPowerReaches != 0 && card.Charges <= 0) return;
        if (card.Power < whenMyPowerReaches) return;

        if (whenMyPowerReaches > 0) card.Charges = 0;

        List<Card> allCards = new();
        allCards.AddRange(playerResources.myEnemy.Row1List);
        allCards.AddRange(playerResources.myEnemy.Row2List);
        allCards.AddRange(playerResources.myEnemy.Row3List);

        int maxPower = allCards.Max(card => card.Power);

        List<Card> strongestCards = allCards.Where(card => card.Power == maxPower).ToList();

        if (strongestCards.Count > 0)
        {
            int randomIndex = Random.Range(0, strongestCards.Count);
            Card selectedCard = strongestCards[randomIndex];

            if (!useThisBoostInstead) selectedCard.Power -= damage;
            else selectedCard.Power -= lastBoostValue;
        }
    }
}