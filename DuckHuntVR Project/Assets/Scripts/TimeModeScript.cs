using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;


public class TimeModeScript : MonoBehaviour
{

    //UI
    public GameObject timerPanel;
    public TMP_Text timerInfo;
    public TMP_Text pointsInfo;

    //Spawner i bron
    public GameObject targetSpawner;
    public GameObject weapon;

    //Konfiguracja trybu
    private int totalPoints = 0;
    private float timeRemaining = 120f;
    private bool isGameOver = false;

    public GameObject endGamePanel;

    public void StartTimeMode()
    {
        endGamePanel.SetActive(false);
        FireBulletTrigger.IsTimeMode = true;
        timerPanel.SetActive(true);
        ResetGame();
        StartCoroutine(GameLoop());
    }

    private void ResetGame()
    {
        timeRemaining = 122f;
        totalPoints = 0;
        UpdatePointsUI();
        UpdateTimerUI();
    }

    private IEnumerator GameLoop()
    {
        TargetSpawnerTime spawner = targetSpawner.GetComponent<TargetSpawnerTime>();
        if (spawner == null)
        {
            Debug.LogError("Nie odnaleziono komponentu TargetSpawnerTime!");
            yield break;
        }

        spawner.StartSpawning();

        int previousSecond = Mathf.CeilToInt(timeRemaining);

        while (timeRemaining > 0 && !isGameOver)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();

            int currentSecond = Mathf.CeilToInt(timeRemaining);

            if (currentSecond > 0 && currentSecond <= 11 && currentSecond != previousSecond)
            {
                AudioManager.Instance.PlaySound("clock");
            }

            previousSecond = currentSecond;

            yield return null;
        }

        EndGame();
    }

    private void UpdateTimerUI()
    {
        if (timeRemaining <= 11)
        {
            timerInfo.color = Color.red;
        }
        else if (timeRemaining <= 30)
        {
            timerInfo.color = Color.yellow;
        }
        else
        {
            timerInfo.color = Color.white;
        }

        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerInfo.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void AddPoints(int points)
    {
        totalPoints += points;
        UpdatePointsUI();
    }

    private void UpdatePointsUI()
    {
        pointsInfo.text = $"Punkty: {totalPoints}";
    }

    private void EndGame()
    {
        endGamePanel.SetActive(true);

        FireBulletTrigger.IsTimeMode = false;
        isGameOver = true;

        TargetSpawnerTime spawner = targetSpawner.GetComponent<TargetSpawnerTime>();
        if (spawner != null)
        {
            spawner.StopSpawning();
        }

        pointsInfo.text = $"!Koniec! Twoje punkty: {totalPoints}";
    }

}
