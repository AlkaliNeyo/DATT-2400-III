using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject warningPrefab;
    public GameObject laserPrfab;
    public Enemy enemyPrfab;
    public PlayerMovement player;
    public Transform[] spawnPoints;
    private List<Enemy> activeBots = new List<Enemy>();
    public AudioSource audioName;
  
    public float warningInterval = 3f;
    public float laserInterval = 0.01f;

    public float spawnInterval = 5f;
    public int laserCount = 0;
    public int botCount = 0;

    public bool canSpawn = true;

    //camera Shake
    CameraController cameraController;
   
    //scaling level
    public float initialSpawnInterval = 4f;   // start spawn rate
    public float minSpawnInterval = 0.5f;   // 
    public float intervalDecreaseRate = 0.05f; 
    public int initialLaserCount = 1; 
    public float maxDifficulty = 1;   
    public float growthRate =0.9f;
    Vector2 dir;
    public float angleOffset = 5f;

    //Dictionary<Transform, Vector2> spawnPointDirections = new Dictionary<Transform, Vector2>();  // storing each dir with the key being spawnPoint
    //Dictionary<Transform, float> spawnPointAngles = new Dictionary<Transform, float>();


    private void Awake() {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        cameraController = Camera.main.GetComponent<CameraController>();
    }
    
    private void Start()
    {
        // start loop for spawning
        StartCoroutine(SpawnObstacle());
        StartCoroutine(SpawnBots());
    }

    void Update() {
        float t = Mathf.Clamp01(player.getVelocity().x / player.getMaxVelocity());   
        float difficulty = maxDifficulty * (Mathf.Exp(growthRate * t) - 1) / (Mathf.Exp(growthRate) - 1); //chatgpt helped me get the math
        UpdateDifficulty(difficulty);

        if(activeBots.Count() == 0) {
            canSpawn = true;
        } 
    }

    private void UpdateDifficulty(float d) {
        spawnInterval = Mathf.Lerp(2f, 0.5f, d / maxDifficulty);
        laserCount = Mathf.FloorToInt(Mathf.Lerp(1, 10, d / maxDifficulty));
        botCount = Mathf.FloorToInt(Mathf.Lerp(1, 10, d / maxDifficulty));

    }

    private List<Transform> GetRandomSpawnPoints(int count) {
        List<Transform> selected = new List<Transform>();
        List<Transform> freeSpawn = new List<Transform>(spawnPoints);  // to remove dups

        for(int i=0; i<count; i++) {
            if (freeSpawn.Count == 0) break;

            int index = Random.Range(0, freeSpawn.Count());
            selected.Add(freeSpawn[index]);

            freeSpawn.RemoveAt(index);
        }

        return selected;
    }

    private IEnumerator SpawnObstacle()
    {
        yield return new WaitForSecondsRealtime(spawnInterval*2);
            while (true)
            {
                
                float angle;
                
                float acceleration = Mathf.Clamp(player.getAcceleration(), 0f, 2f); 
                laserCount = Mathf.Clamp(Mathf.FloorToInt(1 + (3 * Mathf.Pow(acceleration / 5f, 2))), 1, 4); 
                    Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count())];
                    dir = (player.transform.position - spawnPoint.transform.position).normalized;
                    angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
                    //print(angle);
                    angle += Random.Range(-angleOffset, angleOffset);
                    GameObject warning = Instantiate(warningPrefab, spawnPoint.position, Quaternion.identity);
                    warning.transform.rotation = Quaternion.Euler(0, 0, -angle);


                    //camera shake
                    cameraController.StartShaking();
                    audioName.Play();
                    // Wait for the warning duration
                    yield return new WaitForSeconds(warningInterval);

                    Destroy(warning); 

                    // Spawn obstacle at the spawn point
                    GameObject obstacle = Instantiate(laserPrfab, spawnPoint.position, Quaternion.identity);
                    obstacle.transform.rotation = Quaternion.Euler(0, 0, -angle);
    
                    // Wait for the laser duration
                    yield return new WaitForSecondsRealtime(laserInterval);
                    Destroy(obstacle);
                    cameraController.StopShaking();
                    // Wait for the next spawn
                //}
                
                yield return new WaitForSecondsRealtime(spawnInterval*2);
            }
    }

    public void RemoveBot(Enemy bot) {
        activeBots.Remove(bot); // Remove the bot from the list
        
    }
    private IEnumerator SpawnBots()
{               
    yield return new WaitForSeconds(spawnInterval);
    while (true)
    {

        if(canSpawn) {
            yield return new WaitForSeconds(spawnInterval*5);
        }
        // Scale the bot count based on the player's acceleration
        float velo = Mathf.Clamp(player.getVelocity().x, 0f, 120f); 
        botCount = Mathf.Clamp(Mathf.FloorToInt(1 + (3 * Mathf.Pow(velo / 120f, 2))), 1, 4); 
        print(botCount);
        if ((activeBots.Count < botCount) && canSpawn)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Enemy bot = Instantiate(enemyPrfab, spawnPoint.position, Quaternion.identity);

            activeBots.Add(bot); // Track the bot
            bot.GetComponent<Enemy>().SetSpawner(this); // Pass spawner reference for cleanup
        } else if (activeBots.Count() >= botCount ) {
            canSpawn = false;
        }
        yield return new WaitForSeconds(spawnInterval);
    
    }
}

}
