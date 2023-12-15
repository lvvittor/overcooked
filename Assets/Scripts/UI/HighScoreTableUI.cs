using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HighScoreTableUI : MonoBehaviour
{
    public static HighScoreTableUI Instance { get; private set; }
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> highscoreEntryTransformList;

    [SerializeField] private Button mainMenuButton;

    private const string HIGH_SCORE_TABLE = "highScoreTable"; 

    private void Start() {
        Hide();
    }

    public static HighScoreTableUI GetInstance() {
        return Instance;
    }

    private void Awake(){
        Instance = this;
        mainMenuButton.onClick.AddListener(() => {
            Hide();
        });

        entryContainer = transform.Find("highScoreEntryContainer");
        entryTemplate = entryContainer.Find("highScoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        HighScores highscores = loadHighScores();

        // Sort scores
        highscores.highscoreEntryList.Sort((a, b) => b.score.CompareTo(a.score));

        highscoreEntryTransformList = new List<Transform>();

        // Limit to the top 10 entries
        int entriesToShow = Mathf.Min(10, highscores.highscoreEntryList.Count);
        for (int i = 0; i < entriesToShow; i++){
            CreateHighScoreEntryTransform(highscores.highscoreEntryList[i], entryContainer, highscoreEntryTransformList);
        }
    }

    private void CreateHighScoreEntryTransform(HighScoreEntry highScoreEntry, Transform container, List<Transform> transformList){
        float templateHeight = 40f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank){
            default:
                rankString = rank + "TH"; break;
            case 1: rankString = "1ST"; break;
            case 2: rankString = "2ND"; break;
            case 3: rankString = "3RD"; break;
        }
        entryTransform.Find("posText").GetComponent<Text>().text = rankString;
        entryTransform.Find("scoreText").GetComponent<Text>().text = highScoreEntry.score.ToString();
        entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);

        if (rank == 1){
            entryTransform.Find("posText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
        }

        transformList.Add(entryTransform);
    }

    private HighScores loadHighScores(){
        string jsonString = PlayerPrefs.GetString(HIGH_SCORE_TABLE);
        Debug.Log("Loaded: " + jsonString);
        HighScores highscores = JsonUtility.FromJson<HighScores>(jsonString);

        if (highscores == null || highscores.highscoreEntryList == null) {
            Debug.Log("Initializing high scores");
            highscores = new HighScores();
            highscores.highscoreEntryList = new List<HighScoreEntry>();
        }

        return highscores;
    }

    private void saveHighScores(HighScores highscores){
        string json = JsonUtility.ToJson(highscores, true); // PrettyPrint set to true
        PlayerPrefs.SetString(HIGH_SCORE_TABLE, json);
        PlayerPrefs.Save();
        Debug.Log("Saved: " + json);
    }

    public void AddHighScoreEntry(int score){
        // Create HighScoreEntry
        HighScoreEntry highscoreEntry = new HighScoreEntry{ score = score };

        // Load saved HighScores
        HighScores highscores = loadHighScores();

        // Add new entry to HighScores
        highscores.highscoreEntryList.Add(highscoreEntry);

        // Save updated HighScores
        saveHighScores(highscores);

        Debug.Log("High score entry added: " + score);
        Debug.Log("High Scores:" + highscores.highscoreEntryList.Count);
        Debug.Log("Has Key:" + PlayerPrefs.HasKey(HIGH_SCORE_TABLE));
        Debug.Log("Updated to: " + PlayerPrefs.GetString(HIGH_SCORE_TABLE));
    }
    
    [System.Serializable]
    private class HighScores{
        public List<HighScoreEntry> highscoreEntryList;
    }

    [System.Serializable]
    private class HighScoreEntry{
        public int score;
    }

    public void Show(){
        gameObject.SetActive(true);
    }

    private void Hide(){
        gameObject.SetActive(false);
    }
}
