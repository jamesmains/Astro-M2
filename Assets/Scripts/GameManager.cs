using System;
using System.Collections.Generic;
using System.Linq;
using ParentHouse.UI;
using ParentHouse.Utils;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Dependencies")]
    private GameObject PlayerPrefab;
    
    [SerializeField] [FoldoutGroup("Dependencies")]
    private GameObject EnemyPrefab;

    [SerializeField] [FoldoutGroup("Dependencies")]
    private Transform EntitiesContent;

    [SerializeField] [FoldoutGroup("Dependencies")]
    private TextMeshProUGUI ScoreText;

    [SerializeField] [FoldoutGroup("Dependencies")]
    private TextMeshProUGUI TimeText;

    [SerializeField] [FoldoutGroup("Dependencies")]
    private MenuGroup ResultsMenuGroup;

    [SerializeField] [FoldoutGroup("Settings")]
    private float StartingTime;

    [SerializeField] [FoldoutGroup("Settings")]
    private float PointsPerCapture;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private int MaxEnemySpawns;

    [SerializeField] [FoldoutGroup("Events")]
    private UnityEvent OnStartGame = new();
    
    [SerializeField] [FoldoutGroup("Events")]
    private UnityEvent OnEndGame = new();

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private float RemainingTime;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private int SessionScore;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private int NumberOfSpawnableEnemies;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    public static bool GameActive;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    public static bool TimerActive;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private float TileSize;

    public static GameManager Singleton;

    private void Awake() {
        Singleton = this;
        TimeText.text = RemainingTime.ToString("");
        Application.targetFrameRate = 60; // more of a mobile thing
    }

    // Look at for scoring
    public static void CapturePiece() {
        if (!GameActive) return;
        float timeReward = Singleton.PointsPerCapture / Singleton.NumberOfSpawnableEnemies;
        timeReward = Mathf.Clamp(timeReward, 1f, timeReward);
        Singleton.RemainingTime += timeReward;
        Singleton.SessionScore++;
    }

    private void Update() {
        print($"Game is Active: {GameActive}, Timer is Active: {TimerActive}");
        if (!TimerActive) return;
        if (RemainingTime > 0) {
            RemainingTime -= Time.deltaTime;
            TimeText.text = RemainingTime.ToString("0.0");
        }
        else EndGame(false);
    }

    private void StartGame() {
        TimerActive = false;
        GameActive = true;
        SessionScore = 0;
        ScoreText.text = SessionScore.ToString();
        RemainingTime = StartingTime;
        TimeText.text = RemainingTime.ToString("0.0");
        OnStartGame.Invoke();
    }

    public void EndGame(bool didQuit) {
        GameActive = false;
        TimerActive = false;
        if(!didQuit)
            Leaderboard.SetNewLeaderboardScore(SessionScore);
        ResetGame();
        OnEndGame.Invoke();
    }

    public static void ResetGame() {
    }
}