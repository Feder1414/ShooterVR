using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWaveAction", menuName = "Wave Action", order = 0)]
public class WaveAction : ScriptableObject
{
    [Header("Meta")]
    public string waveName;

    public float preDelay = 0f;

    public bool sequential = true;      // secuencial o paralelo

    [System.Serializable]
    public class WaveActionStep
    {   


        public GameObject enemyPrefab;
        public string actionName;
        public float duration;

        public int count = 1;

        public bool randomSpawnPoint = true;

        //public Transform[] specificSpawnPoints;
        public string[] specificSpawnPointIds;


        public GameObject spawnAreaObject;

    }

    public List<WaveActionStep> steps = new List<WaveActionStep>();
    
    
    public IEnumerator Run(SpawnManager spawnManager)
    {
        yield return new WaitForSeconds(preDelay);

        foreach (WaveActionStep step in steps)
        {
            Debug.Log("Executing step: " + step.actionName + " with " + step.count + " enemies."); 
            if (sequential)
            {

                yield return spawnManager.SpawnStep(step);



            }
            else
            {
                // Parallel execution
                List<Coroutine> coroutines = new List<Coroutine>();

                coroutines.Add(spawnManager.StartCoroutine(spawnManager.SpawnStep(step)));

            }
        }
    }

}
