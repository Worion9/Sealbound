using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Card;

public class CardDatabase : MonoBehaviour
{
    public static List<Card> cardList = new();

    private Card cardToAdd;
    private int id = 0;

    private void Awake()
    {
        if (cardList.Count > 0) return;

        cardToAdd = new Card(id++, "Statue of Justice", 7, 0, false, "This card appears by itself on the side of the player who does not start the game.", "StatueOfJustice", Rarity.None);
        // no ability
        cardList.Add(cardToAdd);

        ///////////////////////////////////////////////////////////////////////////////// Manipulacja Kartami

        cardToAdd = new Card(id++, "Junior Mage", 11, 0, false, "Draw a card.", "JuniorMage", Rarity.Common);
        cardToAdd.AddAbility(new DrawCardsAbility(1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Senior Mage", 8, 0, false, "Draw 2 cards.", "SeniorMage", Rarity.Common);
        cardToAdd.AddAbility(new DrawCardsAbility(2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Ascended Mage", 6, 0, false, "Draw cards until you have at least 5.", "AscendedMage", Rarity.Common);
        cardToAdd.AddAbility(new DrawCardsToAmountAbility(5));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Patient Mage", 7, 0, false, "Draw a card. <b>Cooldown 4.</b>", "PatientMage", Rarity.Common);
        cardToAdd.AddAbility(new CooldownDrawCardsAbility(4));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Visionary", 4, 0, true, "<b>Spy.</b> Play a selected card from your deck.", "Visionary", Rarity.Epic);
        cardToAdd.AddAbility(new SelectCardFromDecksAbility(1, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Helpful Witch", 2, 0, true, "<b>Spy.</b> Show 3 random cards from your deck and play one of them.", "HelpfulWitch", Rarity.Rare);
        cardToAdd.AddAbility(new ShowAndPlayCardsFromDeckAbility(3));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Cunning Witch", 3, 0, false, "Show 3 random cards. Create and play one of them.", "CunningWitch", Rarity.Rare);
        cardToAdd.AddAbility(new ShowRandomCardsAndPlayOneAbility(3));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Street Magician", 2, 0, false, "Replay one of your cards.", "StreetMagician", Rarity.Common);
        cardToAdd.AddAbility(new ReplayCardAbility());
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, false, 1, true, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Alchemist", 2, 0, true, "<b>Spy.</b> Choose a card from the board, and play a copy of it from your deck.", "Alchemist", Rarity.Common);
        cardToAdd.AddAbility(new PlayCardFromDeckAbility());
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Illusionist", 5, 0, true, "<b>Spy.</b> Choose a card from the board, create and play a base copy of it.", "Illusionist", Rarity.Common);
        cardToAdd.AddAbility(new CreateACopyOfCardInDeckAbility(1, true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Nice Sorceress", 11, 0, false, "Draw 3 cards. Shuffle 3 cards back into your deck.", "NiceSorceress", Rarity.Common);
        cardToAdd.AddAbility(new MulliganCardsAbility(3, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Greedy Warlock", 11, 0, false, "Draw 3 cards. Discard 3 cards from your hand to the graveyard.", "GreedyWarlock", Rarity.Common);
        cardToAdd.AddAbility(new MulliganCardsAbility(3, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Necromancer", 11, 0, false, "Shuffle up to 3 cards from your graveyard into your deck.", "Necromancer", Rarity.Common);
        cardToAdd.AddAbility(new SelectCardFromGraveyardAbility(3, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Honest Wizard", 7, 0, false, "Each time you draw a card, other than at the start of your turn, boost me by 1.", "HonestWizard", Rarity.Common);
        cardToAdd.AddAbility(new BoostMeOnCardDrawAbility(1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Old Sorcerer", 10, 0, false, "If you have more than 5 cards in your hand, boost me by 2 for each excess card.", "OldSorcerer", Rarity.Common);
        cardToAdd.AddAbility(new BoostMeForCardsInHandAbility(5, 2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Clever Enchanter", 11, 0, true, "<b>Spy.</b> You may play 2 additional cards.", "CleverEnchanter", Rarity.Legendary);
        cardToAdd.AddAbility(new PlayAdditionalCardAbility(2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Student of Witchcraft", 5, 0, false, "When you mulligan or discard me from your hand, summon me to a random row.", "StudentOfWitchcraft", Rarity.Rare);
        cardToAdd.AddAbility(new AutoPlayMeOnMillganOrDiscardAbility());
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Pleasant Sage", 6, 0, false, "<b>Row 1:</b> Increase the HP of both players by 10.\r\n<b>Row 2:</b> Boost me by 5.\r\n<b>Row 3:</b> Reduce the HP of both players by 10.", "PleasantSage", Rarity.Rare);
        cardToAdd.AddAbility(new IncreaseOrDecreasePointsToWinAbility(10, 5));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Summoner", 2, 0, true, "<b>Spy.</b> <b>Choose one:</b>\r\nCreate and play <b>Ice Giant.</b>\r\nCreate and play <b>Great Golem.</b>\r\nCreate and play <b>Elder Samurai.</b>", "Summoner", Rarity.Rare);
        cardToAdd.AddAbility(new ShowCardsPlayOneAbility(new int[] { 28, 24, 40 })); // ZALE¯NE OD ID!
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Madwoman", 4, 0, false, "<b>Choose one:</b>\r\nBoost me by 8.\r\nBoost me by a random amount from 0 to 18.", "Madwoman", Rarity.Rare);
        cardToAdd.AddAbility(new ChooseBostMeByRandomNumberAbility(8, 0, 18));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Archmage", 4, 0, false, "If there are no duplicates in your starting deck, boost me by 1 for every card in your starting deck.", "Archmage", Rarity.Legendary);
        cardToAdd.AddAbility(new BoostMeIfDeckConditionsAreMetAbility(1, 1, 1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Conjurer", 4, 0, false, "If your starting deck does not have more than 2 copies of any card, boost me by 1 for every 2 cards in your starting deck.", "Conjurer", Rarity.Epic);
        cardToAdd.AddAbility(new BoostMeIfDeckConditionsAreMetAbility(2, 1, 2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Time Whisperer", 4, 0, false, "Boost me by 2. <b>Cooldown 1.</b>\r\n<b>Delay 7:</b> Instead of boosting, deal 1 damage to me from now on.", "TimeWhisperer", Rarity.Epic);
        cardToAdd.AddAbility(new CooldownBoostMeOrDealDamageToMeAbility(2, 7));
        cardList.Add(cardToAdd);

        ///////////////////////////////////////////////////////////////////////////////// Agro

        cardToAdd = new Card(id++, "Great Golem", 14, 0, false, "<i>none</i>", "GreatGolem", Rarity.Common);
        // no ability
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Chummy Pebble", 3, 0, false, "You may play 1 additional card.", "ChummyPebble", Rarity.Common);
        cardToAdd.AddAbility(new PlayAdditionalCardAbility(1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Selfish Troll", 16, 0, false, "Discard a card from your hand.", "SelfishTroll", Rarity.Common);
        cardToAdd.AddAbility(new DiscardCardsAbility(1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Warlike Troll", 10, 0, false, "Reduce the HP of both players by 6.", "WarlikeTroll", Rarity.Rare);
        cardToAdd.AddAbility(new ChangePointsToWinAbility(-6));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Ice Giant", 18, 0, false, "Damage me by 1. <b>Cooldown 1.</b>", "IceGiant", Rarity.Common);
        cardToAdd.AddAbility(new CooldownBoostMeAbility(-1, 1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "The Irresistible", 6, 0, false, "Boost me by 1. <b>Cooldown 1.</b>\r\nIf it's round 14 or later, kill all your cards.", "TheIrresistible", Rarity.Legendary);
        cardToAdd.AddAbility(new CooldownBoostMeOrDestroyBoardAbility(1, 14));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Cyclops", 5, 0, false, "Discard any number of cards from your hand. Boost me by 3 for each one.", "Cyclops", Rarity.Epic);
        cardToAdd.AddAbility(new DiscrardCardsToBoostMeAbility(3, 10));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Falling Giant", 19, 0, false, "Damage me by current round number.", "FallingGiant", Rarity.Common);
        cardToAdd.AddAbility(new BoostMeByTurnNumberAbility(-1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Aggressive Giant", 4, 0, false, $"Deal 12 damage to a card. Damage is reduced by 1 each round.", "AggressiveGiant", Rarity.Rare);
        cardToAdd.AddAbility(new DealDamageReducedByTurnNumberAbility(12));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd); // Przy zmienie nazwy karty lub obra¿eñ zmieniæ odœwierzanie opisu w TurnLogic!

        cardToAdd = new Card(id++, "Construct", 18, 0, false, "Damage me by 1 for each card in your deck.", "Construct", Rarity.Rare);
        cardToAdd.AddAbility(new ChangeMyPowerByCartsInYourDeckAbility(-1, 1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Troll Mage", 1, 0, false, "Reveal 2 random cards from your deck and play one of them.", "TrollMage", Rarity.Common);
        cardToAdd.AddAbility(new ShowAndPlayCardsFromDeckAbility(2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Troll Hunter", 3, 0, false, "Play a random card from your deck.", "TrollHunter", Rarity.Common);
        cardToAdd.AddAbility(new PlayRandomCardsAbility());
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Troll Knight", 16, 0, false, "Your opponent draws a card.", "TrollKnight", Rarity.Common);
        cardToAdd.AddAbility(new DrawCardsAbility(1, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Troll Artist", 2, 0, false, "At the start of your turn, if I'm in your deck or hand, summon 1 random copy of me.", "TrollArtist", Rarity.Epic);
        cardToAdd.AddAbility(new AutoPlayMeFromDeckOrHandAbility());
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Forest Orc", 2, 0, false, "Boost me by 1 for every 6 of your HP.", "ForestOrc", Rarity.Rare);
        cardToAdd.AddAbility(new BoostMeForHpAbility(6, false));
        cardList.Add(cardToAdd);

        ///////////////////////////////////////////////////////////////////////////////// Scaling

        cardToAdd = new Card(id++, "Younger Samurai", 10, 0, false, "Boost me by 1. <b>Cooldown 2.</b>", "YoungerSamurai", Rarity.Common);
        cardToAdd.AddAbility(new CooldownBoostMeAbility(1, 2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Intermediate Samurai", 6, 0, false, "Boost me by 1. <b>Cooldown 1.</b>", "IntermediateSamurai", Rarity.Common);
        cardToAdd.AddAbility(new CooldownBoostMeAbility(1, 1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Elder Samurai", 1, 0, false, "Boost me by 3. <b>Cooldown 2.</b>", "ElderSamurai", Rarity.Common);
        cardToAdd.AddAbility(new CooldownBoostMeAbility(3, 2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Master Samurai", 1, 0, false, "Boost me by current round number.", "MasterSamurai", Rarity.Rare);
        cardToAdd.AddAbility(new BoostMeByTurnNumberAbility(1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Patient Disciple", 9, 0, false, "<b>Delay 4:</b> Boost me by 8.", "PatientDisciple", Rarity.Common);
        cardToAdd.AddAbility(new BoostMeAbility(8));
        cardToAdd.AddAbility(new DelayAbility(4));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Patient Master", 3, 0, false, "<b>Delay 4:</b> Boost me by 16.", "PatientMaster", Rarity.Common);
        cardToAdd.AddAbility(new BoostMeAbility(16));
        cardToAdd.AddAbility(new DelayAbility(4));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "The Ascended", 1, 0, false, "<b>Delay 18:</b> Boost me by 99. <b>Invulnerable.</b>", "TheAscended", Rarity.Legendary, true);
        cardToAdd.AddAbility(new BoostMeAbility(99));
        cardToAdd.AddAbility(new DelayAbility(18));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Armorer's Apprentice", 10, 0, false, "Give a card 7 armor.", "ArmorersApprentice", Rarity.Common);
        cardToAdd.AddAbility(new ChangeArmorAbility(7));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Faithful Weaponsmith ", 7, 0, false, "Give a card 14 armor.", "FaithfulWeaponsmith", Rarity.Common);
        cardToAdd.AddAbility(new ChangeArmorAbility(14));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Excellent Blacksmith", 5, 0, false, "Give a card 21 armor.", "ExcellentBlacksmith", Rarity.Common);
        cardToAdd.AddAbility(new ChangeArmorAbility(21));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Honest Armorer", 9, 0, false, "Give a card in hand 6 armor.", "HonestArmorer", Rarity.Rare);
        cardToAdd.AddAbility(new BoostCardInHandAbility(6, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "The Protector", 12, 0, false, "Whenever you play a card to my right, give it my boost and armor, then weaken me by 2.", "TheProtector", Rarity.Epic);
        cardToAdd.AddAbility(new GiveAllMyBonusesAbility(2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "The Defender", 9, 0, false, "I take all damage dealt to your cards in this row.", "TheDefender", Rarity.Epic);
        cardToAdd.AddAbility(new DefenderAbility());
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Calm Swordsman", 10, 0, false, "If it is round 12 or later, boost me by 7.", "CalmSwordsman", Rarity.Common);
        cardToAdd.AddAbility(new BoostMeIfTurnNumberEqualsAbility(12, 7));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Brave Swordsman", 1, 0, false, "Deal damage to the card equal to the current round number.", "BraveSwordsman", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(-1, true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Fearless Swordswoman", 4, 0, false, "Boost me by 1 for every 2 cards in your deck.", "FearlessSwordswoman", Rarity.Rare);
        cardToAdd.AddAbility(new ChangeMyPowerByCartsInYourDeckAbility(1, 2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Crafty Swordswoman", 2, 0, false, "Boost me by 1 for every 6 HP of your opponent.", "CraftySwordswoman", Rarity.Rare);
        cardToAdd.AddAbility(new BoostMeForHpAbility(6, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Peaceful Mistress", 10, 0, false, "Increase the HP of both players by 6.", "PeacefulMistress", Rarity.Rare);
        cardToAdd.AddAbility(new ChangePointsToWinAbility(6));
        cardList.Add(cardToAdd);

        ///////////////////////////////////////////////////////////////////////////////// Movement

        cardToAdd = new Card(id++, "Elven Strategist", 10, 0, false, "Move 2 cards to another row.", "ElvenStrategist", Rarity.Common);
        cardToAdd.AddAbility(new MoveCardAbility());
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true, 2, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Elven General", 6, 0, false, "Move 4 cards to another row.", "ElvenGeneral", Rarity.Common);
        cardToAdd.AddAbility(new MoveCardAbility());
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true, 4, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Elven Princess", 8, 0, false, "Move all other cards from this row to other rows.", "ElvenPrincess", Rarity.Epic);
        cardToAdd.AddAbility(new MoveAllCardsInRowAbility(false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Elven Prince", 7, 0, false, "Move all cards from the opposite row to other rows.", "ElvenPrince", Rarity.Epic);
        cardToAdd.AddAbility(new MoveAllCardsInRowAbility(true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Forest Protector", 9, 0, false, "Boost all other cards in this row by 1.", "ForestProtector", Rarity.Common);
        cardToAdd.AddAbility(new BoostAllCardsOnMyRowAbility(1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Blade Dancer", 4, 0, false, "Deal 2 damage to all cards in the opposite row.", "BladeDancer", Rarity.Common);
        cardToAdd.AddAbility(new DamageAllCardsOnOppositeRowAbility(2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Caring Dryad", 8, 0, false, "When a card appears in this row, boost it by 1.", "CaringDryad", Rarity.Common);
        cardToAdd.AddAbility(new BoostCardWhenPlayedOnMyRowAbility(1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Elven Blacksmith", 5, 0, false, "When a card appears in this row, give it 3 armor.", "ElvenBlacksmith", Rarity.Common);
        cardToAdd.AddAbility(new BoostCardWhenPlayedOnMyRowAbility(3, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Elven Archer", 7, 0, false, "When a card appears in the opposite row, damage it by 2 .", "ElvenArcher", Rarity.Common);
        cardToAdd.AddAbility(new DamageCardWhenPlayedOppositeRowAbility(2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Lady of the Lake", 3, 0, false, "Boost all cards in this row by 1. <b>Cooldown 3.</b>", "LadyOfTheLake", Rarity.Common);
        cardToAdd.AddAbility(new CooldownBoostAllCardsOnThisRowAbility(1, 3));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Lady of the Sword", 3, 0, false, "Deal 1 damage to all cards in the opposite row. <b>Cooldown 3.</b>", "LadyOfTheSword", Rarity.Common);
        cardToAdd.AddAbility(new CooldownDamageAllCardsOnThisRowAbility(1, 3, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Elven Spy", 4, 0, true, "<b>Spy.</b> Deal 1 damage to all cards in this row. <b>Cooldown 2.</b>", "ElvenSpy", Rarity.Rare);
        cardToAdd.AddAbility(new CooldownDamageAllCardsOnThisRowAbility(1, 2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Cheerful Caregiver", 5, 0, false, "Move me to the next row and boost all cards in that row by 1. <b>Cooldown 3.</b>", "CheerfulCaregiver", Rarity.Rare);
        cardToAdd.AddAbility(new BoostAllCardsOnMyRowAbility(1, false));
        cardToAdd.AddAbility(new CooldownMoveMeToNextRowAbility(3));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Agile Archer", 6, 0, false, "Move me to the next row and deal 1 damage to all cards in the opposite row. <b>Cooldown 3.</b>", "AgileArcher", Rarity.Rare);
        cardToAdd.AddAbility(new DamageAllCardsOnOppositeRowAbility(1, false));
        cardToAdd.AddAbility(new CooldownMoveMeToNextRowAbility(3));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Forest Builder", 2, 0, false, "Boost a card and its adjacent cards by 4.", "ForestBuilder", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(4, false, false, false, false, true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Enraged Dryad", 1, 0, false, "Deal 4 damage to a card and its adjacent cards.", "EnragedDryad", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(-4, false, false, false, false, true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Elven Bard", 7, 0, false, "<b>Choose One:</b>\r\nBoost all other cards in this row by 1.\r\nDeal 1 damage to all cards in the opposite row.", "ElvenBard", Rarity.Rare);
        cardToAdd.AddAbility(new ChooseDamageOrBoostRowAbility(1, 1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Cunning Slayer", 75, 0, true, "<b>Spy.</b> Kill all other cards in this row. <b>Invulnerable.</b>", "CunningSlayer", Rarity.Legendary, true);
        cardToAdd.AddAbility(new SetAllCardsInMyRowPowerAbility());
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Ruthless Commander", 5, 0, false, "Set the power of all other cards in this row to 5.", "RuthlessCommander", Rarity.Epic);
        cardToAdd.AddAbility(new SetAllCardsInMyRowPowerAbility(5));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Grovekeeper", 9, 0, false, "Each time your card moves, boost it by 1.", "Grovekeeper", Rarity.Rare);
        cardToAdd.AddAbility(new WhenYourCardMovesBoostItAbility(1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Hidden Archer", 8, 0, false, "Each time your opponent's card moves, damage it by 1.", "HiddenArcher", Rarity.Rare);
        cardToAdd.AddAbility(new WhenEnemyCardMovesDamageItAbility(1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Versatile Elf", 6, 0, false, "Each time your card moves, boost it by 1. Each time your opponent's card moves, deal 1 damage to it.", "VersatileElf", Rarity.Rare);
        cardToAdd.AddAbility(new WhenYourCardMovesBoostItAbility(1));
        cardToAdd.AddAbility(new WhenEnemyCardMovesDamageItAbility(1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Lonely Elf", 4, 0, false, "Boost me by current round number. Reduce boost by 1 for each other card in my row.", "LonelyElf", Rarity.Common);
        cardToAdd.AddAbility(new BoostMeByTurnNumberReductedByCardsInRowAbility(1, 1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Elven Assassin", 5, 0, false, "Damage a card by current round number. Reduce damage by 1 for each other card in its row.", "ElvenAssassin", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(-1, true, false, false, false, false, true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Young Dryad", 9, 0, false, "Boost me by 1 for each other card in this row.", "YoungDryad", Rarity.Common);
        cardToAdd.AddAbility(new BoostMeByTurnNumberReductedByCardsInRowAbility(0, -1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Precision Assassin", 3, 0, false, "Deal damage to a card equal to twice the amount of cards in its row.", "PrecisionAssassin", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(-2, false, false, false, false, false, false, true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Sneaky Charmer", 5, 0, true, "<b>Spy.</b> Each time a card appears on this row, deal 2 damage to me. <b>Revenge:</b> Deal 4 damage to all cards on this row.", "SneakyCharmer", Rarity.Epic);
        cardToAdd.AddAbility(new RevengeChangePowerOfAllCardsInMyRowAbility(-4));
        cardToAdd.AddAbility(new ChangeMyPowerWhenCardsIsPlayedOnMyRow(-2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Rabid Leader", 3, 0, false, "Deal 1 damage to all cards in the opposite row. If any of them die, repeat this ability.", "RabidLeader", Rarity.Legendary);
        cardToAdd.AddAbility(new DamageAllCardsOnOppositeRowAbility(1, true, true));
        cardList.Add(cardToAdd);

        ///////////////////////////////////////////////////////////////////////////////// Swarm

        cardToAdd = new Card(id++, "Boar", 5, 0, false, "Every time a card appears on your side, boost me by 1.", "Boar", Rarity.Common);
        cardToAdd.AddAbility(new BoostCardWhenPlayedOnMySideAbility(1, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Dog", 5, 0, false, "Every time a card appears on your side, boost it by 1.", "Dog", Rarity.Common);
        cardToAdd.AddAbility(new BoostCardWhenPlayedOnMySideAbility(1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Lynx", 2, 0, false, "Boost all your other cards by 1.", "Lynx", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerOfAllCardsOnBoardAbility(1, true, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Puma", 5, 0, false, "Boost all other cards by 2.", "Puma", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerOfAllCardsOnBoardAbility(2, true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Parrot", 1, 0, true, "<b>Spy.</b> Strengthen all your other non-spy entities by 1 wherever they are.", "Parrot", Rarity.Epic);
        cardToAdd.AddAbility(new ChangeBasePowerOfAllCardsAbility(1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Falcon", 7, 0, false, "Boost all your weakest cards by 1. Repeat a total of 2 times.", "Falcon", Rarity.Rare);
        cardToAdd.AddAbility(new BoostAllYourWeakestCardsAbility(1, 2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Owl", 6, 0, false, "Boost all your weakest cards by 1. <b>Cooldown 3.</b>", "Owl", Rarity.Rare);
        cardToAdd.AddAbility(new CooldownBoostAllYourWeakestCardsAbility(1, 3));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Magpie", 5, 0, false, "Boost a card and all cards with the same power on that side by 2.", "Magpie", Rarity.Rare);
        cardToAdd.AddAbility(new ChangPowerOfCardAndAllCardsWithTheSamePowerAbility(2));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Seagull", 4, 0, false, "Boost a card and all copies of it on the same side by 3.", "Seagull", Rarity.Rare);
        cardToAdd.AddAbility(new ChangePowerAbility(3, false, true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Golden Rooster", 5, 0, false, "Whenever a card with a power less than 5 appears in my row, set its power to 5.", "GoldenRooster", Rarity.Legendary);
        cardToAdd.AddAbility(new BoostCardWhenPlayedOnMyRowAbility(5, false, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Cow", 6, 0, false, "Create my base copy.", "Cow", Rarity.Common);
        cardToAdd.AddAbility(new SpawnCardAbility(0));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Crow", 5, 0, false, "Summon 2 my copies from your deck.", "Crow", Rarity.Common);
        cardToAdd.AddAbility(new SummonMyCopyFromDeckAbility(2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Squirrel", 5, 0, false, "<b>Delay 2:</b> Summon a copy of me from your deck.", "Squirrel", Rarity.Common);
        cardToAdd.AddAbility(new SummonMyCopyFromDeckAbility(1, false));
        cardToAdd.AddAbility(new DelayAbility(2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Wolf Pack", 4, 0, false, "Boost all other <b>Wolf Pack</b> on your side by 4.", "WolfPack", Rarity.Common);
        cardToAdd.AddAbility(new BoostEveryOtherMeAbility(4, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Bear", 2, 0, false, $"Boost me by 0. The boost increases by 3 for each <b>Bear</b> you play this game.", "Bear", Rarity.Common);
        cardToAdd.AddAbility(new BoostEveryOtherMeAbility(3, true));
        cardList.Add(cardToAdd);  // Przy zmienie boosta lub nazwy zmieniæ odœwierzanie opisu w BoostEveryOtherMeAbility i CardRenderer.RenderCard!

        cardToAdd = new Card(id++, "Salamander", 11, 0, false, "Choose a card and create a copy of it in your deck.", "Salamander", Rarity.Common);
        cardToAdd.AddAbility(new CreateACopyOfCardInDeckAbility(1));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Chameleon", 8, 0, false, "Choose a card and create 2 copies of it in your deck.", "Chameleon", Rarity.Rare);
        cardToAdd.AddAbility(new CreateACopyOfCardInDeckAbility(2));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Anglerfish", 11, 0, false, "Shuffle a selected card from your graveyard into your deck along with all its copies.", "Anglerfish", Rarity.Epic);
        cardToAdd.AddAbility(new SelectCardFromGraveyardAbility(1, true, true));
        cardList.Add(cardToAdd);

        ///////////////////////////////////////////////////////////////////////////////// Cmentarz

        cardToAdd = new Card(id++, "Egg", 5, 0, false, "<b>Revenge:</b> Create <b>Chick</b> in this row.", "Egg", Rarity.Common);
        cardToAdd.AddAbility(new RevengeSpawnCardAbility());
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Chick", 12, 0, false, "<i>none</i>", "Chick", Rarity.None);
        // no ability
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Larva", 2, 0, false, "Create and play one more <b>Larva</b>. <b>Revenge:</b> Create <b>Worms</b> in this row.", "Larva", Rarity.Common);
        cardToAdd.AddAbility(new CreateAndPlayCardAbility());
        cardToAdd.AddAbility(new RevengeSpawnCardAbility());
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Worms", 7, 0, false, "<i>none</i>", "Worms", Rarity.None);
        // no ability
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Mutant Lizard", 8, 0, false, "<b>Revenge:</b> Create a <b>Venom</b> in your hand.", "MutantLizard", Rarity.Common);
        cardToAdd.AddAbility(new RevengeCreateCardInHandAbility());
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Venom", 0, 0, false, "Deal 7 damage to a card. You may play 1 additional card.", "Venom", Rarity.None);
        cardToAdd.AddAbility(new ChangePowerAbility(-7));
        cardToAdd.AddAbility(new PlayAdditionalCardAbility(1));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Giant's Egg", 11, 0, false, "<b>Revenge:</b> Create a <b>Baby Giant<b> in your hand.", "GiantsEgg", Rarity.Common);
        cardToAdd.AddAbility(new RevengeCreateCardInHandAbility());
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Baby Giant", 19, 0, false, "<i>none</i>", "BabyGiant", Rarity.None);
        // no ability
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Insidious Vampire", 7, 0, false, "<b>Revenge:</b> Resurrect me without this ability.", "InsidiousVampire", Rarity.Epic);
        cardToAdd.AddAbility(new RevengeResMeAbility(false, true));
        cardList.Add(cardToAdd); // Przy zmienie nazwy zmieniæ odœwierzanie opisu w CardManager.DrawCard i CardRenderer.RenderCard!

        cardToAdd = new Card(id++, "Higher Vampire", 8, 0, false, "<b>Revenge:</b> Reduce my base power by half and resurrect me.", "HigherVampire", Rarity.Epic);
        cardToAdd.AddAbility(new RevengeResMeAbility(true, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Shadows of Darkness", 2, 0, false, "Deal 1 damage to your opponent's strongest card. <b>Cooldown 1.</b> <b>Revenge:</b> Deal 6 damage to your opponent's strongest card.", "ShadowsOfDarkness", Rarity.Rare);
        cardToAdd.AddAbility(new CooldownDamageStrongestEnemyCardsAbility(1, 1, true));
        cardToAdd.AddAbility(new RevengeDamageStrongestEnemyCardAbility(6));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Foglet", 4, 0, false, "<b>Revenge:</b> Summon a copy of me from your deck.", "Foglet", Rarity.Rare);
        cardToAdd.AddAbility(new RevengeSummonMyCopyFromDeckAbility());
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Goblin", 5, 0, false, "Resurrect all my copies from your graveyard.", "Goblin", Rarity.Rare);
        cardToAdd.AddAbility(new SummonAllMyCopiesFromGraveyardAbility());
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Disgusting Grub", 11, 0, false, "Kill cards to the right and left and boost me with their power.", "DisgustingGrub", Rarity.Common);
        cardToAdd.AddAbility(new ConsumeCardsAbility(false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Giant Worm", 7, 0, false, "Kill all other cards in this row and boost me with their power.", "GiantWorm", Rarity.Common);
        cardToAdd.AddAbility(new ConsumeCardsAbility(true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Massive Devourer", 8, 0, false, "Kill the card to my right and boost me by its power. <b>Cooldown 3.</b>", "MassiveDevourer", Rarity.Common);
        cardToAdd.AddAbility(new CooldownConsumeCardsAbility(3));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Vran Warrior", 9, 0, false, "Kill your card and deal damage equal to its power to a selected card.", "VranWarrior", Rarity.Rare);
        cardToAdd.AddAbility(new KillYourCardAndDealDamageAbility());
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, false, 2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Werewolf", 6, 0, false, "Select your card and boost me by its base power minus 10.", "Werewolf", Rarity.Rare);
        cardToAdd.AddAbility(new ChangeMyBasePowerAbility(10));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Queen of Worms", 9, 0, false, "<b>Choose one:</b>\r\nKill a card to your right and left and boost me with their power.\r\nDeal 4 damage to me, then create and play one <b>Larva<b>.", "QueenOfWorms", Rarity.Rare);
        cardToAdd.AddAbility(new ChooseConsumeOrSpawnEggAbility(105)); // ZALE¯NE OD ID!
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "The Unbridled", 40, 0, true, "<b>Spy.</b> Kill all other cards. <b>Invulnerable.</b>", "TheUnbridled", Rarity.Legendary, true);
        cardToAdd.AddAbility(new DestroyBoardAbility());
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Spider Mother", 7, 0, false, "Every time your card dies, create <b>Spider<b> on its row.", "SpiderMother", Rarity.Epic);
        cardToAdd.AddAbility(new WhenOtherCardDieSpawnCardAbility());
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Spider", 2, 0, false, "<i>none</i>", "Spider", Rarity.None);
        // no ability
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Wild Beast", 9, 0, false, "Strengthen me by 3.", "WildBeast", Rarity.Common);
        cardToAdd.AddAbility(new StrengthenMeAbility(3));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Little Imp", 7, 0, false, "Strengthen a card by 4.", "LittleImp", Rarity.Common);
        cardToAdd.AddAbility(new ChangeBasePowerAbility(4));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Elder Imp", 4, 0, false, "Strengthen 2 cards by 3.", "ElderImp", Rarity.Common);
        cardToAdd.AddAbility(new ChangeBasePowerAbility(3));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true, 2, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Old Witch", 2, 0, false, "Resurrect a card from your graveyard.", "OldWitch", Rarity.Rare);
        cardToAdd.AddAbility(new SelectCardFromGraveyardAbility(1, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Succubus", 11, 0, false, "Discard up to 3 cards from your deck to the graveyard.", "Succubus", Rarity.Common);
        cardToAdd.AddAbility(new SelectCardFromDecksAbility(3, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Unleashed Demon", 20, 0, true, "<b>Spy.</b> Set a card's base power to its current power.", "UnleashedDemon", Rarity.Legendary);
        cardToAdd.AddAbility(new ChangeBasePowerAbility(0, true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Wyvern", 9, 0, false, "Discard a non-spy card from your hand and boost me by half its power.", "Wyvern", Rarity.Common);
        cardToAdd.AddAbility(new DiscrardCardsToBoostMeAbility(0, 1, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Griffin", 6, 0, false, "Banish a non-spy card from your graveyard and boost me by half its power.", "Griffin", Rarity.Common);
        cardToAdd.AddAbility(new SelectCardFromGraveyardAbility(1, false, false, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Corpse Eater", 3, 0, false, "Banish up to 10 entities from your graveyard and boost me by 2 for each one.", "CorpseEater", Rarity.Epic);
        cardToAdd.AddAbility(new SelectCardFromGraveyardAbility(10, false, false, true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Beast of Doom", 6, 0, false, "Deal 2 damage to all other cards.", "BeastOfDoom", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerOfAllCardsOnBoardAbility(-2, true, true));
        cardList.Add(cardToAdd);

        ///////////////////////////////////////////////////////////////////////////////// Kontrola

        cardToAdd = new Card(id++, "Axewoman", 8, 0, false, "Deal 4 damage to a card.", "Axewoman", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(-4));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Young Axeman", 5, 0, false, "Deal 6 damage to a card.", "YoungAxeman", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(-6));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Experienced Axeman", 2, 0, false, "Deal 8 damage to a card.", "ExperiencedAxeman", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(-8));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Powerful Strike", 0, 0, false, "Deal 10 damage to a card.", "PowerfulStrike", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(-10));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Sneaky Axeman", 2, 0, true, "<b>Spy.</b> Deal 13 damage to a card.", "SneakyAxeman", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(-13));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Solid Warrior", 2, 0, false, "Deal 7 damage to a card. Boost me for the excess damage.", "SolidWarrior", Rarity.Rare);
        cardToAdd.AddAbility(new ChangePowerAbility(-7, false, false, false, true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Ruthless Warrior", 6, 0, false, "Deal 3 damage to a card. If it is killed, boost me by 5.", "RuthlessWarrior", Rarity.Rare);
        cardToAdd.AddAbility(new DamageCardAndBoostMeIfKilledAbility(3, 5));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Persevering Warrior", 3, 0, false, "Deal 5 damage to a card. If it survives, boost me by 5.", "PerseveringWarrior", Rarity.Rare);
        cardToAdd.AddAbility(new DamageCardAndBoostMeIfKilledAbility(5, 5, true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Precise Bowman", 7, 0, false, "Deal 1 damage 4 times.", "PreciseBowman", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(-1));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true, 4));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Shield-maiden", 6, 0, false, "Deal 3 damage to 2 enemies.", "ShieldMaiden", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(-3));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true, 2, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Grimaxe Raider", 3, 0, false, "Deal 3 damage to a card and all of its cards of the same power on the same side.", "GrimaxeRaider", Rarity.Common);
        cardToAdd.AddAbility(new ChangPowerOfCardAndAllCardsWithTheSamePowerAbility(-3));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Frostblade Knight", 6, 0, false, "Deal 3 damage to a card and all copies of it on the same side.", "FrostbladeKnight", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(-3, false, true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Whitebeard", 3, 0, false, "Reduce a card power by half.", "Whitebeard", Rarity.Rare);
        cardToAdd.AddAbility(new ChangePowerAbility(-50, false, false, true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Solitary", 5, 0, false, "Reset a card power to its base power.", "Solitary", Rarity.Rare);
        cardToAdd.AddAbility(new ResetCardPowerAbility());
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Viking King", 6, 0, false, "Set a card power to 11.", "VikingKing", Rarity.Rare);
        cardToAdd.AddAbility(new SetCardPowerAbility(11));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Golden Sword", 7, 0, false, "Block a card.", "GoldenSword", Rarity.Epic);
        cardToAdd.AddAbility(new BlockCardAbility());
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Spectral Falchion", 22, 0, true, "<b>Spy.</b> Swap the power of two cards.", "SpectralFalchion", Rarity.Epic);
        cardToAdd.AddAbility(new SwapTwoCardPowerAbility());
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true, 2, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Hypnotist", 1, 0, false, "Move a card with 4 or less power to the opposite row.", "Hypnotist", Rarity.Epic);
        cardToAdd.AddAbility(new MoveCardAbility(4, true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Lord of the Oceans", 1, 0, true, "<b>Spy.</b> Choose a card and boost me by one and a half of its power. Return it to your hand and replay it.", "LordOfTheOceans", Rarity.Legendary);
        cardToAdd.AddAbility(new ReplayCardAbility(true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Relentless Killer", 6, 0, true, "<b>Spy.</b> Kill a card.", "RelentlessKiller", Rarity.Common);
        cardToAdd.AddAbility(new KillCardAbility());
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Crimson Axeman", 2, 0, false, "Kill a card that is not boosted.", "CrimsonAxeman", Rarity.Common);
        cardToAdd.AddAbility(new KillCardAbility(true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Bloodaxe Marauder", 5, 0, false, "Kill a card that is wounded.", "BloodaxeMarauder", Rarity.Common);
        cardToAdd.AddAbility(new KillCardAbility(false, true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Wild Destroyer", 12, 0, true, "<b>Spy.</b> Kill all other strongest cards.", "WildDestroyer", Rarity.Rare);
        cardToAdd.AddAbility(new KillAllStrongestOrWeakestCardsAbility(true, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Flanking Warrior", 6, 0, false, "Kill all the weakest cards.", "FlankingWarrior", Rarity.Rare);
        cardToAdd.AddAbility(new KillAllStrongestOrWeakestCardsAbility(false, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Thunderous Blade", 14, 0, true, "<b>Spy.</b> Kill all of your opponent's wounded cards.", "ThunderousBlade", Rarity.Legendary);
        cardToAdd.AddAbility(new KillAllEnemyWoundedCardsAbility());
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Faithful Legionnaire", 4, 0, false, "Every time a card appears on your opponent's side, deal 1 damage to it.", "FaithfulLegionnaire", Rarity.Common);
        cardToAdd.AddAbility(new DealDamageToCardWhenPlayedAbility(1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Spearmaiden", 5, 0, false, "Deal 1 damage to the opponent's strongest card. <b>Cooldown 1.</b>", "Spearmaiden", Rarity.Common);
        cardToAdd.AddAbility(new CooldownDamageStrongestEnemyCardsAbility(1, 1, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Stormguard Warriors", 7, 0, false, "Damage all of your opponent's strongest cards by 1. Repeat a total of 3 times.", "StormguardWarriors", Rarity.Rare);
        cardToAdd.AddAbility(new DamageAllStrongestEnemyCardsAbility(1, 3));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Emberblade Sentinels", 7, 0, false, "Deal 1 damage to all of your opponent's strongest cards. <b>Cooldown 3.</b>", "EmberbladeSentinels", Rarity.Rare);
        cardToAdd.AddAbility(new CooldownDamageStrongestEnemyCardsAbility(1, 3));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Wild Beard", 3, 0, false, "Deal 1 damage to all of the opponent's cards.", "WildBeard", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerOfAllCardsOnBoardAbility(-1, false, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Bloodthirsty", 6, 0, true, "<b>Spy.</b> <b>Revenge:</b> Deal 6 damage to all cards on this row.", "Bloodthirsty", Rarity.Epic);
        cardToAdd.AddAbility(new RevengeChangePowerOfAllCardsInMyRowAbility(-6));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Silverfang Pirate", 5, 0, false, "Deal 2 damage to all of your opponent's wounded cards.", "SilverfangPirate", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerOfAllCardsOnBoardAbility(-2, false, true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Great Captain", 3, 0, false, "Each time a card in the opposite row is damaged, boost me by 1", "GreatCaptain", Rarity.Common);
        cardToAdd.AddAbility(new BoostMeWhenEnemyCardIsDamagedAbility(1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Ashen Blade", 6, 0, false, "Every time an opponent's card is killed, boost me by 2.", "AshenBlade", Rarity.Common);
        cardToAdd.AddAbility(new BoostMeWhenEnemyCardDiesAbility(2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Courageous Swordswoman", 5, 0, false, "Boost me by 2 for each wounded opponent card.", "CourageousSwordswoman", Rarity.Common);
        cardToAdd.AddAbility(new BoostMeForEveryWoundedCardOnEnemySideAbility(2));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Ironblood Warrior", 12, 0, false, "Boost me by the current round number. Deal 1 damage to me for each of your opponent's cards.", "IronbloodWarrior", Rarity.Common);
        cardToAdd.AddAbility(new BoostMeByTurnNumberAndDagameMeByEnemyCardsOnBoardAbility(1, 1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Golem Slayer", 12, 0, false, "Create a Mystic Golem on the opponent's side.", "GolemSlayer", Rarity.Epic);
        cardToAdd.AddAbility(new SpawnCardAbility(1, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Mystic Golem", 7, 0, false, "<b>Revenge:</b> Boost the weakest Golem Slayer on the opposing side by 15.", "MysticGolem", Rarity.None);
        cardToAdd.AddAbility(new RevengeBoostTheWeakestCardOnEnemySideAbility(171, 15));
        cardList.Add(cardToAdd);

        ///////////////////////////////////////////////////////////////////////////////// Wzmacnianie

        cardToAdd = new Card(id++, "Young Priestess", 6, 0, false, "Boost a card by 6.", "YoungPriestess", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(6));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Dedicated Priestess", 0, 0, false, "Boost a card by 11.", "DedicatedPriestess", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(11));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Worthy Offering", 6, 0, true, "<b>Spy.</b> Boost a card by 16.", "WorthyOffering", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(16));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Elder Priestess", 3, 0, false, "Boost a card by 1/4 of its power.", "ElderPriestess", Rarity.Epic);
        cardToAdd.AddAbility(new ChangePowerByPercentAbility(25f));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Altar Girl", 8, 0, false, "Boost a card by 1 4 times.", "AltarGirl", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(1));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true, 4));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Empathetic Healer", 2, 0, false, "Boost 2 cards by 5.", "EmpatheticHealer", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerAbility(5));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, true, 2, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Lonely Missionary", 7, 0, false, "Boost a card in your hand by 4.", "LonelyMissionary", Rarity.Rare);
        cardToAdd.AddAbility(new BoostCardInHandAbility(4));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Old Friar", 10, 0, false, "Move an entire boost from one of your cards to another.", "OldFriar", Rarity.Rare);
        cardToAdd.AddAbility(new SwapTwoCardPowerAbility(true));
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, false, 2, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Mother of the Order", 11, 0, false, "Swap the power of two of your cards.", "MotherOfTheOrder", Rarity.Epic);
        cardToAdd.AddAbility(new SwapTwoCardPowerAbility());
        cardToAdd.AddAbility(new StartBoardSelectionAbility(true, false, 2, false));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Divine Messenger", 5, 0, false, "Boost all your boosted cards by 1.", "DivineMessenger", Rarity.Common);
        cardToAdd.AddAbility(new ChangePowerOfAllCardsOnBoardAbility(1, true, false, false, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Monastic Father", 5, 0, false, "Boost your weakest card by 1. <b>Cooldown 1.</b>", "MonasticFather", Rarity.Common);
        cardToAdd.AddAbility(new CooldownBoostAllYourWeakestCardsAbility(1, 1, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Clairvoyant", 5, 0, false, "Boost the card to the right by 1. <b>Cooldown 1.</b>", "Clairvoyant", Rarity.Common);
        cardToAdd.AddAbility(new CooldownBoostCardAbility(1, 1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Mythical Companion", 5, 0, false, "Boost me by 1 for each boosted card on your side.", "MythicalCompanion", Rarity.Common);
        cardToAdd.AddAbility(new BoostMeForEveryBoostedCardOnYourSideAbility(1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Spiritual Serpent", 8, 0, false, "Each time I get a boost, deal 1 damage to the strongest enemy.", "SpiritualSerpent", Rarity.Common);
        cardToAdd.AddAbility(new DealDamageWhenIAmBoostedAbility(1));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "River Idol", 7, 0, false, "When I become boosted, deal 10 damage to the strongest enemy card.", "RiverIdol", Rarity.Rare);
        cardToAdd.AddAbility(new DealDamageWhenIAmBoostedAbility(10, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Azure Whale", 2, 0, false, "When I become boosted, deal damage to the strongest enemy card equal to the boost amount.", "AzureWhale", Rarity.Rare);
        cardToAdd.AddAbility(new DealDamageWhenIAmBoostedAbility(0, true, true));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Kind-hearted Dragon", 10, 0, false, "The first time my power becomes 30 or higher, deal 9 damage to the strongest enemy card.", "KindHeartedDragon", Rarity.Rare);
        cardToAdd.AddAbility(new DealDamageWhenIAmBoostedAbility(9, false, false, 30));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Divine Unicorn", 4, 0, false, "<b>Delay 4:</b> Increase my power by half.", "DivineUnicorn", Rarity.Legendary);
        cardToAdd.AddAbility(new BoostMeAbility(0, true));
        cardToAdd.AddAbility(new DelayAbility(4));
        cardList.Add(cardToAdd);

        cardToAdd = new Card(id++, "Ancient Treebeard", 35, 0, true, "<b>Spy.</b> Kill all cards that are not boosted. <b>Invulnerable.</b>", "AncientTreebeard", Rarity.Epic, true);
        cardToAdd.AddAbility(new DestroyBoardAbility(true));
        cardList.Add(cardToAdd);
    }

    public static Card GetCard(int id)
    {
        var card = cardList.FirstOrDefault(card => card.Id == id);
        if (card == null)
        {
            Debug.LogError($"Karta o ID {id} nie zosta³a znaleziona w bazie.");
        }
        return card;
    }
}