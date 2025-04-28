using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private TargetSpawn _targetSpawn;
    private int _targetID;

    public void Initialize(TargetSpawn targetSpawn, int targetID)
    {
        _targetSpawn = targetSpawn;
        _targetID = targetID;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Bullet"))
        {
            Debug.Log("Target hit by bullet!");
            if (_targetSpawn != null)
            {
                _targetSpawn.TargetHit(gameObject, _targetID);
            }
        }
    }
}
