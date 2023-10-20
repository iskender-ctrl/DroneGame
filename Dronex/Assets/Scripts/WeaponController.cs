using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Photon.Pun;
public class ProjectileSpawnPoint
{
    public Transform spawnPoint;
    public List<Transform> projectiles = new List<Transform>();
    public float nextFire;
}

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    public Transform[] projectileSpawnPoints;
    public float startHealthAttrition = 10f;
    public string projectile;
    public float fireRate = 0.2f;
    public float range = 5.0f;
    public float speed = 20.0f;
    private List<ProjectileSpawnPoint> pointStates = new List<ProjectileSpawnPoint>();
    private ITargetable target;
    public string enemyTag = "EnemyUnit";
    private DroneController droneController;
    public float damageAmountWeight = 0.1f;

    public void setDroneController(DroneController newDroneController)
    {
        droneController = newDroneController;
    }

    public float getHealthAttrition()
    {
        return this.startHealthAttrition + (droneController.getDamageAmount() * damageAmountWeight);
    }

    public void Start()
    {
        int maxSimultaneousProjectile = (int)((range / speed) / fireRate) + 1;
        foreach (Transform spawnPoint in projectileSpawnPoints)
        {
            ProjectileSpawnPoint projectileSpawnPoint = new ProjectileSpawnPoint();
            projectileSpawnPoint.spawnPoint = spawnPoint;

            for (int i = 1; i <= maxSimultaneousProjectile; i++)
            {
                Transform projectileTransform = PhotonNetwork.Instantiate(projectile, spawnPoint.position, spawnPoint.rotation).gameObject.transform;
                projectileTransform.gameObject.SetActive(false);
                ProjectileController projectileController = projectileTransform.GetComponent<ProjectileController>();
                projectileController.setDroneController(droneController);

                projectileSpawnPoint.projectiles.Add(projectileTransform);
            }

            pointStates.Add(projectileSpawnPoint);
        }
    }

    public void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            //Fire();
        }
    }

    public void setTarget(ITargetable target)
    {
        this.target = target;
    }

    public void Fire()
    {
        foreach (ProjectileSpawnPoint pointState in pointStates)
        {
            foreach (Transform projectileTransform in pointState.projectiles)
            {
                if (Time.time > pointState.nextFire)
                {
                    ProjectileController projectileCtrl = projectileTransform.GetComponent<ProjectileController>();
                    projectileCtrl.enemyTag = enemyTag;
                    if (projectileCtrl.ready)
                    {
                        pointState.nextFire = Time.time + fireRate;
                        pointState.spawnPoint.LookAt(target.getTargetable());
                        projectileCtrl.setRange(range);
                        projectileCtrl.setSpeed(speed);
                        projectileCtrl.Fire(pointState);
                        projectileCtrl.ready = false;
                        GetComponent<AudioSource>().Play();
                    }
                }
            }
        }
    }
}
