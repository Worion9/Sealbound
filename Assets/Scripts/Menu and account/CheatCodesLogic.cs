using UnityEngine;
using UnityEngine.EventSystems;

public class CheatCodesLogic : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private MainMenu mainMenu;

    [SerializeField] private GameObject enemyDeckCodeContainer;
    [SerializeField] private GameObject console;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Input.GetKey(KeyCode.Space)) return;

        if (Input.GetKey(KeyCode.Alpha1))
        {
            GameObject consoleInstance = GameObject.FindGameObjectWithTag("Console");
            if (consoleInstance != null) Destroy(consoleInstance);
            else Instantiate(console);
            audioManager.PlaySFX(AudioManager.SFX.CardPlay);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            if (mainMenu.playerAccount.TutorialProgress <= 2) mainMenu.playerAccount.TutorialProgress++;
            else mainMenu.playerAccount.TutorialProgress = 0;
            mainMenu.playerDataManager.SaveToLocal();
            mainMenu.LoadTutorialProgress();
            audioManager.PlaySFX(AudioManager.SFX.CardPlay);
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            enemyDeckCodeContainer.SetActive(!enemyDeckCodeContainer.activeSelf);
            audioManager.PlaySFX(AudioManager.SFX.CardPlay);
        }
    }
}