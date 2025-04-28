using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrentGameMode : MonoBehaviour
{
    private string currentGameMode;

    public GameObject weapon;
    public GameObject targetSpawner;
    public NormalModeScript normalMode;
    public TimeModeScript timeMode;

    public GameObject normalTargetSpawner;
    public GameObject timeTargetSpawner;

    private bool gameStarted = false;


    // Start is called before the first frame update
    void Start()
    {
        currentGameMode = PlayerPrefs.GetString("GameMode");
    }

    public void OnWeaponGrabbed()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            StartGameMode();
        }
    }

    public void StartGameMode()
    {
        if (currentGameMode == "NormalMode")
        {
            SwitchToNormalMode();
            normalMode.StartNormalMode();
       
        }
        else if (currentGameMode == "TimeMode")
        {
            SwitchToTimeMode();
            timeMode.StartTimeMode();
        }
    }

    public void SwitchToNormalMode()
    {
        normalTargetSpawner.SetActive(true);
        timeTargetSpawner.SetActive(false);
    }

    public void SwitchToTimeMode()
    {
        normalTargetSpawner.SetActive(false);
        timeTargetSpawner.SetActive(true);
    }
    public void GoToMenu() 
    {
        SceneManager.LoadScene("Main Menu Scene");
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update(){}
}
