using System;
using Unity.Netcode;

[Serializable]
public struct SerializableCard : IEquatable<SerializableCard>, INetworkSerializable
{
    public Guid InstanceId;
    public int Id;
    public int Power;
    public int BasePower;
    public int Armor;
    public int Charges;
    public int Counter;
    public bool IsSpy;
    public bool WasAbilityExecuted;
    public bool AreAbilitiesBlocked;

    public SerializableCard(Card card)
    {
        InstanceId = card.InstanceId;
        Id = card.Id;
        Power = card.Power;
        BasePower = card.BasePower;
        Armor = card.Armor;
        Charges = card.Charges;
        Counter = card.Counter;
        IsSpy = card.IsSpy;
        WasAbilityExecuted = card.wasAbilityExecuted;
        AreAbilitiesBlocked = card.AreAbilitiesBlocked;
    }

    public SerializableCard(SerializableCard card)
    {
        InstanceId = card.InstanceId;
        Id = card.Id;
        Power = card.Power;
        BasePower = card.BasePower;
        Armor = card.Armor;
        Charges = card.Charges;
        Counter = card.Counter;
        IsSpy = card.IsSpy;
        WasAbilityExecuted = card.WasAbilityExecuted;
        AreAbilitiesBlocked = card.AreAbilitiesBlocked;
    }

    public readonly Card ToCard()
    {
        Card newCard = CardDatabase.GetCard(Id).Clone();
        newCard.InstanceId = InstanceId;
        newCard.power = Power;
        newCard.BasePower = BasePower;
        newCard.Armor = Armor;
        newCard.Charges = Charges;
        newCard.Counter = Counter;
        newCard.IsSpy = IsSpy;
        newCard.wasAbilityExecuted = WasAbilityExecuted;
        newCard.AreAbilitiesBlocked = AreAbilitiesBlocked;

        return newCard;
    }

    public bool Equals(SerializableCard other)
    {
        return InstanceId.Equals(other.InstanceId);
    }

    public override int GetHashCode()
    {
        return InstanceId.GetHashCode();
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        byte[] guidBytes;

        if (serializer.IsWriter)
        {
            guidBytes = InstanceId.ToByteArray();
            for (int i = 0; i < 16; i++)
                serializer.SerializeValue(ref guidBytes[i]);
        }
        else
        {
            guidBytes = new byte[16];
            for (int i = 0; i < 16; i++)
                serializer.SerializeValue(ref guidBytes[i]);
            InstanceId = new Guid(guidBytes);
        }

        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref Power);
        serializer.SerializeValue(ref BasePower);
        serializer.SerializeValue(ref Armor);
        serializer.SerializeValue(ref Charges);
        serializer.SerializeValue(ref Counter);
        serializer.SerializeValue(ref IsSpy);
        serializer.SerializeValue(ref WasAbilityExecuted);
        serializer.SerializeValue(ref AreAbilitiesBlocked);
    }
}