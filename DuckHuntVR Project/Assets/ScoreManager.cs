using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Normal Mode Text Fields")]
    [SerializeField] private TextMeshPro[] normalModeRounds;
    [SerializeField] private TextMeshPro[] normalModeDates;

    [Header("Time Mode Text Fields")]
    [SerializeField] private TextMeshPro[] timeModePoints;
    [SerializeField] private TextMeshPro[] timeModeDates;

    private const string TimeModeFile = "TimeModeScores.json";
    private const string NormalModeFile = "NormalModeScores.json";

    private List<ScoreEntry> timeModeScores = new List<ScoreEntry>();
    private List<ScoreEntry> normalModeScores = new List<ScoreEntry>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("Instancja ScoreManager zosta³a stworzona.");
            LoadScores();
        }
        else
        {
            Debug.LogWarning("Instancja ScoreManager ju¿ istnieje, niszczê now¹.");
            Destroy(gameObject);
        }
    }

    [Serializable]
    public class ScoreEntry
    {
        public int score; // punkty dla trybu czasowego lub liczba rund dla normalnego
        public string date; // data w formacie "dd-MM-yyyy"
    }

    private void LoadScores()
    {
        timeModeScores = LoadFromFile(TimeModeFile);
        normalModeScores = LoadFromFile(NormalModeFile);
    }

    private List<ScoreEntry> LoadFromFile(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            Debug.Log($"Wczytywanie wyników z pliku: {fileName}");
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<ScoreList>(json)?.scores ?? new List<ScoreEntry>();
        }
        else
        {
            Debug.LogWarning($"Plik {fileName} nie istnieje. Tworzenie nowej listy wyników.");
            return new List<ScoreEntry>();
        }
    }


    private void SaveScores(List<ScoreEntry> scores, string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ScoreList scoreList = new ScoreList { scores = scores };
        string json = JsonUtility.ToJson(scoreList);
        File.WriteAllText(path, json);
        Debug.Log($"Wyniki zapisane do pliku: {fileName}");
    }

    [Serializable]
    private class ScoreList
    {
        public List<ScoreEntry> scores;
    }

    public void AddScore(int score, bool isTimeMode)
    {
        List<ScoreEntry> scores = isTimeMode ? timeModeScores : normalModeScores;
        string fileName = isTimeMode ? TimeModeFile : NormalModeFile;

        Debug.Log($"Dodawanie wyniku {score} dla trybu {(isTimeMode ? "czasowego" : "normalnego")}.");

        if (scores == null)
        {
            Debug.LogError($"Lista wyników dla trybu {(isTimeMode ? "czasowego" : "normalnego")} jest null. Tworzenie nowej listy.");
            scores = new List<ScoreEntry>();
        }

        if (scores.Count < 5 || score > scores[scores.Count - 1].score)
        {
            ScoreEntry newEntry = new ScoreEntry
            {
                score = score,
                date = DateTime.Now.ToString("dd-MM-yyyy")
            };

            scores.Add(newEntry);
            scores.Sort((a, b) => b.score.CompareTo(a.score)); // Sortowanie malej¹co
            if (scores.Count > 5) scores.RemoveAt(5); // Zachowanie tylko top 5

            SaveScores(scores, fileName);
        }
    }


    public void UpdateScoreTable(bool isTimeMode)
    {
        List<ScoreEntry> scores = isTimeMode ? timeModeScores : normalModeScores;

        TextMeshPro[] valueFields = isTimeMode ? timeModePoints : normalModeRounds;
        TextMeshPro[] dateFields = isTimeMode ? timeModeDates : normalModeDates;

        // Dodaj logi debuguj¹ce
        if (valueFields == null || dateFields == null)
        {
            Debug.LogError($"Pola tekstowe nie zosta³y przypisane dla trybu {(isTimeMode ? "czasowego" : "normalnego")}!");
            return;
        }
        if (valueFields.Length != dateFields.Length)
        {
            Debug.LogError("Tablice 'valueFields' i 'dateFields' maj¹ ró¿ne rozmiary!");
            return;
        }

        for (int i = 0; i < valueFields.Length; i++)
        {
            if (valueFields[i] == null || dateFields[i] == null)
            {
                Debug.LogError($"Brak przypisania elementu w tablicy na pozycji {i} dla trybu {(isTimeMode ? "czasowego" : "normalnego")}.");
                continue;
            }

            if (i < scores.Count)
            {
                valueFields[i].text = scores[i].score.ToString();
                dateFields[i].text = scores[i].date;
            }
            else
            {
                valueFields[i].text = "-";
                dateFields[i].text = "-";
            }
        }
        Debug.Log($"Tabela wyników zaktualizowana dla trybu {(isTimeMode ? "czasowego" : "normalnego")}.");
    }

}
