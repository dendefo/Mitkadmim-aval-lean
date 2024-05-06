using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager : MonoBehaviour, ISavable
{

    public static GameManager instance;
    public PlayerBase playerBase;
    public TMP_Text goldText;

    public List<Transform> Spawners;
    public List<Enemy> PossibleEnemiesToSpawn;
    public float TimeWithouPasuse;
    public float SpawnIntervalSeconds = 3f;
    public int MaxEnemies = 10;
    [SerializeField] private float spawnTimer = 3f;
    public int GoldAmount = 0;
    public float SecondsBetweenGoldIncome = 2f;
    [SerializeField] private float goldIncomeTimer = 2f;
    public float difficulryMultiplyer = 1;
    public GameObject pausePanel;
    private int enemiesKilled = 0;
    private void Awake()
    {
        IPausable.Pause(false);
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
        ISavable.SaveEvent += SaveData;
        ISavable.LoadEvent += LoadData;
        CameraMovement.CameraCreated += CameraCreated;
        GoldCoin.OnGoldCoinClick += GoldCoinClicked;
        playerBase.stats.Die += EndOfGame;
        
    }

    private void EndOfGame()
    {
        EndGameStats.TimePlayed = TimeWithouPasuse;
        EndGameStats.GoldCollected = GoldAmount;
        EndGameStats.EnemiesKilled = enemiesKilled;
        SceneManager.LoadScene(2);

    }

    private void Start()
    {
        if (ISavable.WantsToLoad)
        {
            Creature.creatures.ForEach(creature => Destroy(creature.gameObject));
            Creature.creatures.Clear();
            LoadGame();
        }
    }

    private void LoadData(Dictionary<string, object> dictionary)
    {
        GoldAmount = int.Parse(dictionary["GoldAmount"].ToString());
        difficulryMultiplyer = float.Parse(dictionary["difficulryMultiplyer"].ToString());
        spawnTimer = float.Parse(dictionary["SpawnTimer"].ToString());
        goldIncomeTimer = float.Parse(dictionary["GoldIncomeTimer"].ToString());
        playerBase.stats.CurrentHP = int.Parse(dictionary["PlayerBaseHp"].ToString());
        playerBase.stats.MaxHP = int.Parse(dictionary["PlayerBaseMaxHp"].ToString());
        playerBase.stats.Damage = int.Parse(dictionary["PlayerBaseDamage"].ToString());
        TimeWithouPasuse = float.Parse(dictionary["TimePlayed"].ToString());
        goldText.text = GoldAmount.ToString();
        enemiesKilled = int.Parse(dictionary["EnemiesKilled"].ToString());
        foreach (var data in dictionary.Keys)
        {
            if (!data.Contains("Creature")) continue;
            JsonSerializerSettings settings = new();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            var creatureData = JsonConvert.DeserializeObject<CreatureSaveData>(dictionary[data].ToString(), settings);
            Debug.Log(creatureData.Name);
            var creature = Instantiate(Resources.Load<Creature>("Creatures/" + creatureData.Name), creatureData.Position, Quaternion.identity);
            creature.Load(creatureData);
            if (creature is Enemy enemy)
            {
                enemy.AttackingTarget = playerBase.transform;
                enemy.Stats.Multiply(difficulryMultiplyer);
                enemy.Stats.Die += () => EnemyDead(enemy);
            }
        }
        ISavable.WantsToLoad = false;
    }

    //Fill this method with data you want to save
    private void SaveData(Dictionary<string, object> dictionary)
    {
        dictionary["GoldAmount"] = GoldAmount.ToString();
        dictionary["difficulryMultiplyer"] = difficulryMultiplyer.ToString();
        dictionary["SpawnTimer"] = spawnTimer.ToString();
        dictionary["GoldIncomeTimer"] = goldIncomeTimer.ToString();
        dictionary["PlayerBaseHp"] = playerBase.stats.CurrentHP.ToString();
        dictionary["PlayerBaseMaxHp"] = playerBase.stats.MaxHP.ToString();
        dictionary["PlayerBaseDamage"] = playerBase.stats.Damage.ToString();
        dictionary["TimePlayed"] = TimeWithouPasuse.ToString();
        dictionary["EnemiesKilled"] = enemiesKilled.ToString();
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
        if (!IPausable.isPaused) { TimeWithouPasuse += Time.deltaTime; }
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
        difficulryMultiplyer *= 1.05f;
        enemiesKilled++;
        enemy.Stats.Die -= () => EnemyDead(enemy);
    }

    private void CameraCreated(Camera camera)
    {
        playerBase.hpBar.SetUp(camera.transform);
    }
    private void OnDisable()
    {
        CameraMovement.CameraCreated -= CameraCreated;
        GoldCoin.OnGoldCoinClick -= GoldCoinClicked;
        ISavable.SaveEvent -= SaveData;
        ISavable.LoadEvent -= LoadData;
        playerBase.stats.Die -= EndOfGame;
    }

    public void Pause(bool isPaused)
    {
        IPausable.Pause(isPaused);
        pausePanel.SetActive(isPaused);
    }

    public void SaveGame()
    {
        ISavable.Save();
        SceneManager.LoadScene(0);
    }

    private void LoadGame()
    {
        ISavable.Load();
    }
}
public struct EndGameStats
{
    public static float TimePlayed;
    public static int GoldCollected;
    public static int EnemiesKilled;
}