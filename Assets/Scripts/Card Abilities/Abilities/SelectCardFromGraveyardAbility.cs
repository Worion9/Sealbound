public class SelectCardFromGraveyardAbility : Ability
{
    private readonly int amount;
    private readonly bool toDeck;
    private readonly bool allCopies;
    private readonly bool toBanish;
    private readonly bool onlyEntities;

    public SelectCardFromGraveyardAbility(int amount, bool toDeck, bool allCopies = false, bool toBanish = false, bool onlyEntities = false)
    {
        this.amount = amount;
        this.toDeck = toDeck;
        this.allCopies = allCopies;
        this.toBanish = toBanish;
        this.onlyEntities = onlyEntities;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (playerResources.GraveyardList.Count <= 0) return;
        playerResources.graveyardManager.SelectCardsFromGraveyard(playerResources, amount, toDeck, allCopies, toBanish, onlyEntities);
    }
}
