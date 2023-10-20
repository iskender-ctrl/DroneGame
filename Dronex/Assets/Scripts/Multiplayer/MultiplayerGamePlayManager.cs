using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerGamePlayManager : MonoBehaviour
{
    [SerializeField] GameObject AloneSecretPlayer;
    void Start()
    {
        float randomPos = Random.Range(-10, 10);
        Instantiate(AloneSecretPlayer, new Vector3(randomPos, 1.083333f, randomPos), Quaternion.identity);
    }
}
