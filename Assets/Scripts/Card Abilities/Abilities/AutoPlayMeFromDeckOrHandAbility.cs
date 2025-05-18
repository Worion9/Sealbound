using System.Linq;

public class AutoPlayMeFromDeckOrHandAbility : Ability
{
    public static bool alreadyUsed;

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (alreadyUsed || playerResources.isPlayerInMulliganFaze || playerResources.myEnemy.isPlayerInMulliganFaze) return;

        // Krok 1: Zlicz karty odpowiadaj�ce "card" w r�ce i talii
        int countInHand = playerResources.HandList.Count(c => c.Id == card.Id);
        int countInDeck = playerResources.DeckList.Count(c => c.Id == card.Id);

        // Je�li brak takich kart w talii i na r�ce, zako�cz
        if (countInHand == 0 && countInDeck == 0) return;

        // Krok 2: Wybierz losowo kart� z r�ki lub talii
        System.Random rng = new();
        bool selectFromHand = countInHand > 0 && (countInDeck == 0 || rng.NextDouble() < (double)countInHand / (countInHand + countInDeck));

        // Krok 3: Wywo�aj odpowiedni� metod� AutoPlay
        if (selectFromHand)
            playerResources.cardManager.SummonCard(playerResources, card, true, playerResources.HandList);
        else
            playerResources.cardManager.SummonCard(playerResources, card, false, playerResources.DeckList);

        // Zaznacz, �e karta zosta�a u�yta
        alreadyUsed = true;
    }
}