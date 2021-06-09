using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Common;

public class DroneController : MonoBehaviour, ITargetable
{
    private NavMeshAgent mNavMeshAgent;
    private MoveController movementController;
    private Animator animator;

    private WeaponController weaponController;
    private GameObject[] enemies;
    public float radarRange = 10.0f;
    private PlatoonController platoon;
    public Transform lookableBodyPartial;
    public Transform targetableBodyPartial;
    public enum ActionMode
    {
        Fresh,
        Attack
    }
    public ActionMode actionMode = ActionMode.Fresh;
    public float health = 100f;
    public float healthIncreaseInTime = 0.1f;
    public string enemyTag = "EnemyUnit";
    private float damageAmount = 0;

    public void setPlatoon(PlatoonController platoon)
    {
        this.platoon = platoon;
    }

    public void addDamageAmount(float newAmount)
    {
        this.damageAmount += newAmount;
    }

    public float getDamageAmount()
    {
        return this.damageAmount;
    }

    public void Start()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        movementController = GetComponent<MoveController>();
        animator = GetComponent<Animator>();
        weaponController = GetComponent<WeaponController>();
        weaponController.enemyTag = enemyTag;
        weaponController.setDroneController(this);
    }

    public void Update()
    {
        enemyInRange();

        updateAction();

        float animSpeed = mNavMeshAgent.speed;
        if (movementController.agentIsArrive())
        {
            animSpeed = 0;
        }

        animator.SetFloat("speed", animSpeed);

        if (health < 100)
        {
            health += healthIncreaseInTime;
        }
    }

    private void enemyInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject enemy = getClosestEnemyInWeaponRange(enemies);

        if (enemy)
        {
            lookableBodyPartial.LookAt(enemy.transform);
            actionMode = ActionMode.Attack;
            ITargetable target = enemy.GetComponent(typeof(ITargetable)) as ITargetable;
            weaponController.setTarget(target);
        }
        else
        {
            actionMode = ActionMode.Fresh;
        }
    }

    private GameObject getClosestEnemyInWeaponRange(GameObject[] enemies)
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget <= Mathf.Pow(weaponController.range, 2) && dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }


    private void updateAction()
    {
        switch (actionMode)
        {
            case ActionMode.Attack:
                animator.SetBool("enemyInRange", true);
                weaponController.Fire();
                break;
            case ActionMode.Fresh:
                animator.SetBool("enemyInRange", false);
                break;
        }
    }

    public Transform getTargetable()
    {
        return targetableBodyPartial;
    }
}
