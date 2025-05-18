public class ChangeArmorAbility : Ability
{
    public readonly int armor;
    public readonly bool alsoGiveInvulnerability;

    public ChangeArmorAbility(int armor, bool alsoGiveInvulnerability = false)
    {
        this.armor = armor;
        this.alsoGiveInvulnerability = alsoGiveInvulnerability;
    }

    public override void Execute(PlayerResources playerResources, Card targetedCard)
    {
        targetedCard.Armor += armor;

        if (alsoGiveInvulnerability) targetedCard.isInvulnerable = true;
    }
}