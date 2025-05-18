public class BoostCardWhenPlayedOnMySideAbility : Ability
{
    public readonly int boost;
    public readonly bool boostMe;

    public BoostCardWhenPlayedOnMySideAbility(int boost, bool boostMe = false)
    {
        this.boost = boost;
        this.boostMe = boostMe;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (playerResources.cardManager.cardToPlay != card)
        {
            if (!boostMe) playerResources.cardManager.cardToPlay.Power += boost;
            else card.Power += boost;
        }
    }
}