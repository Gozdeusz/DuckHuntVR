using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public static string currentGameMode { get; private set; }
    public static string currentMap { get; private set; }

    public GameObject startPanel, modePanel, mapPanel;

    [SerializeField] 
    private GameObject scoreTable;

    //Kolejnosc okien: start->mode->map

    void Start()
    {
        startPanel.SetActive(true); //Pokaz ono startowe
        modePanel.SetActive(false);
        mapPanel.SetActive(false);
    }

    public void SetGameMode(string mode)
    {
        currentGameMode = mode;
        PlayerPrefs.SetString("GameMode", mode);
        PlayerPrefs.Save();

        AudioManager.Instance.PlaySound("interface-click");
        modePanel.SetActive(false); //Wylaczyc to okno
        mapPanel.SetActive(true); //Wlaczyc okno map
    }

    public void SetMap(string map)
    {
        AudioManager.Instance.PlaySound("interface-click");
        currentMap = map;
        mapPanel.SetActive(false); //Wylaczyc okno map
        StartingGame();
    }

    public void PlayGame()
    {
        AudioManager.Instance.PlaySound("interface-click");
        startPanel.SetActive(false); //Wylaczyc okno startowe 
        modePanel.SetActive(true); //Wlaczyc okno map
    }

    public void StartingGame() 
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentMap);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowScores(bool isTimeMode)
    {
        // Upewnij siê, ¿e tabela wyników jest aktywna
        scoreTable.SetActive(true);
        startPanel.SetActive(false);

        // Zaktualizuj tabelê wyników w zale¿noœci od trybu gry
        ScoreManager.Instance.UpdateScoreTable(isTimeMode);
    }

    public void CloseScores()
    {
        scoreTable.SetActive(false);
        startPanel.SetActive(true);
    }
    
}
