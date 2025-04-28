using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [SerializeField]
    private WayPointPath _waypointPath;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private Transform _playerTarget;

    private int _targetWaypointIndex;

    private Transform _previousWaypoint;
    private Transform _targetWayPoint;

    private float _timeToWaypoint;
    private float _elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        TargetNextWaypoint();

        Vector3 initialDirectionToPlayer = _playerTarget.position - transform.position;
        Quaternion initialRotation = Quaternion.LookRotation(initialDirectionToPlayer);
        transform.rotation = initialRotation;
    }

    // Update is called once per frame
    void Update()
    {
        _elapsedTime += Time.deltaTime;

        float elapsedPercentage = _elapsedTime / _timeToWaypoint;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        transform.position = Vector3.Lerp(_previousWaypoint.position, _targetWayPoint.position, elapsedPercentage);


        Vector3 directionToPlayer = _playerTarget.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, elapsedPercentage);

        if (elapsedPercentage >= 1)
        {
            TargetNextWaypoint();
        }
    }

    private void TargetNextWaypoint()
    {
        _previousWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);
        _targetWaypointIndex = _waypointPath.GetNextWaypointIndex(_targetWaypointIndex);
        _targetWayPoint = _waypointPath.GetWaypoint(_targetWaypointIndex);

        _elapsedTime = 0;

        float distanceToWaypoint = Vector3.Distance(_previousWaypoint.position, _targetWayPoint.position);
        _timeToWaypoint = distanceToWaypoint / _speed;
    }

}
