using System;
using TMPro;
using UnityEngine;

public class TutorialLogic : MonoBehaviour
{
    private PlayerDataManager playerDataManager;
    private PlayerAccount playerAccount;
    private DateTime lastOkButtonPress = DateTime.MinValue;
    public int tutorialStage;
    public bool grubHaveToEatEgg = false;
    public int cardForcedToMulliganId = 0;
    public int cardForcedToPlayId = 0;
    public int cardForcedToSelectId = 0;
    public int cardForcedToSelectFromGraveyardId = 0;
    // if 0: no card if forced
    // if -1: player cannot mulligan / play any card
    // if N: player can only mulligan / play card of id N

    [SerializeField] private int tutorialPhase = 0;

    [SerializeField] private PlayerResources playerResources;

    [SerializeField] private GameObject tutorialPanel1;
    [SerializeField] private TextMeshProUGUI tutorialPanel1Text;
    [SerializeField] private GameObject tutorialPanel1Button;
    [SerializeField] private GameObject tutorialPanel2;
    [SerializeField] private TextMeshProUGUI tutorialPanel2Text;
    [SerializeField] private GameObject tutorialPanel2Button;
    [SerializeField] private GameObject endMulliganBlocker;
    [SerializeField] private GameObject endRoundBlocker;

    [SerializeField] private GameObject[] Arrows;

    public bool IsThisTutorial()
    {
        playerAccount = new PlayerAccount();
        playerDataManager = new PlayerDataManager(playerAccount);
        playerDataManager.LoadFromLocal();
        tutorialStage = playerAccount.CurrentTutorialStage;

        if (tutorialStage > 0) return true;

        return false;
    }

    public void SetUpDecks(PlayerResources playerResources)
    {
        if (tutorialStage == 1 || tutorialStage == 2 || playerResources.isAI) playerResources.deckManager.SetUpDeckForTutorial(playerResources, tutorialStage);
        else if (tutorialStage == 3) playerResources.deckManager.FillDeckWithCardsFromCode("10610810B10O10Q10S11811A11P11U12I33113513D13F13M13P13U14014414A14D14G15815B15H15O15Q", playerResources);
    }

    public void PlayTutorial()
    {
        switch (tutorialStage)
        {
            case 1:
                PlayTutorial1();
                break;

            case 2:
                PlayTutorial2();
                break;

            case 3:
                PlayTutorial3();
                break;
        }
    }

    private void PlayTutorial1()
    {
        switch (tutorialPhase)
        {
            case 0:
                tutorialPanel1.SetActive(true);
                endMulliganBlocker.SetActive(true);
                cardForcedToMulliganId = -1;
                cardForcedToPlayId = -1;
                tutorialPanel1Text.text = "Welcome to the first game tutorial!";
                break;
            case 1:
                Arrows[0].SetActive(true);
                tutorialPanel1Text.text = "At the start of the game, each player draws 6 cards from their deck. These are your cards.";
                break;
            case 2:
                Arrows[0].SetActive(false);
                tutorialPanel1Text.text = "The game begins with a mulligan faze where you can exchange up to 3 unwanted cards.";
                break;
            case 3:
                cardForcedToMulliganId = 124;
                tutorialPanel1Button.SetActive(false);
                tutorialPanel1Text.text = "I see you drew 2 <b>Spiders</b> that are very weak. Mulligan them.";
                break;
            case 4:
                break;
            case 5:
                endMulliganBlocker.SetActive(false);
                tutorialPanel1Text.text = "The rest of the cards are fine. Click the <b>End Mulligan</b> button to end the mulligan phase.";
                break;
            case 6:
                endRoundBlocker.SetActive(true);
                tutorialPanel1.SetActive(false);
                tutorialPanel2.SetActive(true);
                tutorialPanel2Text.text = "I will now explain to you each of the visible sections.";
                break;
            case 7:
                Arrows[1].SetActive(true);
                Arrows[2].SetActive(true);
                tutorialPanel2Text.text = "At the top and bottom of the screen you can see your cards and those of your opponent.";
                break;
            case 8:
                Arrows[1].SetActive(false);
                Arrows[2].SetActive(false);
                Arrows[3].SetActive(true);
                tutorialPanel2Text.text = "The centerpiece of the table is the place for the played cards. The bottom three rows contain the cards you have played, and the top three rows contain the cards your opponent has played.";
                break;
            case 9:
                Arrows[3].SetActive(false);
                Arrows[4].SetActive(true);
                Arrows[5].SetActive(true);
                cardForcedToPlayId = 24;
                tutorialPanel2Button.SetActive(false);
                tutorialPanel2Text.text = "This number is each player's total points. You haven't played any cards yet, so your points are 0. Now play a <b>Great Golem</b> card to any row.";
                break;
            case 10:
                cardForcedToPlayId = -1;
                tutorialPanel2Button.SetActive(true);
                tutorialPanel2Text.text = "Great! Notice that your points are now 14.";
                break;
            case 11:
                Arrows[4].SetActive(false);
                Arrows[5].SetActive(false);
                Arrows[6].SetActive(true);
                endRoundBlocker.SetActive(false);
                tutorialPanel2Button.SetActive(false);
                tutorialPanel2Text.text = "You can only play one card on your turn. Click the blue coin to end your turn.";
                break;
            case 12:
                Arrows[6].SetActive(false);
                endRoundBlocker.SetActive(true);
                tutorialPanel2Button.SetActive(true);
                tutorialPanel2Text.text = "Your opponent also played a card and ended their turn.";
                break;
            case 13:
                tutorialPanel2Text.text = "At the beginning of each turn, a player draws one card from his deck.";
                break;
            case 14:
                Arrows[7].SetActive(true);
                Arrows[8].SetActive(true);
                tutorialPanel2Text.text = "The largest number in the corner of the screen is yours and your opponent's HP.";
                break;
            case 15:
                tutorialPanel2Text.text = "To win the game you must reduce your opponent's HP to 0.";
                break;
            case 16:
                tutorialPanel2Text.text = "Each card played both reduces your opponent's HP and increases your HP.";
                break;
            case 17:
                cardForcedToPlayId = 24;
                tutorialPanel2Button.SetActive(false);
                tutorialPanel2Text.text = "Play another <b>Great Golem</b> and notice how yours and your opponent's HP changes.";
                break;
            case 18:
                Arrows[7].SetActive(false);
                Arrows[8].SetActive(false);
                cardForcedToPlayId = -1;
                endRoundBlocker.SetActive(false);
                tutorialPanel2Text.text = "Great. Now end your turn.";
                break;
            case 19:
                endRoundBlocker.SetActive(true);
                tutorialPanel2Button.SetActive(true);
                tutorialPanel2Text.text = "Some cards perform their ability when played.";
                break;
            case 20:
                tutorialPanel2Text.text = "You just drew a <b>Falling Giant</b>. It's very strong, but when played it deals damage to itself equal to the current round number.";
                break;
            case 21:
                Arrows[9].SetActive(true);
                tutorialPanel2Text.text = "It is currently the third round which you can see here.";
                break;
            case 22:
                Arrows[9].SetActive(false);
                cardForcedToPlayId = 31;
                tutorialPanel2Button.SetActive(false);
                tutorialPanel2Text.text = "Play <b>Falling Giant</b> before it's too late!";
                break;
            case 23:
                cardForcedToPlayId = -1;
                endRoundBlocker.SetActive(false);
                tutorialPanel2Text.text = "Great, that's 16 points! Now end your turn.";
                break;
            case 24:
                endRoundBlocker.SetActive(true);
                tutorialPanel2Button.SetActive(true);
                tutorialPanel2Text.text = "<b>Warlike Troll</b> is a very aggressive card that strongly reduces the opponent's HP, but also yours!";
                break;
            case 25:
                cardForcedToPlayId = 27;
                tutorialPanel2Button.SetActive(false);
                tutorialPanel2Text.text = "We are currently winning, so let's play this card.";
                break;
            case 26:
                cardForcedToPlayId = -1;
                tutorialPanel2Button.SetActive(true);
                tutorialPanel2Text.text = "At the end of each round, the HP of two players is automatically reduced by 3.";
                break;
            case 27:
                tutorialPanel2Button.SetActive(false);
                endRoundBlocker.SetActive(false);
                tutorialPanel2Text.text = "End the turn and watch your and your opponent's HP decrease as your opponent ends their turn.";
                break;
            case 28:
                tutorialPanel2Button.SetActive(true);
                endRoundBlocker.SetActive(true);
                tutorialPanel2Text.text = "Our opponent has very little HP left. Finish him off!";
                break;
            case 29:
                cardForcedToPlayId = 25;
                tutorialPanel2Button.SetActive(false);
                tutorialPanel2Text.text = "Play two <b>Chummy Pebbles</b> and finally the <b>Troll Knight</b>.";
                break;
            case 30:
                break;
            case 31:
                cardForcedToPlayId = 36;
                break;
            case 32:
                cardForcedToPlayId = -1;
                endRoundBlocker.SetActive(false);
                tutorialPanel2Text.text = "Great! Your opponent's HP has dropped below 1, which means you've won!";
                CompleteTutorialStage(1);
                break;
        }
        tutorialPhase++;
    }

    private void PlayTutorial2()
    {
        switch (tutorialPhase)
        {
            case 0:
                tutorialPanel1.SetActive(true);
                endMulliganBlocker.SetActive(true);
                cardForcedToMulliganId = -1;
                cardForcedToPlayId = -1;
                cardForcedToSelectId = -1;
                tutorialPanel1Text.text = "In this tutorial you will learn about many new cards and their keywords.";
                break;
            case 1:
                tutorialPanel1Button.SetActive(false);
                cardForcedToMulliganId = 24;
                tutorialPanel1Text.text = "To start, mulligan all cards that have no ability.";
                break;
            case 2:
                cardForcedToMulliganId = 106;
                break;
            case 3:
                cardForcedToMulliganId = 124;
                break;
            case 4:
                cardForcedToMulliganId = -1;
                tutorialPanel1.SetActive(false);
                tutorialPanel2.SetActive(true);
                endMulliganBlocker.SetActive(false);
                endRoundBlocker.SetActive(true);
                tutorialPanel2Text.text = "If a card does not specify when it performs its ability, it means it triggers it immediately after being played.";
                break;
            case 5:
                cardForcedToPlayId = 95;
                tutorialPanel2Button.SetActive(false);
                tutorialPanel2Text.text = "Play a <b>Cow</b>.";
                break;
            case 6:
                cardForcedToPlayId = -1;
                endRoundBlocker.SetActive(false);
                tutorialPanel2Text.text = "Great! Your cow has instantly created a copy of itself. Now end your turn.";
                break;
            case 7:
                endRoundBlocker.SetActive(true);
                tutorialPanel2Button.SetActive(true);
                tutorialPanel2Text.text = "Some cards also require you to select a target.";
                break;
            case 8:
                cardForcedToPlayId = 173;
                tutorialPanel2Button.SetActive(false);
                tutorialPanel2Text.text = "Play <b>Young Priestess</b> and boost one of your cows.";
                break;
            case 9:
                cardForcedToPlayId = -1;
                cardForcedToSelectId = 95;
                break;
            case 10:
                cardForcedToSelectId = -1;
                endRoundBlocker.SetActive(false);
                tutorialPanel2Text.text = "Notice that the cow's strength is now green which means it is boosted. Now end your turn.";
                break;
            case 11:
                endRoundBlocker.SetActive(true);
                tutorialPanel2Button.SetActive(true);
                tutorialPanel2Text.text = "If a card has an <b>X</b> instead of a power, it means it is a <b>Spell</b>, not an <b>Entity</b>.";
                break;
            case 12:
                cardForcedToPlayId = 138;
                tutorialPanel2Button.SetActive(false);
                tutorialPanel2Text.text = "Play <b>Powerful Strike</b> and destroy one of the opponent's <b>Worms</b>.";
                break;
            case 13:
                cardForcedToPlayId = -1;
                cardForcedToSelectId = 106;
                break;
            case 14:
                cardForcedToSelectId = -1;
                tutorialPanel2Button.SetActive(true);
                Arrows[10].SetActive(true);
                Arrows[11].SetActive(true);
                tutorialPanel2Text.text = "Destroyed entities and used spells are moved to their owner's graveyard.";
                break;
            case 15:
                tutorialPanel2Button.SetActive(false);
                endRoundBlocker.SetActive(false);
                tutorialPanel2Text.text = "At any point during the game you can click on your deck or graveyard to view it. Now end your turn.";
                break;
            case 16:
                tutorialPanel2Button.SetActive(true);
                endRoundBlocker.SetActive(true);
                Arrows[10].SetActive(false);
                Arrows[11].SetActive(false);
                tutorialPanel2Text.text = "Some cards have passive abilities that work continuously as long as the card is on the table.";
                break;
            case 17:
                tutorialPanel2Button.SetActive(false);
                cardForcedToPlayId = 85;
                tutorialPanel2Text.text = "Play <b>Boar</b> and end your turn.";
                break;
            case 18:
                cardForcedToPlayId = -1;
                endRoundBlocker.SetActive(false);
                break;
            case 19:
                endRoundBlocker.SetActive(true);
                tutorialPanel2Button.SetActive(true);
                tutorialPanel2Text.text = "Cards with <b>Cooldown X</b> perform their ability when played and every <b>X</b> rounds.";
                break;
            case 20:
                cardForcedToPlayId = 39;
                tutorialPanel2Button.SetActive(false);
                tutorialPanel2Text.text = "Play <b>Younger Samurai</b> and end your turn.";
                break;
            case 21:
                cardForcedToPlayId = -1;
                endRoundBlocker.SetActive(false);
                break;
            case 22:
                endRoundBlocker.SetActive(true);
                tutorialPanel2Button.SetActive(true);
                tutorialPanel2Text.text = "Cards with the <b>Delay X</b> keyword trigger their ability after <b>X</b> rounds.";
                break;
            case 23:
                cardForcedToPlayId = 44;
                tutorialPanel2Button.SetActive(false);
                tutorialPanel2Text.text = "Play <b>Patient Master</b> and end your turn.";
                break;
            case 24:
                cardForcedToPlayId = -1;
                endRoundBlocker.SetActive(false);
                break;
            case 25:
                endRoundBlocker.SetActive(true);
                tutorialPanel2Button.SetActive(true);
                tutorialPanel2Text.text = "The <b>Revenge</b> keyword means that the card activates its ability when it is destroyed.";
                break;
            case 26:
                cardForcedToPlayId = 103;
                tutorialPanel2Button.SetActive(false);
                tutorialPanel2Text.text = "Play an <b>Egg</b> and end your turn.";
                break;
            case 27:
                cardForcedToPlayId = -1;
                endRoundBlocker.SetActive(false);
                break;
            case 28:
                endRoundBlocker.SetActive(true);
                tutorialPanel2Button.SetActive(true);
                tutorialPanel2Text.text = "Some of your cards are <b>Boosted</b> so their power is green.";
                break;
            case 29:
                tutorialPanel2Text.text = "However, this <b>Boost</b> can be reset, and does not remain on the entity upon death.";
                break;
            case 30:
                tutorialPanel2Text.text = "If you want to permanently improve your entity use the <b>Strengthen</b> keyword.";
                break;
            case 31:
                cardForcedToPlayId = 126;
                tutorialPanel2Button.SetActive(false);
                tutorialPanel2Text.text = "Play a <b>Little Imp</b> and use it on your <b>Egg</b>.";
                break;
            case 32:
                cardForcedToPlayId = -1;
                cardForcedToSelectId = 103;
                break;
            case 33:
                cardForcedToSelectId = -1;
                tutorialPanel2Button.SetActive(true);
                tutorialPanel2Text.text = "Note that the <b>Egg</b>'s strength is not green because it has been <b>Strengthen</b> rather than <b>Boosted</b>.";
                break;
            case 34:
                tutorialPanel2Button.SetActive(false);
                endRoundBlocker.SetActive(false);
                tutorialPanel2Text.text = "Now end your turn.";
                break;
            case 35:
                cardForcedToPlayId = 116;
                grubHaveToEatEgg = true;
                endRoundBlocker.SetActive(true);
                tutorialPanel2Text.text = "Play <b>Disgusting Grub</b> next to your <b>Egg</b> to destroy it.";
                break;
            case 36:
                cardForcedToPlayId = -1;
                endRoundBlocker.SetActive(false);
                tutorialPanel2Text.text = "What a combo! A <b>Chick</b> popped out of your <b>Egg</b>. Now End your turn.";
                break;
            case 37:
                cardForcedToPlayId = 128;
                grubHaveToEatEgg = false;
                endRoundBlocker.SetActive(true);
                tutorialPanel2Text.text = "Play <b>Old Witch</b> and resurrect your empowered <b>Egg</b>.";
                break;
            case 38:
                cardForcedToPlayId = 103;
                cardForcedToSelectFromGraveyardId = 103;
                break;
            case 39:
                cardForcedToPlayId = -1;
                cardForcedToSelectFromGraveyardId = 0;
                endRoundBlocker.SetActive(false);
                tutorialPanel2Text.text = "Notice that the <b>Egg</b> is still stronger because it has been <b>Strengthen</b>, not <b>Boosted</b>. Now End your turn.";
                break;
            case 40:
                tutorialPanel2Button.SetActive(true);
                endRoundBlocker.SetActive(true);
                tutorialPanel2Text.text = "Cards with the <b>Invulnerable</b> keyword cannot be targeted or affected by area effects.";
                break;
            case 41:
                cardForcedToPlayId = 45;
                tutorialPanel2Button.SetActive(false);
                tutorialPanel2Text.text = "Play <b>The Ascended</b> and end your turn.";
                break;
            case 42:
                cardForcedToPlayId = -1;
                endRoundBlocker.SetActive(false);
                break;
            case 43:
                tutorialPanel2Button.SetActive(true);
                endRoundBlocker.SetActive(true);
                tutorialPanel2Text.text = "Finally, the <b>Spy</b> keyword means that the unit is played against the opponent's side, instead of yours.";
                break;
            case 44:
                cardForcedToPlayId = 89;
                tutorialPanel2Button.SetActive(false);
                tutorialPanel2Text.text = "Play a <b>Parrot</b> to finish off your enemy!";
                break;
            case 45:
                cardForcedToPlayId = 0;
                endRoundBlocker.SetActive(false);
                tutorialPanel2Text.text = "Well done! You have defeated your opponent once again!";
                CompleteTutorialStage(2);
                break;
        }
        tutorialPhase++;
    }

    private void PlayTutorial3()
    {
        switch (tutorialPhase)
        {
            case 0:
                tutorialPanel1.SetActive(true);
                endMulliganBlocker.SetActive(true);
                cardForcedToMulliganId = -1;
                cardForcedToPlayId = -1;
                tutorialPanel1Text.text = "To complete the tutorial you must face a difficult challenge.";
                break;
            case 1:
                tutorialPanel1Text.text = "Use everything you've learned to defeat your opponent. Good luck!";
                break;
            case 2:
                tutorialPanel1.SetActive(false);
                endMulliganBlocker.SetActive(false);
                cardForcedToMulliganId = 0;
                cardForcedToPlayId = 0;
                break;
        }
        tutorialPhase++;
    }

    public void CompleteTutorialStage(int stageToComplete)
    {
        if (playerAccount.TutorialProgress < stageToComplete)
        {
            playerAccount.TutorialProgress = stageToComplete;
            playerDataManager.SaveToLocal();
        }
    }

    public void OkButtonHandler()
    {
        if ((DateTime.Now - lastOkButtonPress).TotalSeconds < 0.3f) return;
        lastOkButtonPress = DateTime.Now;

        PlayTutorial();
    }
}