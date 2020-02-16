﻿using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

using Shop;
using Wave;

public class Barkeeper : MonoBehaviour
{
    #region Singelton

    public static Barkeeper instance;
    private PlayerInputActions inputActions;
    
    void Awake()
    {
        instance = this;

        inputActions = new PlayerInputActions();
        inputActions.UI.Select.performed += Select;
        inputActions.UI.Close.performed += Close;
    }

    #endregion;

    public ItemShop shop;
    public Animator animatior;
    private WaveSpawner waveSpawner;

    public bool isPlayerInReach = false;

    void Start()
    {
        waveSpawner = WaveSpawner.instance;
        waveSpawner.OnWaveStateUpdate += OnWaveUpdate;
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") isPlayerInReach = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerInReach = false;
            CloseShop();
        }
    }

    public void OnWaveUpdate(WaveState state, int rounds) 
    {
        if (WaveSpawner.instance.IsWaveRunning)
        {
            if (shop.IsOpen) CloseShop();

            inputActions.Disable();
        } else
        {
            inputActions.Enable();
        }
    }

    public void Select(CallbackContext ctx)
    {
        if (isPlayerInReach && GameState.instance.state != GameStateType.GAME_PAUSED && GameState.instance.state != GameStateType.GAME_OVER)
        {
            OpenShop();
        }
    }

    public void Close(CallbackContext ctx)
    {
        CloseShop();
    }

    public void OpenShop()
    {
        if (shop.IsOpen) return;
        GameState.instance.SetState(GameStateType.IN_SHOP);
        shop.IsOpen = true;
    }

    public void CloseShop()
    {
        if (!shop.IsOpen) return;

        shop.IsOpen = false;
        GameState.instance.SetState(GameStateType.IN_GAME);
    }
}
