public class BoostCardWhenPlayedOnMyRowAbility : Ability
{
    public readonly int boost;
    public readonly bool armorInsteadOfBoost;
    public readonly bool upToPower;

    public BoostCardWhenPlayedOnMyRowAbility(int boost, bool armorInsteadOfBoost = false, bool upToPower = false)
    {
        this.boost = boost;
        this.armorInsteadOfBoost = armorInsteadOfBoost;
        this.upToPower = upToPower;
    }

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (playerResources.cardManager.cardToPlay != card)
        {
            if (!armorInsteadOfBoost)
            {
                if (!upToPower)
                {
                    playerResources.cardManager.cardToPlay.Power += boost;
                }
                else
                {
                    int newBoost = boost - playerResources.cardManager.cardToPlay.Power;
                    if(newBoost > 0) playerResources.cardManager.cardToPlay.Power += newBoost;
                }
            }
            else
            {
                playerResources.cardManager.cardToPlay.Armor += boost;
            }
        }
    }
}