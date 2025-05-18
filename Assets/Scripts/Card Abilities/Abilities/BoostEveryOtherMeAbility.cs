using UnityEngine;
using System.Linq;

public class BoostEveryOtherMeAbility : Ability
{
    public readonly int boost;
    public readonly bool boostMeInstead;

    public BoostEveryOtherMeAbility(int boost, bool boostMeInstead)
    {
        this.boost = boost;
        this.boostMeInstead = boostMeInstead;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (boostMeInstead)
        {
            int currentBoost = boost * playerResources.bearExecutions;
            card.Power += currentBoost;
            playerResources.bearExecutions++;

            var otherCards = playerResources.Row1List.Concat(playerResources.Row2List).Concat(playerResources.Row3List).Concat(playerResources.HandList).Concat(playerResources.GraveyardList).Where(c => c.Id == card.Id);
            foreach (var c in otherCards)
            {
                c.Description = $"Boost me by {3 * playerResources.bearExecutions}. The boost increases by 3 for each <b>Bear</b> you play this game.";
            }
        }
        else
        {
            var otherCards = playerResources.Row1List.Concat(playerResources.Row2List).Concat(playerResources.Row3List).Where(c => c.Id == card.Id && c != card);

            foreach (var c in otherCards)
            {
                c.Power += boost;
            }
        }
    }
}