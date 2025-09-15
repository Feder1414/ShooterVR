using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;

    public SpawnManager spawnManager;
    public static GameManager instance;

    public BuffManager buffManager;

    public GameObject buffCanvas;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is not assigned in GameManager.");
        }

        if (spawnManager == null)
        {
            Debug.LogError("SpawnManager reference is not assigned in GameManager.");
        }



    }

    void OnEnable()
    {
        if (buffManager != null)
        {
            buffManager.OnbuffApplied += OnBuffApplied;
        }
        else
        {
            Debug.LogError("BuffManager reference is not assigned in GameManager.");
        }

        if (spawnManager != null)
        {
            spawnManager.OnWaveEnded += OnWaveEnded;
        }
        else
        {
            Debug.LogError("SpawnManager reference is not assigned in GameManager.");
        }
    }

    void OnDisable()
    {
        if (buffManager != null)
        {
            buffManager.OnbuffApplied -= OnBuffApplied;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnWaveEnded(int currentWave)
    {
        Debug.Log("Wave ended event received in GameManager for wave: " + currentWave);
        if (buffCanvas != null)
        {
            buffCanvas.SetActive(true);
            Debug.Log("Wave ended. BuffCanvas enabled.");
        }
        else
        {
            Debug.LogError("BuffCanvas reference is not assigned in GameManager.");
        }



    }
    
    void OnBuffApplied()
    {
        if (buffCanvas != null)
        {
            buffCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("BuffCanvas reference is not assigned in GameManager.");
        }
        spawnManager.canContinue = true;
    }
    
}
