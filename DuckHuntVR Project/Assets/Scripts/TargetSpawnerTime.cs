using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawnerTime : MonoBehaviour
{
    public GameObject targetPrefab;
    public Transform[] spawnPoints;

    public Transform playerPosition;

    private float spawnInterval = 10f;
    private float targetLifetime = 10f;
    private int targetsPerWave = 3;

    private bool isSpawning = false;
    private HashSet<Transform> occupiedSpawnPoints = new HashSet<Transform>();

    private int score = 0;
    private float timeSinceLastWaveIncrease = 0f;

    public void StartSpawning()
    {
        isSpawning = true;
        score = 0;
        StartCoroutine(SpawnTargetsRoutine());
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();

        foreach (Transform spawnPoint in spawnPoints)
        {
            foreach (Transform child in spawnPoint)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private IEnumerator SpawnTargetsRoutine()
    {
        while (isSpawning)
        {
            SpawnTargets();
            yield return new WaitForSeconds(spawnInterval);

            timeSinceLastWaveIncrease += spawnInterval;
            if (timeSinceLastWaveIncrease >= 30f)
            {
                targetsPerWave++;
                timeSinceLastWaveIncrease = 0f;
            }
        }
    }

    private void SpawnTargets()
    {
        int targetsToSpawn = Mathf.Min(targetsPerWave, spawnPoints.Length - occupiedSpawnPoints.Count);

        while (targetsToSpawn > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            if (!occupiedSpawnPoints.Contains(spawnPoint))
            {
                GameObject target = Instantiate(targetPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);

                if (playerPosition != null)
                {
                    Vector3 directionToPlayer = playerPosition.position - target.transform.position;
                    target.transform.rotation = Quaternion.LookRotation(directionToPlayer);
                }

                TargetTime targetScript = target.GetComponent<TargetTime>();

                if (targetScript != null)
                {
                    targetScript.Initialize(targetLifetime, spawnPoint);
                }

                occupiedSpawnPoints.Add(spawnPoint);
                targetsToSpawn--;
            }
        }
    }

    public void ReleaseSpawnPoint(Transform spawnPoint)
    {
        occupiedSpawnPoints.Remove(spawnPoint);
    }
    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Punkty: " + score);
    }

}


