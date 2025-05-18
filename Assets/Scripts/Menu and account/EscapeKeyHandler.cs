using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeKeyHandler : MonoBehaviour
{
    [SerializeField] AudioManager audioMenager;

    public enum Option
    {
        Nothing,
        GoToScene,
        TogglePanelSetAcive
    }
    public Option option;

    [SerializeField] private string sceneName;
    [SerializeField] private GameObject panelToToggle;

    public void OnEscapePressed()
    {
        audioMenager.PlaySFX(AudioManager.SFX.ButtonClick);

        if (option == Option.GoToScene) SceneManager.LoadScene(sceneName);

        else if (option == Option.TogglePanelSetAcive && panelToToggle != null) panelToToggle.SetActive(!panelToToggle.activeSelf);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) OnEscapePressed();
    }
}