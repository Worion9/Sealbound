using System.Collections.Generic;

public class PlayerAccount
{
    public string Nickname { get; set; } = "New Player";
    public int AvatarIndex { get; set; } = 0;
    public int Level { get; set; } = 1;
    public int Rank { get; set; } = 1;
    public int Packages { get; set; } = 0;
    public int Gold { get; set; } = 0;
    public int Scraps { get; set; } = 0;
    public List<string> DecksList { get; set; } = new List<string>();
    public List<string> DeckNamesList { get; set; } = new List<string>();
    public string CurrentDeck { get; set; } = "";
    public string CurrentEnemyDeck { get; set; } = "50PA0QK33";
    public int CurrentTutorialStage { get; set; } = 0;
    public int TutorialProgress { get; set; } = 0;
    public bool IsGameOnline { get; set; } = false;
}