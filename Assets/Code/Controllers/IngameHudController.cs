using UnityEngine;
using UnityEngine.UI;


public class IngameHudController : MonoBehaviour
{
    private RecordedScore lastRecordedScore;

    [Header("Ingame Menu")]
    [SerializeField] private IngameMenuController ingameMenuController = default;

    [SerializeField] private GameObject topBanner = default;
    [SerializeField] private GameObject scoreBanner = default;

    [Header("Top Banner")]
    [SerializeField] private Button pauseButton   = default;

    [Header("Score Banner")]
    [SerializeField] private TMPro.TextMeshProUGUI leftScoreLabel  = default;
    [SerializeField] private TMPro.TextMeshProUGUI rightScoreLabel = default;

    void Awake()
    {
        ingameMenuController.SetAdditionalActionOnMenuOpen(() => LoadGame());
        SetAdditionalActionOnMenuOpen();
    }
    void OnEnable()
    {
        GameEventCenter.scoreChange.AddListener(UpdateScore);
        pauseButton.onClick.AddListener(PauseGame);

        GameEventCenter.pauseGame.AddListener(ingameMenu.OpenAsPauseMenu);
        GameEventCenter.winningScoreReached.AddListener(ingameMenu.OpenAsEndGameMenu);
    }
    void OnDisable()
    {
        GameEventCenter.scoreChange.RemoveListener(UpdateScore);
        pauseButton.onClick.RemoveListener(PauseGame);

        GameEventCenter.pauseGame.RemoveListener(ingameMenu.OpenAsPauseMenu);
        GameEventCenter.winningScoreReached.RemoveListener(ingameMenu.OpenAsEndGameMenu);
    }

    private void UpdateScore(RecordedScore recordedScore)
    {
        lastRecordedScore    = recordedScore;
        leftScoreLabel.text  = recordedScore.LeftPlayerScore.ToString();
        rightScoreLabel.text = recordedScore.RightPlayerScore.ToString();
    }

    private void PauseGame()
    {
        if (lastRecordedScore == null)
        {
            Debug.LogError($"LastRecordedScore received by {GetType().Name} is null");
        }
        GameEventCenter.pauseGame.Trigger(lastRecordedScore);
    }
}
