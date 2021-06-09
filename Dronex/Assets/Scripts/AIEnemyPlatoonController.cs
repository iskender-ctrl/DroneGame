using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Formation;

public class AIEnemyPlatoonController : MonoBehaviour
{
    public string castleTag = "Castle";
    public string playerTag = "Player";
    private AIEnemySystemController enemyAIEnemySystemController;
    private FormationData formationData;
    private MoveController moveController;
    private MoveController.MovementMode movementMode;
    private Transform player;
    private int formationTimer = 0;
    public int formationChangingPeriodInSeconds = 30;

    public void setAIEnemySystemController(AIEnemySystemController newAIEnemySystemController)
    {
        enemyAIEnemySystemController = newAIEnemySystemController;
    }

    void Start()
    {
        moveController = GetComponent<MoveController>();
        movementMode = MoveController.MovementMode.Walk;
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
    }

    void Update()
    {
        if (enemyAIEnemySystemController != null)
        {
            formationData = enemyAIEnemySystemController.getFormationData();
            moveController.setLookPoint(player.position);
            moveController.setMovementMode(movementMode);
            moveController.setTarget(player.position);

            formationTimer = (int)(Time.time);
            if((formationTimer % formationChangingPeriodInSeconds) == 0) {
                PlatoonController platoonController = GetComponent<PlatoonController>();
                int randomFormationIndex = (new System.Random()).Next(0, formationData.formations.Count - 1);
                platoonController.setFormation(formationData.formations[randomFormationIndex]);
            }
        }
    }
}
