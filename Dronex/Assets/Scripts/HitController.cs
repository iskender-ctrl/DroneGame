using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Common;

public class HitController : MonoBehaviour, ITargetable
{
    private Animator animator;
    private DroneController dronic;
    private WeaponController weapon;
    private float animRate = 0.3f;
    private float nextAnim = 0f;
    public Transform targetableBodyPartial;
    public Transform p;
    public bool disableAnim = false;
    public string projectileTag = "Projectile";

    public void Start()
    {
        animator = p.GetComponent<Animator>();
        dronic = transform.GetComponent<DroneController>();
        weapon = transform.GetComponent<WeaponController>();
    }

    public void Update()
    {
        if (Time.time > nextAnim)
        {
            animator.SetBool("onHitted", false);
        }

        if (dronic.health <= 0)
        {
            if (disableAnim)
            {
                Die();
            }
            else
            {
                p.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == projectileTag)
        {
            nextAnim = Time.time + animRate;
            animator.SetBool("onHitted", true);

            ProjectileController projectileController = other.gameObject.GetComponent<ProjectileController>();
            DroneController otherDroneController = projectileController.getDroneController();
            WeaponController otherWeaponController = otherDroneController.GetComponent<WeaponController>();

            dronic.health -= otherWeaponController.getHealthAttrition();
            otherDroneController.addDamageAmount(otherWeaponController.getHealthAttrition());
        }
    }

    public Transform getTargetable()
    {
        return targetableBodyPartial;
    }

    public void SetKinematic(bool newValue)
    {
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            rb.isKinematic = newValue;
        }
    }

    public void Die()
    {
        SetKinematic(false);
        animator.enabled = false;
        //Destroy(this);
        //Destroy(p.GetComponent<MoveController>());
        //Destroy(p.gameObject, 5);
    }
}
