using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class MoveController : MonoBehaviour
{
    private bool hasTarget = false;
    private Vector3 targetPosition = new Vector3(0, 0.5f, 0);
    private NavMeshAgent mNavMeshAgent;
    private Vector3 lookPoint;
    private bool hasLookPoint;
    public bool setDynamicTarget = false;
    public Camera playerCamera = null;

    public enum MovementMode
    {
        Walk,
        Run
    }
    public MovementMode movementMode = MovementMode.Walk;
    public float walkSpeed = 1.0f;
    public float runSpeed = 3.5f;

    public void Start()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        if (!setDynamicTarget)
        {
            checkInputMouse();

            updateSpeed();
        }

        if (hasTarget)
        {
            mNavMeshAgent.destination = targetPosition;
        }
        else
        {
            mNavMeshAgent.destination = transform.position;
            if (hasLookPoint)
            {
                setRotation(lookPoint - mNavMeshAgent.steeringTarget);
            }
        }

        if (!agentIsArrive())
        {
            Vector3 lookRotation = mNavMeshAgent.steeringTarget - transform.position;
            if (lookRotation != Vector3.zero)
            {
                setRotation(mNavMeshAgent.steeringTarget - transform.position);
            }
        }
        else
        {
            hasTarget = false;
        }
    }

    private void setRotation(Vector3 lookRotation)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookRotation), Time.deltaTime * 5f);
    }

    public bool agentIsArrive()
    {
        if (!mNavMeshAgent.pathPending)
        {
            if (mNavMeshAgent.remainingDistance <= mNavMeshAgent.stoppingDistance)
            {
                if (!mNavMeshAgent.hasPath || mNavMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void setTarget(Vector3 point)
    {
        targetPosition = point;
        hasTarget = true;
    }

    public void setMovementMode(MovementMode newMovementMode)
    {
        this.movementMode = newMovementMode;
    }

    public void setLookPoint(Vector3 newLookPoint)
    {
        this.lookPoint = newLookPoint;
        this.hasLookPoint = true;
    }

    public void removeLookPoint()
    {
        this.hasLookPoint = false;
    }

    private void checkInputMouse()
    {
        if (playerCamera != null && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                setTarget(hit.point);
            }
        }
    }

    private void updateSpeed()
    {
        float speed = mNavMeshAgent.speed;
        switch (movementMode)
        {
            case MovementMode.Walk:
                speed = walkSpeed;
                break;
            case MovementMode.Run:
                speed = runSpeed;
                break;
        }

        mNavMeshAgent.speed = speed;
    }
}