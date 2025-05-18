public class CooldownMoveMeToNextRowAbility : Ability
{
    public readonly int renewal;

    public CooldownMoveMeToNextRowAbility(int renewal)
    {
        this.renewal = renewal;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (renewal > 1)
        {
            card.Counter--;

            if (card.Counter <= 0)
            {
                card.Counter = renewal;

                if ((playerResources.cardManager.cardToPlay != card) &&
                    (card.ParentRowList == playerResources.Row1List || card.ParentRowList == playerResources.Row2List))
                    card.Counter++; // Zabezpieczenie, aby umiejêtnoœæ nie wykona³a siê dwa razy
            }
            else return;
        }

        playerResources.boardManager.MoveCardToNextRow(playerResources, card);
        card.TriggerFirstAbility(playerResources, card);
    }
}