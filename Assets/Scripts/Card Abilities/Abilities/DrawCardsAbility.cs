public class DrawCardsAbility : Ability
{
    public int ammount;
    public bool enemyDrawsInstead;

    public DrawCardsAbility(int ammount, bool enemyDrawsInstead = false)
    {
        this.ammount = ammount;
        this.enemyDrawsInstead = enemyDrawsInstead;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        for (int i = 0; i < ammount; i++)
        {
            if(!enemyDrawsInstead) playerResources.cardManager.DrawCard(playerResources, true);
            else playerResources.cardManager.DrawCard(playerResources.myEnemy, true);
        }
    }
}