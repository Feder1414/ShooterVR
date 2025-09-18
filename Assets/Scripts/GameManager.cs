using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;

    public SpawnManager spawnManager;
    public  GameManager instance;

    public BuffManager buffManager;

    public GameObject buffCanvas;

    [SerializeField] GameObject gameOverCanvas;

    void Awake()
    {
      
        
    }


    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is not assigned in GameManager.");
        }

        player.GetComponent<Killable>().OnDied += OnPlayerDied;

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
        var playerKillable = player.GetComponent<Killable>();
        playerKillable.Heal(playerKillable.GetBaseLife()); // Heal to full life at the end of the wave
        



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

    public void StartLoopSpawn()
    {
        if (spawnManager != null)
        {
            spawnManager.StartLoopSpawn();
            Debug.Log("Starting loop spawn from GameManager.");
        }
        else
        {
            Debug.LogError("SpawnManager reference is not assigned in GameManager.");
        }
    }

    void OnPlayerDied(Killable killable)
    {
        Debug.Log("Player died. Stopping spawns and killing all enemies.");
        if (spawnManager != null)
        {
            spawnManager.spawnEnabled = false;
        }
        else
        {
            Debug.LogError("SpawnManager reference is not assigned in GameManager.");
        }

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }
        else
        {
            Debug.LogError("GameOverCanvas reference is not assigned in GameManager.");
        }

        killAllEnemies();
        spawnManager.StopSpawning();

    }

    void killAllEnemies()
    {
        Killable[] killables = FindObjectsOfType<Killable>();
        foreach (Killable killable in killables)
        {
            if (killable.GetTeam() == Killable.Team.Enemy)
            {
                Destroy(killable.gameObject);
            }
        }
    }
    
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
}
