using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 20.0f;
    public float range = 5.0f;
    public bool ready = true;
    public string enemyTag = "EnemyUnit";
    private float firedAt = 0f;
    private DroneController droneController;
    public Transform[] vfxs;

    public void setDroneController(DroneController newDroneController)
    {
        droneController = newDroneController;
    }

    public DroneController getDroneController()
    {
        return droneController;
    }

    void Update()
    {
        if (!ready && Time.time - firedAt > range / speed)
        {
            gameObject.SetActive(false);
            GetComponent<Rigidbody>().velocity = transform.forward * 0f;
            ready = true;
        }
    }

    public void Fire(ProjectileSpawnPoint pointState)
    {
        if (droneController != null)
        {
            int max = (int)Math.Floor(droneController.getDamageAmount() / 20);
            int l = (vfxs.Length >= max) ? (max > 0 ? max : 1) : vfxs.Length;

            for (int i = 0; i < l; i++)
            {
                if (i == l - 1)
                {
                    vfxs[i].gameObject.SetActive(true);
                }
                else
                {
                    vfxs[i].gameObject.SetActive(false);
                }
            }
        }

        firedAt = Time.time;
        transform.position = pointState.spawnPoint.position;
        transform.rotation = pointState.spawnPoint.rotation;
        gameObject.SetActive(true);

        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

    public void setRange(float range)
    {
        this.range = range;
    }

    public void setSpeed(float speed)
    {
        this.speed = speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == enemyTag)
        {
            gameObject.SetActive(false);
            GetComponent<Rigidbody>().velocity = transform.forward * 0f;
            ready = true;
        }
    }
}
