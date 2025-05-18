using UnityEngine;

public class BoostMeForHpAbility : Ability
{
    private readonly int hpNeeded;
    private readonly bool enemyHp;

    public BoostMeForHpAbility(int hpNeeded, bool enemyHp)
    {
        this.hpNeeded = hpNeeded;
        this.enemyHp = enemyHp;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        int trueHp;

        if(enemyHp) trueHp = playerResources.myEnemy.HP - card.Power;
        else trueHp = playerResources.HP - card.Power;

        int boost = Mathf.FloorToInt(trueHp / (float)hpNeeded);
        card.Power += boost;
    }
}