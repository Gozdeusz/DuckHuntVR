using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTarget : MonoBehaviour
{
    private float speed;

    public void SetMovementSpeed(float multiplier)
    {
        speed = 1f * multiplier; // Bazowa prêdkoœæ razy mno¿nik
    }

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}
