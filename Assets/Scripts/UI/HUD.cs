using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class HUD : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TMP_Text enemiesText;
    [SerializeField] TMP_Text waveText;
    void Start()
    {
        var spawnManager = GameManager.instance.spawnManager;
        spawnManager.onEnemyKilled += (aliveEnemies) =>
        {
            enemiesText.text = "Enemies: \n" + aliveEnemies;
        };
        spawnManager.onWaveStarted += (waveNumber) =>
        {
            waveText.text = "Wave: \n" + waveNumber;
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
