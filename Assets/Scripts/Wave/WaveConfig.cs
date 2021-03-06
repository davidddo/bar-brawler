﻿namespace Wave
{
    using UnityEngine;

    /// <summary>
    /// Class <c> WaveConfig</c> represents a config file for one or more waves.
    /// The config includes the <see cref="round"/> number, the difficulty, the
    /// enemy prefab as well as the <see cref="EnemyConfig"/>. Is the current
    /// wave count <see cref="WaveSpawner.Rounds"/> equals to set <see cref="round"/>
    /// number the specific config gets loaded into the wave spawner and applied to all
    /// new created enemies.
    /// </summary>
    [CreateAssetMenu(fileName = "New Wave Config", menuName = "Configs/Wave Config")]
    public class WaveConfig : ScriptableObject
    {
        public int round;
        public Difficulty difficulty;
        public GameObject enemy;

        [Header("Enemy Config")]
        public EnemyConfig enemyConfig;
    }
}