using System.Linq;

public class AutoPlayMeFromDeckOrHandAbility : Ability
{
    public static bool alreadyUsed;

    public override void Execute(PlayerResources playerResources, Card card)
    {
        if (alreadyUsed || playerResources.isPlayerInMulliganFaze || playerResources.myEnemy.isPlayerInMulliganFaze) return;

        // Krok 1: Zlicz karty odpowiadaj¹ce "card" w rêce i talii
        int countInHand = playerResources.HandList.Count(c => c.Id == card.Id);
        int countInDeck = playerResources.DeckList.Count(c => c.Id == card.Id);

        // Jeœli brak takich kart w talii i na rêce, zakoñcz
        if (countInHand == 0 && countInDeck == 0) return;

        // Krok 2: Wybierz losowo kartê z rêki lub talii
        System.Random rng = new();
        bool selectFromHand = countInHand > 0 && (countInDeck == 0 || rng.NextDouble() < (double)countInHand / (countInHand + countInDeck));

        // Krok 3: Wywo³aj odpowiedni¹ metodê AutoPlay
        if (selectFromHand)
            playerResources.cardManager.SummonCard(playerResources, card, true, playerResources.HandList);
        else
            playerResources.cardManager.SummonCard(playerResources, card, false, playerResources.DeckList);

        // Zaznacz, ¿e karta zosta³a u¿yta
        alreadyUsed = true;
    }
}