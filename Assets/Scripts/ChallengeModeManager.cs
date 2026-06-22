using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChallengeModeManager : MonoBehaviour
{
    [System.Serializable]
    public class ChallengeWave
    {
        public string waveName;

        public int meleeEnemyCount;
        public int archerEnemyCount;
        public int flyingEnemyCount;
        public int miniBossCount;
    }

    [Header("Player")]
    public Transform player;

    [Header("Enemy Prefabs")]
    public GameObject meleeEnemyPrefab;
    public GameObject archerEnemyPrefab;
    public GameObject flyingEnemyPrefab;
    public GameObject miniBossPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Waves")]
    public ChallengeWave[] waves;
    public float timeBetweenWaves = 3f;
    public float spawnDelay = 0.4f;

    [Header("Score")]
    public int meleeEnemyScore = 10;
    public int archerEnemyScore = 15;
    public int flyingEnemyScore = 20;
    public int miniBossScore = 100;

    [Header("UI")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemiesLeftText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    [Header("Result Panel")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    private readonly List<GameObject> aliveEnemies = new List<GameObject>();

    private int currentWaveIndex = -1;
    private int score;
    private float challengeTime;
    private bool challengeActive;
    private bool spawningWave;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        score = 0;
        challengeTime = 0f;
        challengeActive = true;

        UpdateUI();

        StartCoroutine(StartNextWaveRoutine());
    }

    private void Update()
    {
        if (!challengeActive) return;

        challengeTime += Time.deltaTime;

        RemoveDeadEnemiesFromList();
        UpdateUI();

        if (!spawningWave && aliveEnemies.Count <= 0 && currentWaveIndex >= 0)
        {
            if (currentWaveIndex >= waves.Length - 1)
            {
                FinishChallenge();
            }
            else
            {
                StartCoroutine(StartNextWaveRoutine());
            }
        }
    }

    private IEnumerator StartNextWaveRoutine()
    {
        spawningWave = true;

        currentWaveIndex++;

        if (waveText != null)
        {
            waveText.text = "Волна " + (currentWaveIndex + 1);
        }

        yield return new WaitForSeconds(timeBetweenWaves);

        ChallengeWave wave = waves[currentWaveIndex];

        yield return StartCoroutine(SpawnEnemies(wave.meleeEnemyCount, meleeEnemyPrefab));
        yield return StartCoroutine(SpawnEnemies(wave.archerEnemyCount, archerEnemyPrefab));
        yield return StartCoroutine(SpawnEnemies(wave.flyingEnemyCount, flyingEnemyPrefab));
        yield return StartCoroutine(SpawnEnemies(wave.miniBossCount, miniBossPrefab));

        spawningWave = false;
    }

    private IEnumerator SpawnEnemies(int count, GameObject prefab)
    {
        if (prefab == null) yield break;
        if (spawnPoints == null || spawnPoints.Length == 0) yield break;

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject enemy = Instantiate(
                prefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            SetupEnemy(enemy);

            aliveEnemies.Add(enemy);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SetupEnemy(GameObject enemy)
    {
        if (enemy == null || player == null) return;

        EnemyShooter enemyShooter = enemy.GetComponent<EnemyShooter>();
        if (enemyShooter != null)
        {
            enemyShooter.player = player;
        }

        FlyingMeleeEnemy flyingMeleeEnemy = enemy.GetComponent<FlyingMeleeEnemy>();
        if (flyingMeleeEnemy != null)
        {
            flyingMeleeEnemy.SetTarget(player);
        }

        MiniBossController miniBossController = enemy.GetComponent<MiniBossController>();
        if (miniBossController != null)
        {
            miniBossController.player = player;
        }
    }

    private void RemoveDeadEnemiesFromList()
    {
        for (int i = aliveEnemies.Count - 1; i >= 0; i--)
        {
            if (aliveEnemies[i] == null)
            {
                aliveEnemies.RemoveAt(i);
                AddScoreForKill();
            }
        }
    }

    private void AddScoreForKill()
    {
        score += 10;
    }

    private void UpdateUI()
    {
        if (waveText != null)
        {
            if (currentWaveIndex < 0)
            {
                waveText.text = "Подготовка";
            }
            else
            {
                waveText.text = "Волна " + (currentWaveIndex + 1) + " / " + waves.Length;
            }
        }

        if (enemiesLeftText != null)
        {
            enemiesLeftText.text = "Врагов: " + aliveEnemies.Count;
        }

        if (scoreText != null)
        {
            scoreText.text = "Очки: " + score;
        }

        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(challengeTime / 60f);
            int seconds = Mathf.FloorToInt(challengeTime % 60f);

            timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }

    private void FinishChallenge()
    {
        challengeActive = false;

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (resultText != null)
        {
            resultText.text =
                "Испытание завершено!\n" +
                "Очки: " + score + "\n" +
                "Время: " + timerText.text;
        }

        Time.timeScale = 0f;
    }

    public void RestartChallenge()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}