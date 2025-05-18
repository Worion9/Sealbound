using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerDataManager
{
    private readonly PlayerAccount playerAccount;

    public PlayerDataManager(PlayerAccount playerAccount)
    {
        this.playerAccount = playerAccount;
    }

    public void SaveToLocal()
    {
        PlayerPrefs.SetString("Nickname", playerAccount.Nickname);
        PlayerPrefs.SetInt("AvatarIndex", playerAccount.AvatarIndex);
        PlayerPrefs.SetInt("Level", playerAccount.Level);
        PlayerPrefs.SetInt("Rank", playerAccount.Rank);
        PlayerPrefs.SetInt("Packages", playerAccount.Packages);
        PlayerPrefs.SetInt("Gold", playerAccount.Gold);
        PlayerPrefs.SetInt("Scraps", playerAccount.Scraps);
        PlayerPrefs.SetString("DecksList", string.Join(";", playerAccount.DecksList));
        PlayerPrefs.SetString("DeckNamesList", string.Join(";", playerAccount.DeckNamesList));
        PlayerPrefs.SetString("CurrentDeck", playerAccount.CurrentDeck);
        PlayerPrefs.SetString("CurrentEnemyDeck", playerAccount.CurrentEnemyDeck);
        PlayerPrefs.SetInt("CurrentTutorialStage", playerAccount.CurrentTutorialStage);
        PlayerPrefs.SetInt("TutorialProgress", playerAccount.TutorialProgress);
        PlayerPrefs.SetInt("IsGameOnline", playerAccount.IsGameOnline ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadFromLocal()
    {
        playerAccount.Nickname = PlayerPrefs.GetString("Nickname", playerAccount.Nickname);
        playerAccount.AvatarIndex = PlayerPrefs.GetInt("AvatarIndex", playerAccount.AvatarIndex);
        playerAccount.Level = PlayerPrefs.GetInt("Level", playerAccount.Level);
        playerAccount.Rank = PlayerPrefs.GetInt("Rank", playerAccount.Rank);
        playerAccount.Packages = PlayerPrefs.GetInt("Packages", playerAccount.Packages);
        playerAccount.Gold = PlayerPrefs.GetInt("Gold", playerAccount.Gold);
        playerAccount.Scraps = PlayerPrefs.GetInt("Scraps", playerAccount.Scraps);

        playerAccount.DecksList = new List<string>(
            PlayerPrefs.GetString("DecksList", string.Join(";", playerAccount.DecksList))
            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

        playerAccount.DeckNamesList = new List<string>(
            PlayerPrefs.GetString("DeckNamesList", string.Join(";", playerAccount.DeckNamesList))
            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

        playerAccount.CurrentDeck = PlayerPrefs.GetString("CurrentDeck", playerAccount.CurrentDeck);
        playerAccount.CurrentEnemyDeck = PlayerPrefs.GetString("CurrentEnemyDeck", playerAccount.CurrentEnemyDeck);
        playerAccount.CurrentTutorialStage = PlayerPrefs.GetInt("CurrentTutorialStage", playerAccount.CurrentTutorialStage);
        playerAccount.TutorialProgress = PlayerPrefs.GetInt("TutorialProgress", playerAccount.TutorialProgress);
        playerAccount.IsGameOnline = PlayerPrefs.GetInt("IsGameOnline", playerAccount.IsGameOnline ? 1 : 0) == 1;
    }
}