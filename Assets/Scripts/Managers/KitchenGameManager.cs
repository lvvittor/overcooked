using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour {

    public static KitchenGameManager Instance { get; private set; }

    public event EventHandler OnStageChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameResumed;

    private const string HIGH_SCORE_TABLE = "highScoreTable"; 

    private enum State {
        WaitingForPlayerToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    private State state;
    private float waitingToStartTimer = 1f;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 60f;
    private bool isPaused = false;
    private bool hasSavedHighScores = false;

    private void Awake(){
        Instance = this;
        state = State.WaitingForPlayerToStart;
    }

    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
    }

    private void Update(){
        switch (state) {
            case State.WaitingForPlayerToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer < 0f) {
                    state = State.CountdownToStart;
                    OnStageChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f) {
                    state = State.GamePlaying;
                    gamePlayingTimer = gamePlayingTimerMax;
                    OnStageChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f) {
                    state = State.GameOver;
                    OnStageChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                if (!hasSavedHighScores) { 
                    // Create HighScoreEntry
                    HighScoreEntry highscoreEntry = new HighScoreEntry{ score = DeliveryManager.Instance.GetRecipesDelivered() };

                    // Load saved HighScores
                    HighScores highscores = loadHighScores();

                    // Add new entry to HighScores
                    highscores.highscoreEntryList.Add(highscoreEntry);

                    // Save updated HighScores
                    saveHighScores(highscores);
                    
                    hasSavedHighScores = true;
                    Debug.Log("Game Over" + PlayerPrefs.GetString(HIGH_SCORE_TABLE));
                }
                break;
        }
    }

    private HighScores loadHighScores(){
        string jsonString = PlayerPrefs.GetString(HIGH_SCORE_TABLE);
        HighScores highscores = JsonUtility.FromJson<HighScores>(jsonString);

        if (highscores == null || highscores.highscoreEntryList == null) {
            highscores = new HighScores();
            highscores.highscoreEntryList = new List<HighScoreEntry>();
        }

        return highscores;
    }

    private void saveHighScores(HighScores highscores){
        string json = JsonUtility.ToJson(highscores, true); // PrettyPrint set to true
        PlayerPrefs.SetString(HIGH_SCORE_TABLE, json);
        PlayerPrefs.Save();
    }

    [System.Serializable]
    private class HighScores{
        public List<HighScoreEntry> highscoreEntryList;
    }

    [System.Serializable]
    private class HighScoreEntry{
        public int score;
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        TogglePause();
    }

    public bool IsGamePlaying() {
        return state == State.GamePlaying;
    }

    public bool IsCountdownToStartActive() {
        return state == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer(){
        return countdownToStartTimer;
    }

    public bool IsGameOver() {
        return state == State.GameOver;
    }

    public float GetGamePlayingTimerNormalized(){
        return 1 - (gamePlayingTimer / gamePlayingTimerMax);
    }
    
    public void TogglePause() {
        isPaused = !isPaused;
        if (isPaused) {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        } else {
            Time.timeScale = 1f;
            OnGameResumed?.Invoke(this, EventArgs.Empty);
        }
    }
}
