using System;
using UnityEngine;
using UnityEngine.UI;


public class IngameMenuController : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI title    = default;
    [SerializeField] private TMPro.TextMeshProUGUI subtitle = default;

    [Header("Menu Buttons")]
    [SerializeField] private Button resumeButton   = default;
    [SerializeField] private Button mainMenuButton = default;
    [SerializeField] private Button restartButton  = default;
    [SerializeField] private Button quitButton     = default;

    private Action actionOnMenuOpen;
    private Action actionOnMenuClose;

    public void SetActionOnMenuOpen(Action actionOnMenuOpen)   { this.actionOnMenuOpen  = actionOnMenuOpen;  }
    public void SetActionOnMenuClose(Action actionOnMenuClose) { this.actionOnMenuClose = actionOnMenuClose; }

    public void OpenAsPauseMenu(RecordedScore recordedScore)
    {
        actionOnMenuOpen();
        gameObject.SetActive(true);
        UiUtils.SetButtonActiveAndEnabled(resumeButton, true);
        title.text = "Game Paused";
        subtitle.text = recordedScore.LeftPlayerScore.ToString() + " - " + recordedScore.RightPlayerScore.ToString();
    }
    public void OpenAsEndGameMenu(RecordedScore recordedScore)
    {
        actionOnMenuOpen();
        gameObject.SetActive(true);
        title.text = recordedScore.IsLeftPlayerWinning() ? "Game Won" : "Game Lost";
        subtitle.text = recordedScore.LeftPlayerScore.ToString() + " - " + recordedScore.RightPlayerScore.ToString();

        UiUtils.SetButtonActiveAndEnabled(resumeButton, false);
    }

    void Awake()
    {
        gameObject.SetActive(false);
        #if UNITY_WEBGL
            UiUtils.SetButtonActiveAndEnabled(quitButton, false);
        #endif
    }

    void OnEnable()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(MoveToMainMenu);
        restartButton.onClick.AddListener(TriggerRestartGameEvent);
        quitButton.onClick.AddListener(SceneUtils.QuitGame);
    }
    void OnDisable()
    {
        resumeButton.onClick.RemoveListener(ResumeGame);
        mainMenuButton.onClick.RemoveListener(MoveToMainMenu);
        restartButton.onClick.RemoveListener(TriggerRestartGameEvent);
        quitButton.onClick.RemoveListener(SceneUtils.QuitGame);
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
        GameEventCenter.resumeGame.Trigger("Resuming game");
        actionOnMenuClose();
    }
    private void MoveToMainMenu()
    {
        Time.timeScale = 1;
        GameEventCenter.gotoMainMenu.Trigger("Opening main menu");
        SceneUtils.LoadScene("MainMenu");
        actionOnMenuClose();
    }
    private void TriggerRestartGameEvent()
    {
        Time.timeScale = 1;
        GameEventCenter.restartGame.Trigger("Restarting game");
    }
}
