﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState { SPAWNING, WAITING, COUNTING }


    public Transform enemyPrefab;
    public Transform[] spawnPoints;

    public bool enableWaves;
    public float timeBetweenWaves = 31f;

    public Text stateOfGameText;
    public Text skipCountdownInformation;

    public SpawnState state = SpawnState.COUNTING;

    private int waveIndex = 0;
    private float waveCountdown;
    private float searchCountdown = 1f;

    void Start()
    {
        waveCountdown = timeBetweenWaves;
        //state = SpawnState.COUNTING;
    }

    void Update()
    {
        if (enableWaves)
        {
            if (state == SpawnState.WAITING)
            {
                if (!IsEnemyAlive)
                {
                    Start();
                }
                else
                {
                    return;
                }
            }

            if (waveCountdown <= 0f || Input.GetKeyDown(KeyCode.LeftShift))
            {
                waveCountdown = 0f;
                skipCountdownInformation.gameObject.SetActive(false);
                if (state != SpawnState.SPAWNING)
                {
                    StartCoroutine(SpawnWave());
                }
            }
            else
            {
                skipCountdownInformation.gameObject.SetActive(true);
                waveCountdown -= Time.deltaTime;
                if (waveCountdown > 0f)
                {
                    stateOfGameText.text = Mathf.Floor(waveCountdown).ToString();
                }
            }
        }
    }

    private IEnumerator SpawnWave()
    {
        waveIndex++;
        stateOfGameText.text = waveIndex.ToString();
        state = SpawnState.SPAWNING;

        for (int i = 0; i < waveIndex * 2; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1f);

        }

        state = SpawnState.WAITING;
        yield break;
    }

    private void SpawnEnemy()
    {
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
    }

    private bool IsEnemyAlive
    {
        get
        {
            searchCountdown -= Time.deltaTime;
            if (searchCountdown <= 0f)
            {
                searchCountdown = 1f;
                if (GameObject.FindGameObjectWithTag("Enemy") == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}