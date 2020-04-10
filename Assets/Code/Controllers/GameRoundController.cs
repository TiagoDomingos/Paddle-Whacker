﻿using UnityEngine;


// todo: handle difficulty level, maybe when ai controller is reset, pass it in?
public class GameRoundController : MonoBehaviour
{
    private RecordedScore recordedScore;

    // todo: replace with Paddle/MoveController interfaces, and use like `MoveController.Reset()`
    [SerializeField] private GameObject ball         = default;
    [SerializeField] private GameObject playerPaddle = default;
    [SerializeField] private GameObject aiPaddle     = default;

    [SerializeField] private GameObject leftGoal     = default;
    [SerializeField] private GameObject rightGoal    = default;

    void Awake()
    {
        if ((playerPaddle.transform.position.x < 0 && aiPaddle.transform.position.x < 0) ||
            (playerPaddle.transform.position.x > 0 && aiPaddle.transform.position.x > 0))
        {
            Debug.LogError("Both player and paddle cannot be on the same side of the arena.");
        }
        GameEventCenter.startNewGame.AddAutoUnsubscribeListener(StartNewGame);
    }

    void OnEnable()
    {
        GameEventCenter.goalHit.AddListener(MoveToNextRound);
        GameEventCenter.restartGame.AddListener(RestartGame);
    }
    void OnDisable()
    {
        GameEventCenter.goalHit.RemoveListener(MoveToNextRound);
        GameEventCenter.restartGame.RemoveListener(RestartGame);
    }

    private void StartNewGame(GameSettings gameSettings)
    {
        recordedScore = new RecordedScore(gameSettings.NumberOfGoals);
        aiPaddle.GetComponent<AiController>().SetDifficultyLevel(gameSettings.DifficultyLevel);
        GameEventCenter.scoreChange.Trigger(recordedScore);
    }
    private void RestartGame(string status)
    {
        ResetMovingObjects();
        recordedScore = new RecordedScore(recordedScore.WinningScore);
        GameEventCenter.scoreChange.Trigger(recordedScore);
    }
    private void MoveToNextRound(string goalName)
    {
        if (recordedScore == null)
        {
            Debug.LogError($"RecordedScore that is set upon starting a new game {GetType().Name} is missing, " +
                           $"perhaps the event wasn't fired or listened to? " +
                           $"If running from game scene in play mode, try starting from main menu instead");
        }
        ResetMovingObjects();
        IncrementScoreBasedOnGoal(goalName);
        GameEventCenter.scoreChange.Trigger(recordedScore);
        if (recordedScore.IsWinningScoreReached())
        {
            GameEventCenter.winningScoreReached.Trigger(recordedScore);
        }
    }

    private void ResetMovingObjects()
    {
        ball.GetComponent<BallController>().Reset();
        playerPaddle.GetComponent<PlayerController>().Reset();
        aiPaddle.GetComponent<AiController>().Reset();
    }
    private void IncrementScoreBasedOnGoal(string goalName)
    {
        if (goalName == rightGoal.name)
        {
            recordedScore.IncrementLeftPlayerScore();
        }
        else if (goalName == leftGoal.name)
        {
            recordedScore.IncrementRightPlayerScore();
        }
        else
        {
            Debug.LogError("Goal name '" + goalName + "' does not match registered goal names");
        }
    }
}