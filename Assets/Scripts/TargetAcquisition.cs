﻿using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using Utils;

public class TargetAcquisition : MonoBehaviour
{
    #region Singelton

    public static TargetAcquisition instance;
    public PlayerInputActions inputActions;

    void Awake()
    {
        instance = this;
        inputActions = new PlayerInputActions();
        inputActions.Crosshair.ToggleCrosshair.performed += ToggleTargetAcquisition;
        inputActions.Crosshair.SelectLast.performed += SelectLast;
        inputActions.Crosshair.SelectNext.performed += SelectNext;
        inputActions.Crosshair.SelectNearest.performed += SelectNearest;
        inputActions.Crosshair.Unselect.performed += Unselect;
    }

    #endregion;

    public Enemy CurrentEnemy { get; protected set; }
    private List<Enemy> enemies = new List<Enemy>();
    private int currentIndex;

    private float minDistance = Mathf.Infinity;

    public bool IsEnabled { get; set; } = false;

    void Start()
    {
        FunctionPeriodic.Create(() =>
        {
            UpdateEnemies();
            if (enemies.Count == 0 && IsEnabled)
            {
                Toggle();
            }

        }, 1f);
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    public void ToggleTargetAcquisition(CallbackContext context)
    {
        Toggle();
    }

    public void SelectNearest(CallbackContext context)
    {
        if (IsEnabled) SelectClosestEnemy();
    }

    public void SelectLast(CallbackContext context)
    {
        if (IsEnabled) SelectLastEnemy();
    }

    public void SelectNext(CallbackContext context)
    {
        if (IsEnabled) SelectNextEnemy();
    }

    public void Unselect(CallbackContext context)
    {
        if (IsEnabled) UnselectCurrentEnemy();
    }

    public void Toggle()
    {
        if (!GameState.instance.IsInGame) return;

        IsEnabled = !IsEnabled;
        Time.timeScale = IsEnabled ? 0.2f : 1.0f;
        GameState.instance.SetState(IsEnabled ? GameStateType.TARGET_ACQUISITION : GameStateType.IN_GAME);

        if (IsEnabled)
        {
            if (CurrentEnemy == null)
            {
                SelectClosestEnemy();
            }
        }
    }

    public void SelectLastEnemy()
    {
        if (enemies.Count != 0)
        {
            SelectEnemy(currentIndex - 1);
            return;
        }
    }

    public void SelectNextEnemy()
    {
        if (enemies.Count != 0)
        {
            SelectEnemy(currentIndex + 1);
            return;
        }
    }

    public void SelectClosestEnemy()
    {
        SelectEnemy(FindClosestEnemy());
    }

    public void UnselectCurrentEnemy(bool autoSelect = false)
    {
        if (CurrentEnemy != null)
        {
            SelectEnemy(null);
            if (autoSelect) SelectClosestEnemy();
        }
    }

    public void SelectEnemy(Enemy enemy)
    {
        SetCurrentEnemy(enemy);
    }

    private void SelectEnemy(int nextIndex)
    {
        if (InBounds(nextIndex, enemies))
        {
            SetCurrentEnemy(enemies[nextIndex]);
        }
    }

    private Enemy FindClosestEnemy()
    {
        Enemy enemy = null;
        UpdateEnemies();
        if (enemies != null && enemies.Count != 0)
        {
            Vector3 playerPositon = Player.instance.gameObject.transform.position;
            for (int i = 0; i < enemies.Count; i++)
            {
                float distance = Vector3.Distance(enemies[i].transform.position, playerPositon);
                if (distance < minDistance)
                {
                    enemy = enemies[i];
                }
            }
        }
        return enemy;
    }

    private void SetCurrentEnemy(Enemy enemy)
    {
        if (CurrentEnemy != null)
        {
            CurrentEnemy.SetCrosshairActive(false);
        }

        currentIndex = enemy == null ? -1 : enemies.IndexOf(enemy);
        CurrentEnemy = enemy;

        if (CurrentEnemy != null)
        {
            CurrentEnemy.SetCrosshairActive(true);
        }
    }

    private void UpdateEnemies()
    {
        enemies.Clear();
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            if (enemy.stats.IsDead) continue;
            enemies.Add(enemy);
        }
    }

    private bool InBounds(int index, List<Enemy> enemies)
    {
        return (index >= 0) && (index < enemies.Count);
    }
}
