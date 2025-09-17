using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [Header("Spawn position and enemies")]
    public Transform[] spawnPoints;
    public GameObject[] enemyPrefabs;

    public WaveAction[] waveActionsPrefabs;

    public int baseEnemyCount = 5;

    public float baseSpawnRate = 2f;

    public float delayBetweenWaves = 5f;

    public float incrementFactorWave = 1.1f;
    public float rateGrowth = 1.0f;

    public float difficultLifeFactor = 1.05f;
    public float difficultDamageFactor = 1.03f;

    public float speedIncreaseSum = 0.05f;

    public float maxEnemyFireRate = 1f;

    public float decreaseEnemyFireRate = 0.01f;

    public float aliveEnemies = 0;

    public bool spawnEnabled = true;

    public int currentWave = 0;

    private Dictionary<string, Transform> spawnPointDictionary;

    public bool canContinue = false;

    public event Action<int> OnEnemyKilled;

    public event Action<int> OnWaveStarted;

    public event Action<int> OnWaveEnded;

    public event Action<int> OnEnemySpawned;

    

    public void Awake()
    {
        spawnPointDictionary = new Dictionary<string, Transform>();
        foreach (var p in FindObjectsOfType<SpawnPointGizmos>())
        {
            var spawnPointGizmo = p.GetComponent<SpawnPointGizmos>();
            spawnPointDictionary[spawnPointGizmo.identifier] = p.transform;

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // if (spawnEnabled)
        // {
        //     StartCoroutine(WaveLoopSpawn());
        // }
    }

    public void StartLoopSpawn()
    {
        if (spawnEnabled)
        {
            StartCoroutine(WaveLoopSpawn());
            Debug.Log("SpawnManager: Wave loop spawn started.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator WaveLoopSpawn()
    {
        
        while (true)
        {
            OnWaveStarted?.Invoke(currentWave);
            for (int i = 0; i < currentWave + 1; i++)
            {
                WaveAction waveAction = waveActionsPrefabs[UnityEngine.Random.Range(0, waveActionsPrefabs.Length)];
                Debug.Log("Starting Wave: " + waveAction.waveName);
                yield return StartCoroutine(waveAction.Run(this));

            }


            while (aliveEnemies > 0)
            {
                yield return null;
            }
            

            currentWave++;
            canContinue = false;
            OnWaveEnded?.Invoke(currentWave);
            IncreaseFactorDifficult();

            Debug.Log("Wave " + currentWave + " ended. Waiting for player to continue...");
            yield return new WaitForSeconds(delayBetweenWaves);
         
            while (!canContinue)
            {
                yield return null;
            }
        }
        // int currentEnemyCount = baseEnemyCount;
        // float currentDifficultLifeFactor = difficultLifeFactor;

        // while (true)
        // {
        //     for (int i = 0; i < currentEnemyCount; i++)
        //     {
        //         yield return SpawnEnemy();

        //     }

        //     while (aliveEnemies > 0)
        //     {
        //         yield return null; // Wait until all enemies are dead
        //     }

        //     currentEnemyCount = Mathf.FloorToInt(currentEnemyCount * incrementFactorWave);
        //     baseSpawnRate *= rateGrowth;
        //     currentDifficultLifeFactor *= incrementFactorWave;
        //     yield return new WaitForSeconds(delayBetweenWaves);
        // }

    }


    IEnumerator SpawnEnemy()
    {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        GameObject enemyPrefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];
        var enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        enemy.GetComponent<Killable>().FactorIncreaseLife(difficultLifeFactor);
        aliveEnemies++;

        yield return new WaitForSeconds(baseSpawnRate);
    }

    public IEnumerator SpawnStep(WaveAction.WaveActionStep step)
    {
    
        for (int i = 0; i < step.count; i++)
        {
            if (step.randomSpawnPoint)
            {
                Vector3 randomPosition = RandomPointInArea(step.spawnAreaObject);
                var enemy = Instantiate(step.enemyPrefab, randomPosition, Quaternion.identity);

                aliveEnemies++;
                OnEnemySpawned?.Invoke((int)aliveEnemies);
                var enemyKillable = enemy.GetComponent<Killable>();
                if (enemyKillable != null)
                {
                    SetDifficulFactorEnemy(enemyKillable);
                    enemyKillable.OnDied += OnEnemyDied;
                }
                else
                {
                    Debug.LogWarning("The spawned enemy does not have a Killable component.");
                }

                if (step.duration > 0)
                {
                    yield return new WaitForSeconds(step.duration);
                }
            }
            else
            {
                if (step.specificSpawnPointIds.Length > 0 && step.specificSpawnPointIds.Length == step.count)
                {
                    Transform spawnPoint = spawnPointDictionary[step.specificSpawnPointIds[i]];

                    if (spawnPoint == null)
                    {
                        Debug.LogWarning("Spawn point with ID " + step.specificSpawnPointIds[i] + " not found. Using a random spawn point instead.");
                        continue;
                    }

                    var enemy = Instantiate(step.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                    aliveEnemies++;
                    OnEnemySpawned?.Invoke((int)aliveEnemies);
                    var enemyKillable = enemy.GetComponent<Killable>();
                    if (enemyKillable != null)
                    {
                        SetDifficulFactorEnemy(enemyKillable);


                        enemyKillable.OnDied += OnEnemyDied;
                    }
                    else
                    {
                        Debug.LogWarning("The spawned enemy does not have a Killable component.");
                    }

                    if (step.duration > 0)
                    {
                        yield return new WaitForSeconds(step.duration);
                    }

                }
                else
                {
                    Debug.LogWarning("No specific spawn points defined for the step: " + step.actionName);
                }
            }



        }
    }

    void IncreaseFactorDifficult()
    {
        difficultLifeFactor *= difficultLifeFactor;
        difficultDamageFactor *= difficultDamageFactor;
        speedIncreaseSum += 0.05f;
        decreaseEnemyFireRate += 0.01f;
        


    }

    void SetDifficulFactorEnemy(Killable enemyKillable)
    {
        if (enemyKillable != null)
        {
            enemyKillable.FactorIncreaseLife((difficultLifeFactor));
            enemyKillable.IncreaseFactorDamage(difficultDamageFactor);
            enemyKillable.IncreaseSpeed(0.05f * currentWave);
            var fireRate = enemyKillable.GetFireRate();
            if (fireRate > maxEnemyFireRate)
            {
                enemyKillable.IncreaseFireRate(decreaseEnemyFireRate);
            }
        }
    }

    void OnEnemyDied(Killable enemy)
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        if (enemy != null)
        {
            enemy.OnDied -= OnEnemyDied;
        }

        OnEnemyKilled?.Invoke((int) aliveEnemies);
    }

    Vector3 RandomPointInArea(GameObject spawnAreaObject)
    {
        Transform center = spawnAreaObject.GetComponent<SpawnAreaGizmo>().areaCenter;
        Vector3 size = spawnAreaObject.GetComponent<SpawnAreaGizmo>().areaSize;
        return center.position + new Vector3(
            UnityEngine.Random.Range(-size.x / 2, size.x / 2),
            UnityEngine.Random.Range(-size.y / 2, size.y / 2),
            UnityEngine.Random.Range(-size.z / 2, size.z / 2)
        );
    }
    


}
