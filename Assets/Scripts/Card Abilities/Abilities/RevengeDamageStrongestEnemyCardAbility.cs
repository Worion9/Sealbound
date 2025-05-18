using System.Linq;

public class RevengeDamageStrongestEnemyCardAbility : Ability
{
    public readonly int damage;

    public RevengeDamageStrongestEnemyCardAbility(int damage)
    {
        this.damage = damage;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        var strongestCard = playerResources.myEnemy.Row1List
            .Concat(playerResources.myEnemy.Row2List)
            .Concat(playerResources.myEnemy.Row3List)
            .Where(c => c.Power == playerResources.myEnemy.Row1List.Concat(playerResources.myEnemy.Row2List).Concat(playerResources.myEnemy.Row3List).Max(card => card.Power))
            .FirstOrDefault();

        if(strongestCard == null) return;

        strongestCard.Power -= damage;
    }
}