using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTime : MonoBehaviour
{
    private float lifetime;
    private Transform spawnPoint;

    public void Initialize(float lifetime, Transform spawnPoint)
    {
        this.lifetime = lifetime;
        this.spawnPoint = spawnPoint;

        StartCoroutine(LifetimeRoutine());
    }

    private IEnumerator LifetimeRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < lifetime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        DestroyTarget(false);
    }

    private void DestroyTarget(bool wasHit)
    {

        // Zwolnienie punktu spawn
        TargetSpawnerTime spawner = FindObjectOfType<TargetSpawnerTime>();
        if (spawner != null)
        {
            spawner.ReleaseSpawnPoint(spawnPoint);
        }

        // Jeœli cel nie zosta³ trafiony, nie dodajemy punktów
        if (!wasHit)
        {
            TimeModeScript timeMode = FindObjectOfType<TimeModeScript>();
            if (timeMode != null)
            {
                timeMode.AddPoints(0); // Brak punktów
            }
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            AudioManager.Instance.PlayRandomSound(new string[]{ "hit-sound-1","hit-sound-2","hit-sound-3"});
            int points = 1000;

            TimeModeScript timeMode = FindObjectOfType<TimeModeScript>();
            if (timeMode != null)
            {
                timeMode.AddPoints(points);
            }

            DestroyTarget(true);
        }
    }
}