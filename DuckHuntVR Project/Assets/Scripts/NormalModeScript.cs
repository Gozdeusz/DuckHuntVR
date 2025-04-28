using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NormalModeScript : MonoBehaviour
{
    public GameObject targetSpawner; // Spawner celów
    public GameObject weapon; // Broñ gracza
    public int playerLives = 3; // Liczba ¿yæ gracza
    public int bulletsPerRound = 5; // Liczba pocisków na rundê
    public int currentLevel = 1; // Aktualny poziom

    private int roundsCompleted = 0;
    private bool isGameOver = false;
    private int currentBullets;

    // UI Elements
    public GameObject normalMoadePanel;

    public Transform HPList ;
    public Transform BulletList;

    public Sprite fullHPSprite;
    public Sprite emptyHPSprite;

    public Sprite fullBulletSprite; 
    public Sprite emptyBulletSprite;

    public TMP_Text gameInfoText;

    public GameObject endGamePanel;


    public void StartNormalMode()
    {
        endGamePanel.SetActive(false);
        if (targetSpawner != null) 
            targetSpawner.SetActive(true);
        normalMoadePanel.SetActive(true);

        ResetGame();
        StartCoroutine(GameLoop());
    }

    private void ResetGame()
    {
        playerLives = 3;
        currentLevel = 1;
        roundsCompleted = 0;
        currentBullets = bulletsPerRound;
        UpdateHPUI();
        UpdateBulletUI();
    }

    private IEnumerator GameLoop()
    {
        TargetSpawn spawner = targetSpawner.GetComponent<TargetSpawn>();
        if (spawner == null)
        {
            Debug.LogError("Nie odnaleziono komponentu <<TargetSpawn>>!");
            yield break;
        }

        while (!isGameOver)
        {
            FireBulletTrigger fireTrigger = weapon.GetComponent<FireBulletTrigger>();

            //Aktywowanie broni
            if (fireTrigger != null)
            {
                fireTrigger.ResetShots();
            }

            //Resetowanie liczby strzalow
    

            fireTrigger.blockShooting();
            yield return StartCoroutine(PreRoundCooldown());
            ResetShots();
            fireTrigger.allowShooting();

            //Wyswietlenie informacji o aktualnej rundzie
            gameInfoText.text = $"Runda {roundsCompleted + 1}";

            //Spawnowanie celow
            spawner.SpawnTargets();

            // SprawdŸ, czy lista celów jest pusta
            yield return new WaitUntil(() => spawner.IsRoundComplete() || currentBullets == 0);

            //Koniec rundy
            if (spawner.AllTargetsHit())
            {
                gameInfoText.text = $"Runda {roundsCompleted + 1} zakoñczona!";
            }
            else
            {
                playerLives--;
                UpdateHPUI();

                if (playerLives <= 0)
                {
                    EndGame();
                    yield break;
                }
            }

            //Zwieksza poziom i trudnosc o 1.
            roundsCompleted++;
            currentLevel++;
        }
    }

    //Odliczanie przed startem kazdej rudny
    private IEnumerator PreRoundCooldown()
    {
        for (int i = 3; i > 0; i--)
        {
            gameInfoText.text = $"{i}";
            yield return new WaitForSeconds(1f);
        }
    }

    //Odejmowanie liczby dostepnych strzalow podczas strzelania
    public void UseBullet()
    {
        if (currentBullets > 0)
        {
            currentBullets--;
            UpdateBulletUI();
        }
    }

    //Resetowanie liczby strzalow
    private void ResetShots()
    {
        AudioManager.Instance.PlaySound("reload");
        currentBullets = bulletsPerRound;
        UpdateBulletUI();
    }

    //Wyswietlanie liczby zycia
    private void UpdateHPUI()
    {
        for (int i = 0; i < HPList.childCount; i++)
        {
            Image hpImage = HPList.GetChild(i).GetComponent<Image>();
            if (hpImage != null)
            {
                hpImage.sprite = i < playerLives ? fullHPSprite : emptyHPSprite;
            }
        }
    }

    //Wyswietlanie liczby strzalow
    private void UpdateBulletUI()
    {
        for (int i = 0; i < BulletList.childCount; i++)
        {
            Image bulletImage = BulletList.GetChild(i).GetComponent<Image>();
            if (bulletImage != null)
            {
                bulletImage.sprite = i < currentBullets ? fullBulletSprite : emptyBulletSprite;
            }
        }
    }

    //Koniec gry
    private void EndGame()
    {
        endGamePanel.SetActive(true);
        isGameOver = true;
        if (targetSpawner != null) targetSpawner.SetActive(false);
        gameInfoText.text = $"Ukoñczone rundy: {roundsCompleted}";
    }
}
