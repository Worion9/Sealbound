using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Card
{
    public Guid InstanceId { get; set; } = Guid.NewGuid();

    public enum Rarity
    {
        None,
        Common,
        Rare,
        Epic,
        Legendary
    }

    public enum ParentRowListEnum
    {
        none,
        Player1Row1,
        Player1Row2,
        Player1Row3,
        Player2Row1,
        Player2Row2,
        Player2Row3
    }
    public ParentRowListEnum parentRowListEnum;

    [field: NonSerialized] public List<Card> ParentRowList { get; set; }
    [field: SerializeField] public DisplayCard DisplayCard { get; set; }
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    public string Description { get; set; }
    public bool IsSpy { get; set; }
    public string ImageName => $"CardImages/{imageName}";
    private readonly string imageName;

    public int power;
    public int Power
    {
        get => power;
        set
        {
            if (power == value) return;
            if (isSpell) return;
            if (isInvulnerable) return;

            if (DisplayCard != null)
            if (DisplayCard.playerResources.GraveyardList.Contains(this) || DisplayCard.playerResources.myEnemy.GraveyardList.Contains(this)) return;

            int damage = power - value;
            if (damage > 0)
            {
                CardManager.DealDamageToCard(this, damage);
            }
            else
            {
                power = value;
                DealDamageWhenIAmBoostedAbility.lastBoostValue = -damage;
                DisplayCard?.playerResources.cardTriggers.TriggerWhenIAmBoosted(DisplayCard.playerResources, this);
            }

            if (DisplayCard == null) return;
            DisplayCard.UpdateCardUI();
        }
    }

    [SerializeField] private int basePower;
    public int BasePower
    {
        get => basePower;
        set
        {
            if (basePower == value) return;
            if (isSpell) return;
            if (isInvulnerable) return;

            if (DisplayCard != null)
            if (DisplayCard.playerResources.GraveyardList.Contains(this) || DisplayCard.playerResources.myEnemy.GraveyardList.Contains(this)) return;

            basePower = value;

            if (DisplayCard == null) return;
            DisplayCard.UpdateCardUI();
        }
    }

    [SerializeField] private int armor;
    public int Armor
    {
        get => armor;
        set
        {
            if (armor == value) return;
            if (isSpell) return;
            if (isInvulnerable) return;

            if (DisplayCard != null)
            if (DisplayCard.playerResources.GraveyardList.Contains(this) || DisplayCard.playerResources.myEnemy.GraveyardList.Contains(this)) return;

            armor = value;

            if (DisplayCard == null) return;
            DisplayCard.UpdateCardUI();
        }
    }

    [SerializeField] private int charges;
    public int Charges
    {
        get => charges;
        set
        {
            if (charges == value) return;
            charges = value;
            if (DisplayCard == null) return;
            if (DisplayCard.playerResources.GraveyardList.Contains(this) || DisplayCard.playerResources.myEnemy.GraveyardList.Contains(this)) return;

            DisplayCard.UpdateCardUI();
        }
    }

    [SerializeField] private int counter;
    public int Counter
    {
        get => counter;
        set
        {
            if (counter == value) return;
            counter = value;
            if (DisplayCard == null) return;
            if (DisplayCard.playerResources.GraveyardList.Contains(this) || DisplayCard.playerResources.myEnemy.GraveyardList.Contains(this)) return;

            DisplayCard.UpdateCardUI();
        }
    }

    public bool isSpell;
    public bool isInvulnerable;
    public bool wasAbilityExecuted;
    public bool isCausingSelectionNow;
    [SerializeField]private bool areAbilitiesBlocked;
    public bool AreAbilitiesBlocked
    {
        get => areAbilitiesBlocked;
        set
        {
            areAbilitiesBlocked = value;
            if (DisplayCard != null)
            {
                DisplayCard.UpdateBlockedState(areAbilitiesBlocked);
            }
        }
    }
    private Rarity rarity = Rarity.None;
    public Rarity CardRarity
    {
        get => rarity;
        set => rarity = value;
    }

    public List<Ability> Abilities { get; set; } = new();

    public Card(int id, string name, int power, int armor, bool isSpy, string description, string imageName, Rarity rarity, bool isInvulnerable = false, List<Card> ParentList = null)
    {
        Id = id;
        Name = name;
        Power = power;
        BasePower = power;
        Armor = armor;
        IsSpy = isSpy;
        Description = description;
        this.imageName = imageName;
        this.rarity = rarity;
        isSpell = Power == 0;
        this.isInvulnerable = isInvulnerable;
        ParentRowList = ParentList;
    }

    public void AddAbility(Ability ability) => Abilities.Add(ability);

    public Card Clone()
    {
        var clonedCard = new Card(Id, Name, Power, Armor, IsSpy, Description, imageName, rarity, isInvulnerable, ParentRowList)
        {
            Abilities = Abilities,
            InstanceId = Guid.NewGuid()
        };
        return clonedCard;
    }

    public bool HasAbility<T>() where T : Ability
    {
        return Abilities?.Any(ability => ability is T) == true;
    }

    public void TriggerFirstAbility(PlayerResources playerResources, Card card)
    {
        if (Abilities.Any())
        {
            Abilities.First().Execute(playerResources, card);
        }
    }

    public void BlockAllAbilities()
    {
        AreAbilitiesBlocked = true;
    }
}