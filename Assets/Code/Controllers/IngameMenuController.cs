using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class IngameMenuController : MonoBehaviour
{
    [SerializeField] private GameObject ingameMenu = default;
    [SerializeField] private TMPro.TextMeshProUGUI title    = default;
    [SerializeField] private TMPro.TextMeshProUGUI subtitle = default;

    [Header("Menu Buttons")]
    [SerializeField] private Button resumeButton   = default;
    [SerializeField] private Button mainMenuButton = default;
    [SerializeField] private Button restartButton  = default;
    [SerializeField] private Button quitButton     = default;

    [Header("GameObjects to Hide on Menu Open")]
    [SerializeField] private GameObject hud = default;
    [SerializeField] private GameObject movingObjects = default;

    void Awake()
    {
        if (ReferenceEquals(gameObject, ingameMenu))
        {
            Debug.LogError($"Controller {GetType().Name} should not be attached to its corresponding menu object, " +
                           $"as it is always running in the game scene, " +
                           $"and needs to be able to deactivate the menu object");
        }
        ingameMenu.SetActive(false);

        GameEventCenter.pauseGame.AddListener(OpenAsPauseMenu);
        GameEventCenter.winningScoreReached.AddListener(OpenAsEndGameMenu);

        #if UNITY_WEBGL
            UiUtils.SetButtonActiveAndEnabled(quitButton, false);
        #endif
    }
    void OnDestroy()
    {
        GameEventCenter.pauseGame.RemoveListener(OpenAsPauseMenu);
        GameEventCenter.winningScoreReached.RemoveListener(OpenAsEndGameMenu);
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
    private void ToggleMenuVisibility(bool isVisible)
    {
        if (isVisible)
        {
            Time.timeScale = 0;
            ingameMenu.SetActive(true);
            hud.SetActive(false);
            movingObjects.SetActive(false);
        }
        else
        {
            Time.timeScale = 1;
            ingameMenu.SetActive(false);
            hud.SetActive(true);
            movingObjects.SetActive(true);
        }
        #if UNITY_WEBGL
            UiUtils.SetButtonActiveAndEnabled(quitButton, false);
        #endif
    }

    private void OpenAsPauseMenu(RecordedScore recordedScore)
    {
        title.text    = "Game Paused";
        subtitle.text = recordedScore.LeftPlayerScore.ToString() + " - " + recordedScore.RightPlayerScore.ToString();

        UiUtils.SetButtonActiveAndEnabled(resumeButton, true);
        ToggleMenuVisibility(true);
    }
    private void OpenAsEndGameMenu(RecordedScore recordedScore)
    {
        if (!recordedScore.IsWinningScoreReached())
        {
            Debug.LogError($"Opening ingame menu as triggered by event `WinningScoreReached`, " +
                           $"but no players have met or surpassed the score: {recordedScore}");
        }
        title.text    = recordedScore.IsLeftPlayerWinning() ? "Game Won" : "Game Lost";
        subtitle.text = recordedScore.LeftPlayerScore.ToString() + " - " + recordedScore.RightPlayerScore.ToString();

        UiUtils.SetButtonActiveAndEnabled(resumeButton, false);
        ToggleMenuVisibility(true);
    }

    private void ResumeGame()
    {
        GameEventCenter.resumeGame.Trigger("Resuming game");
        ToggleMenuVisibility(false);
    }
    private void MoveToMainMenu()
    {
        GameEventCenter.gotoMainMenu.Trigger("Opening main menu");
        Time.timeScale = 1;
        SceneUtils.LoadScene("MainMenu");
    }
    private void TriggerRestartGameEvent()
    {
        ToggleMenuVisibility(false);
        GameEventCenter.restartGame.Trigger("Restarting game");
    }
}
