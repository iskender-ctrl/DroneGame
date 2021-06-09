using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalLightController : MonoBehaviour
{
    public enum MomentOfTheDay
    {
        Morning,
        Noon,
        Evening,
        Night
    }

    public MomentOfTheDay momentOfTheDay;

    private Light globalLight;

    public void Start()
    {
        globalLight = GetComponent<Light>();
    }

    public void Update()
    {
        switch (momentOfTheDay)
        {
            case MomentOfTheDay.Morning:
                globalLight.intensity = 0.5f;
                globalLight.color = new Color(0.82f, 0.82f, 0.82f);
                break;
            case MomentOfTheDay.Noon:
                globalLight.intensity = 1.1f;
                globalLight.color = new Color(1.0f, 0.99f, 0.93f);
                break;
            case MomentOfTheDay.Evening:
                globalLight.intensity = 0.7f;
                globalLight.color = new Color(1.0f, 0.66f, 0.74f);
                break;
            case MomentOfTheDay.Night:
                globalLight.intensity = 0.1f;
                globalLight.color = new Color(0.19f, 0.18f, 0.14f);
                break;
        }
    }

    public float getGlobalIntensity()
    {
        return globalLight.intensity;
    }
}
