using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Common;
using Common.Formation;
using Photon.Pun;

public class PlatoonController : MonoBehaviour
{
    private Formation formation;
    private Transform[] units;
    private NavMeshAgent mNavMeshAgent;
    private List<Transform> pointIcons = new List<Transform>();
    private GameObject dummyFormationObject;
    public float maxDrift = 1.0f;
    public float maxSpeedModifier = 2.0f;
    public string unitName, unitNameEnemy;
    public int unitSize = 8;

    [Range(1.0f, 5.0f)]
    public float scaleFactor = 1.0f;
    private float scaleFactorOld = 1.0f;
    public float rotateFactor = 0f;
    private Boolean platoonMaked = false;
    [SerializeField] PhotonView photonView;
    MonoBehaviour[] scriptComponents;
    public void Start()
    {
        scaleFactorOld = scaleFactor;
        dummyFormationObject = new GameObject("dummyFormationObject");

        if (pointIcons.Count != unitSize)
        {
            for (int i = 0; i < unitSize; i++)
            {
                GameObject pointGameObject = new GameObject("pointGameObject_" + i);
                Transform pointIcon = pointGameObject.transform;
                pointIcons.Add(pointIcon);
            }
        }

        mNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void makePlatoon()
    {
        units = new Transform[unitSize];

        int idx = 0;
        foreach (Transform pointIcon in pointIcons)
        {
            GameObject dronic = PhotonNetwork.Instantiate(unitName, pointIcon.position, Quaternion.identity);
            scriptComponents = dronic.GetComponents<MonoBehaviour>();


            photonView = dronic.GetComponent<PhotonView>();

            if (photonView.IsMine)
            {
                foreach (MonoBehaviour component in scriptComponents)
                {
                    dronic.gameObject.tag = "Unit";
                    component.enabled = true;
                }
            }
            else
            {
                print(dronic.gameObject.name);
            }

            dronic.GetComponent<DroneController>().setPlatoon(this);
            units[idx] = dronic.transform;
            idx++;
        }

        platoonMaked = true;
    }

    public void setFormation(Formation formation)
    {
        dummyFormationObject.transform.position = formation.getCenterOfMass();
        dummyFormationObject.transform.rotation = Quaternion.identity;

        List<Vector3> formationPoints = formation.getPoints();

        int idx = 0;
        foreach (Transform pointIcon in pointIcons)
        {
            pointIcon.position = formationPoints[idx];
            pointIcon.parent = dummyFormationObject.transform;
            idx++;
        }

        dummyFormationObject.transform.position = transform.position;
        dummyFormationObject.transform.rotation = transform.rotation;

        foreach (Transform pointIcon in pointIcons)
        {
            pointIcon.parent = transform;
        }
    }

    public void Update()
    {
        if (platoonMaked == false)
        {
            return;
        }

        bool scaleFactorChanged = scaleFactorOld != scaleFactor;

        float _scaleFactor = 1.0f;
        if (scaleFactorChanged)
        {
            _scaleFactor = scaleFactor / scaleFactorOld;
            scaleFactorOld = scaleFactor;
        }

        Transform[] activeUnits = Array.FindAll(units, u => u.gameObject.activeSelf);

        int idx = 0;
        foreach (Transform unit in activeUnits)
        {
            NavMeshHit navmeshHit;
            int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");

            if (scaleFactorChanged)
            {
                pointIcons[idx].localPosition = pointIcons[idx].localPosition * _scaleFactor;
                Quaternion r = pointIcons[idx].localRotation;
            }

            pointIcons[idx].RotateAround(transform.position, Vector3.up, rotateFactor * Time.deltaTime);

            NavMesh.SamplePosition(pointIcons[idx].position, out navmeshHit, 1.0f, walkableMask);

            Debug.DrawLine(
                new Vector3(navmeshHit.position.x, 5.0f, navmeshHit.position.z),
                navmeshHit.position,
                Color.green);

            Vector3 target = navmeshHit.position;
            NavMeshAgent unitNavMeshAgent = unit.GetComponent<NavMeshAgent>();
            unitNavMeshAgent.speed = CalculateUnitSpeed(unitNavMeshAgent, target, mNavMeshAgent.speed);

            MoveController unitMoveController = unit.GetComponent<MoveController>();
            unitMoveController.setDynamicTarget = true;
            unitMoveController.setTarget(target);

            idx++;
        }
    }

    private float CalculateUnitSpeed(NavMeshAgent unitAgent, Vector3 formationPosition, float speed)
    {
        Vector3 dist = unitAgent.transform.InverseTransformPoint(formationPosition);
        float distToFormationPosZ = dist.z;
        float distToFormationPosX = dist.x;

        float mSpeedModifier;

        if (distToFormationPosZ > 0 && distToFormationPosZ > maxDrift)
        {
            mSpeedModifier = distToFormationPosZ / maxDrift;
        }
        else if (distToFormationPosZ < 0 && Mathf.Abs(distToFormationPosZ) > maxDrift && unitAgent.remainingDistance < maxDrift * 4.0f)
        {
            mSpeedModifier = maxDrift / Mathf.Abs(distToFormationPosZ);
        }
        else
        {
            mSpeedModifier = 1.0f;
        }

        if (distToFormationPosX > maxDrift)
        {
            mSpeedModifier = 1.0f;
        }

        if (mSpeedModifier > maxSpeedModifier)
        {
            mSpeedModifier = maxSpeedModifier;
        }

        return speed * mSpeedModifier;
    }

    public bool platoonDestroyed()
    {
        return Array.FindAll(units, u => u.gameObject.activeSelf).Length == 0;
    }
}
