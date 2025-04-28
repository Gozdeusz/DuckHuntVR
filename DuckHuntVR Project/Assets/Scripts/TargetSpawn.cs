using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

public class TargetSpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject _targetPrefab; // Prefab celu

    [SerializeField]
    private Transform _playerTarget; // Pozycja gracza

    [SerializeField]
    private float _targetLifetime = 10.0f; // Czas ¿ycia celu

    [SerializeField]
    private List<Transform> _spawnPoints; // Lista punktów spawnów

    private List<GameObject> _activeTargets = new List<GameObject>(); // Lista aktywnych celów

    [SerializeField]
    private int targetsPerRound = 2; // Liczba celów na rundê

    [SerializeField, Range(0f, 1f)]
    private float movingTargetChance = 0.2f; // Szansa na ruchomy cel

    [SerializeField]
    private float movementSpeedMultiplier = 1.0f; // Mno¿nik szybkoœci celu

    private int targetIDCounter = 0;

    private Dictionary<int, bool> _targetStatus = new Dictionary<int, bool>();

    // Generowanie celów
    public void SpawnTargets()
    {
        ClearActiveTargets(); // Usuniêcie starych celów

        //Kontrola punktow spawnu i obiektu celu
        if (_spawnPoints.Count == 0 || _targetPrefab == null)
        {
            Debug.LogWarning("Brak punktów spawnów lub prefabu celu!");
            return;
        }

        int targetsPerRound = Random.Range(2, 4);

        //Generowanie okreslonej liczby celow
        for (int i = 0; i < targetsPerRound; i++)
        {
            SpawnSingleTarget();
        }
    }

    //Tworzenie pojedynczego celu
    private void SpawnSingleTarget()
    {
        //Losowanie punktu z listy punkow spownu
        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Count)];

        //Ustawienie celu
        GameObject newTarget = Instantiate(_targetPrefab, spawnPoint.position, Quaternion.identity);

        // Przypisz unikalny ID
        int targetID = targetIDCounter++;

        _targetStatus[targetID] = false;

        //Obracanie celu w kierunku pozycji gracza
        if (_playerTarget != null)
        {
            Vector3 directionToPlayer = _playerTarget.position - newTarget.transform.position;
            Vector3 newRotation = newTarget.transform.rotation.eulerAngles; 
            newRotation.x = 440; 
            newTarget.transform.rotation = Quaternion.Euler(newRotation);
            newTarget.transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }

        Target targetScript = newTarget.GetComponent<Target>();
        if (targetScript != null)
        {
            targetScript.Initialize(this, targetID);
        }

        //Dodanie celu do listy celow
        _activeTargets.Add(newTarget);
        StartCoroutine(DestroyTargetAfterLifetime(newTarget, targetID));
    }

    //Niczenie celow po uplywie czasu
    private IEnumerator DestroyTargetAfterLifetime(GameObject target, int targetID)
    {
        yield return new WaitForSeconds(_targetLifetime);
        if (_targetStatus.ContainsKey(targetID) && !_targetStatus[targetID])
        {
            TargetHit(target, targetID, false);
        }
    }

    //Trafienie celu
    public void TargetHit(GameObject target, int targetId, bool hit = true)
    {
        //Sprawdzanie czy cel istnieje na liscie
        if (target != null && _activeTargets.Contains(target))
        {
            _activeTargets.Remove(target);
            AudioManager.Instance.PlayRandomSound(new string[] { "hit-sound-1", "hit-sound-2", "hit-sound-3" });
            Destroy(target);

            if (_targetStatus.ContainsKey(targetId))
            {
                _targetStatus[targetId] = hit;
            }
        }
    }

    //Niszczenie wszytskich celow i czyszczenie listy
    private void ClearActiveTargets()
    {
        foreach (var target in _activeTargets)
        {
            if (target != null)
            {
                Destroy(target);
            }
        }
        _activeTargets.Clear();
        _targetStatus.Clear();
    }

    public bool IsRoundComplete()
    {
        return _activeTargets.Count == 0;
    }

    public bool AllTargetsHit()
    {
        return _targetStatus.Values.All(status => status);
    }
}
