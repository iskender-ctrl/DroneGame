using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HeadlightController : MonoBehaviour
{
    public string globalLightTag = "GlobalLight";
    public string headlightPath = "";
    public float headlightSensorValue = 0.8f;

    public void Update()
    {
        GameObject globalLightGO = GameObject.FindGameObjectWithTag(globalLightTag);
        if(globalLightGO == null) {
            return;
        }

        Light globalLight = GameObject.FindGameObjectWithTag(globalLightTag).GetComponent<Light>();

        if (globalLight.intensity <= headlightSensorValue)
        {
            transform.Find(headlightPath).gameObject.SetActive(true);
        }
        else
        {
            transform.Find(headlightPath).gameObject.SetActive(false);
        }
    }
}
