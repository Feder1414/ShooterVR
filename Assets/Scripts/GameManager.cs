using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;

    public SpawnManager spawnManager;
    public static GameManager instance;

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

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
