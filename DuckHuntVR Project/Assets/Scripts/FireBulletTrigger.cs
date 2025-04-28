using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FireBulletTrigger : MonoBehaviour
{
    public static bool IsTimeMode = false;

    private string currentGameMode;

    public GameObject bullet;
    public Transform firePoint;
    public float fireSpeed = 20.0f;

    public int maxShots = 0;
    private int remainingShots;

    private bool canShoot = true;
    public float shootCooldown = 1f;

    void Start()
    {
        bulletsNumberSettings();
        remainingShots = maxShots; // Normalny tryb

        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(FireBullet);
    }
   
    void Update(){}

    //Strzelanie
    public void FireBullet(ActivateEventArgs arg) 
    {
        if (canShoot && remainingShots > 0)
        {
            StartCoroutine(ShootCooldown());
            GameObject spawnedBullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
            spawnedBullet.GetComponent<Rigidbody>().velocity = firePoint.forward * fireSpeed;
            AudioManager.Instance.PlaySound("shoot");
            Destroy(spawnedBullet, 3);

            remainingShots--;
            NormalModeScript normalMode = FindObjectOfType<NormalModeScript>();
            normalMode.UseBullet();

        }

    }

    public void bulletsNumberSettings()
    {
        currentGameMode = PlayerPrefs.GetString("GameMode");
        if (currentGameMode == "NormalMode")
        {
            maxShots = 5;
        }
        else if (currentGameMode == "TimeMode")
        {
            maxShots = 99999;
        }
    }

    public void blockShooting()
    {
        canShoot = false;
    }

    public void allowShooting()
    {
        canShoot = true;
    }

    //Colldown 1 sek na oddanie strza³u
    private IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    //Resetowanie ilosci strzalow
    public void ResetShots()
    {
            remainingShots = maxShots;
    }

}
