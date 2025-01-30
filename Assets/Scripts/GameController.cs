using System;
using ParentHouse.UI;
using ParentHouse.Utils;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour {
    
    [SerializeField, FoldoutGroup("Settings")]
    private GameObject PlayerPrefab;
    
    [SerializeField, FoldoutGroup("Settings")]
    private float ScoreRateMultiplier = 1.5f;
    
    [SerializeField, FoldoutGroup("Dependencies")]
    private MenuGroup GameHudGroup;
    
    [SerializeField, FoldoutGroup("Dependencies")]
    private MenuGroup GameOverGroup;

    [SerializeField, FoldoutGroup("Dependencies")]
    private TextMeshProUGUI SessionScoreText;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private int SessionScore;
    
    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private bool GameIsActive;
    
    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private GameObject CurrentPlayer;
    
    public static float GameTimeScale = 1;
    public static Action OnGameStart;
    public static Action OnGameEnd;
    private bool GameActive;
    
    private void Awake() {
        GameTimeScale = 1;
    }

    private void OnEnable() {
        OnGameEnd += FinishGame;
    }

    private void OnDisable() {
        OnGameEnd -= FinishGame;
    }

    private void FixedUpdate() {
        if (!GameIsActive || GameTimeScale == 0) return;
        SessionScore += (int)(Time.deltaTime * ScoreRateMultiplier);
        SessionScoreText.text = SessionScore.ToString();
    }

    public void StartNewGame() {
        CurrentPlayer = Pooler.Spawn(PlayerPrefab);
        SessionScore = 0;
        GameIsActive = true;
        GameTimeScale = 1;
        GameHudGroup.OpenAll();
        GameOverGroup.CloseAll();
    }

    public void FinishGame() {
        CurrentPlayer.SetActive(false);
        CurrentPlayer = null;
        GameIsActive = false;
        GameTimeScale = 1;
        GameHudGroup.CloseAll();
        GameOverGroup.OpenAll();
        Leaderboard.SetNewLeaderboardScore(SessionScore);
    }
}
