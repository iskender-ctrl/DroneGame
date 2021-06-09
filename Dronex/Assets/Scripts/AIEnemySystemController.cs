using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Formation;

public class AIEnemySystemController : MonoBehaviour
{
    public int waveStartingPeriodInSeconds = 30;
    public Transform[] enemySpawnPoints;
    public Transform enemyPrefab;
    private int waveTimer = 0;
    private FormationData formationData;
    private bool waveProgressing = false;
    private PlatoonController platoonController;

    void Start()
    {
        loadStandartFormationData();
        //waveDedectAndFight(true);
    }

    void Update()
    {
        waveDedectAndFight();
    }

    public FormationData getFormationData() {
        return formationData;
    }

    protected void waveDedectAndFight(bool forced = false)
    {
        if(platoonController != null && platoonController.platoonDestroyed()) {
            waveProgressing = false;
        }

        if(!waveProgressing) {
            waveTimer = (int)(Time.time);
            if (forced || (!forced && (waveTimer == 10 || ((waveTimer % waveStartingPeriodInSeconds) == 0))))
            {
                waveProgressing = true;
                // Start fight wave
                // Spawn enemy platoon
                int randomSpawnIndex = (new System.Random()).Next(0, enemySpawnPoints.Length - 1);
                Transform enemyPlatoon = Instantiate(enemyPrefab, enemySpawnPoints[randomSpawnIndex].position, Quaternion.identity);
                platoonController = enemyPlatoon.GetComponent<PlatoonController>();

                int randomFormationIndex = (new System.Random()).Next(0, formationData.formations.Count - 1);
                platoonController.Start();
                platoonController.setFormation(formationData.formations[randomFormationIndex]);
                platoonController.makePlatoon();

                AIEnemyPlatoonController enemyAIEnemyPlatoonController = enemyPlatoon.GetComponent<AIEnemyPlatoonController>();
                enemyAIEnemyPlatoonController.setAIEnemySystemController(this);
            }
        }
    }

    protected void loadStandartFormationData()
    {
        formationData = JsonUtility.FromJson<FormationData>(Common.DataService.LoadDefaultGameData("standartFormations.json"));
    }
}
