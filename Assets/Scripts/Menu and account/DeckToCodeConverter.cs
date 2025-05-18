using System.Collections.Generic;
using System.Linq;

public class DeckToCodeConverter
{
    private const string Base32Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV";

    public string ConvertDeckToString(List<Card> deck)
    {
        var cardGroups = deck
            .OrderBy(card => card.Id)
            .GroupBy(card => card.Id)
            .Select(group => new { Id = group.Key, Count = group.Count() })
            .ToList();

        return string.Join("", cardGroups.Select(group => ToBase32(group.Count) + ToBase32(group.Id, 2)));
    }

    public List<Card> ConvertStringToDeck(string encodedDeck)
    {
        var deck = new List<Card>();

        for (int i = 0; i < encodedDeck.Length; i += 3)
        {
            string countBase32 = encodedDeck[i].ToString();
            string idBase32 = encodedDeck.Substring(i + 1, 2);

            int count = FromBase32(countBase32);
            int id = FromBase32(idBase32);

            Card templateCard = CardDatabase.cardList.FirstOrDefault(card => card.Id == id);

            if (templateCard != null)
            {
                for (int j = 0; j < count; j++)
                {
                    deck.Add(templateCard.Clone());
                }
            }
        }

        return deck;
    }

    public bool CheckIfDeckCodeIsValid(string deckCode)
    {
        // SprawdŸ, czy kod talia nie jest pusty
        if (string.IsNullOrEmpty(deckCode)) return false;

        // SprawdŸ, czy liczba znaków w deckCode jest wielokrotnoœci¹ 3
        if (deckCode.Length % 3 != 0) return false;

        // SprawdŸ, czy wszystkie znaki nale¿¹ do Base32Chars
        if (deckCode.Any(c => !Base32Chars.Contains(c))) return false;

        // Podziel deckCode na grupy po 3 znaki
        for (int i = 0; i < deckCode.Length; i += 3)
        {
            string group = deckCode.Substring(i, 3);

            // Pierwszy znak powinien byæ od '1' do '9' lub 'A'
            char countChar = group[0];
            if (!("123456789A".Contains(countChar))) return false;

            // Kolejne dwa znaki to ID karty w systemie Base32
            string idBase32 = group.Substring(1, 2);
            int cardId = FromBase32(idBase32);

            // SprawdŸ, czy ID karty jest w zakresie CardDatabase.cardList
            Card card = CardDatabase.cardList.FirstOrDefault(c => c.Id == cardId);
            if (card == null || card.CardRarity == Card.Rarity.None) return false;
        }

        // Wszystkie warunki zosta³y spe³nione
        return true;
    }

    private string ToBase32(int value, int minLength = 1)
    {
        string result = "";

        do
        {
            result = Base32Chars[value % 32] + result;
            value /= 32;
        } while (value > 0);

        while (result.Length < minLength)
        {
            result = "0" + result;
        }

        return result;
    }

    private int FromBase32(string base32Value)
    {
        int result = 0;

        foreach (char c in base32Value)
        {
            result = result * 32 + Base32Chars.IndexOf(c);
        }

        return result;
    }
}