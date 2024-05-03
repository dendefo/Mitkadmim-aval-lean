using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public PlayerBase playerBase;
    public TMP_Text goldText;

    public List<Transform> Spawners;
    public List<Enemy> PossibleEnemiesToSpawn;
    public float SpawnIntervalSeconds = 3f;
    public int MaxEnemies = 10;
    [SerializeField] private float spawnTimer = 3f;
    public int GoldAmount = 0;
    public float SecondsBetweenGoldIncome = 2f;
    [SerializeField] private float goldIncomeTimer = 2f;
    public float difficulryMultiplyer = 1;
    public GameObject pausePanel;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    private void OnEnable()
    {
        CameraMovement.CameraCreated += CameraCreated;
        GoldCoin.OnGoldCoinClick += GoldCoinClicked;
    }

    private void GoldCoinClicked(GoldCoin coin)
    {
        ChangeGoldAmount(coin.value);
        Destroy(coin.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(!IPausable.isPaused);
        }
        //Spawn enemies
        SpawnTimer();
        //Income gold
        GoldIncome();
    }
    private void SpawnTimer()
    {
        if (IPausable.isPaused) return;
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
        }
        else
        {
            if (Enemy.enemies.Count < MaxEnemies)
            {
                SpawnEnemy();
                spawnTimer = SpawnIntervalSeconds;
            }
        }
    }
    private void GoldIncome()
    {
        if (IPausable.isPaused) return;
        if (goldIncomeTimer > 0)
        {
            goldIncomeTimer -= Time.deltaTime;
        }
        else
        {
            ChangeGoldAmount(1);
            goldIncomeTimer = SecondsBetweenGoldIncome;
        }
    }
    private void ChangeGoldAmount(int amount)
    {
        GoldAmount += amount;
        goldText.text = GoldAmount.ToString();
    }
    private void SpawnEnemy()
    {
        if (Spawners.Count == 0 || PossibleEnemiesToSpawn.Count == 0)
        {
            Debug.LogError("No spawners or enemies to spawn", this);
            return;
        }
        var spawnerNumber = UnityEngine.Random.Range(0, Spawners.Count);
        var enemyNumber = UnityEngine.Random.Range(0, PossibleEnemiesToSpawn.Count);
        var enemy = Instantiate(PossibleEnemiesToSpawn[enemyNumber], Spawners[spawnerNumber].position, Quaternion.identity);
        enemy.AttackingTarget = playerBase.transform;
        enemy.Stats.Multiply(difficulryMultiplyer);
        enemy.Stats.Die += () => EnemyDead(enemy);
    }

    private void EnemyDead(Enemy enemy)
    {
        Instantiate(Resources.Load<GoldCoin>("GoldCoin"), enemy.transform.position, Quaternion.identity).value = enemy.data.GoldValue;
        difficulryMultiplyer *= 1.1f;
    }

    private void CameraCreated(Camera camera)
    {
        playerBase.hpBar.SetUp(camera.transform);
    }
    private void OnDisable()
    {
        CameraMovement.CameraCreated -= CameraCreated;
        GoldCoin.OnGoldCoinClick -= GoldCoinClicked;
    }

    public void Pause(bool isPaused)
    {
        IPausable.Pause(isPaused);
        pausePanel.SetActive(isPaused);
    }
}
