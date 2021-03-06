﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Items;
using Utils;
using System.Collections;

public enum EnemyState
{
    Attack,
    Strafe,
    Avoid,
    Seek
}

public class Enemy : Entity
{
    [Header("Visuals")]
    public GameObject model;
    public GameObject crosshair;
    public GameObject cap;

    [Header("Enemy Settings")]

    public float moveSpeed = 5.0f;
    public float turnSpeed = 40.0f;
    public float attackDistance = 1.0f;
    public float dangerDistance = 2.0f;
    public float trackSpeed = 0.1f;
    public float attackRate = 10.0f;
    public float attackRateFluctuation = 0.0f;

    public EnemyState State { get; protected set; }

    private Vector3 destination;
    private Vector3 moveVec;

    private float lastAttackTime = 0.0f;

    private bool disabled = false;
    private float lastThought = 0.0f;
    private float lastReact = 0.0f;
    private float actualAttackRate = 0.0f;

    private float thinkPeriod = 1.5f;
    private float reactPeriod = 0.4f;

    private Vector3 distance;
    private Vector3 avoidDistance = Vector3.zero;
    private float sqrDistance;
    private float sqrAttackDistance;
    private float sqrDangerDistance;
    private bool engagePlayer = false;
    private float strafeDir = 1.0f;
    private float strafeCooldown = 0f;
    private float strafeRate = 3.0f;

    private int[] moneyDrops;

    private Player player;
    private PlayerCombat playerCombat;
    private GameObject playerObject;
    private Avoider avoider;
    private NavMeshAgent agent;

    void OnEnable()
    {
        disabled = false;
    }

    void OnDisable()
    {
        disabled = true;
    }

    protected override void Start()
    {
        base.Start();
        player = Player.instance;
        playerCombat = player.combat as PlayerCombat;
        agent = GetComponent<NavMeshAgent>();
        State = EnemyState.Seek;
        cap.SetActive(Random.value < 0.5f ? true : false);

        actualAttackRate = attackRate + (Random.value - 0.5f) * attackRateFluctuation;
        lastAttackTime = -actualAttackRate;

        avoider = gameObject.GetComponentInChildren<Avoider>();
        if (avoider != null)
        {
            if (avoider != null)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), avoider.GetComponent<Collider>());
            }
        }

        sqrAttackDistance = Mathf.Pow(attackDistance, 2);
        sqrDangerDistance = Mathf.Pow(dangerDistance, 2);

        lastThought += thinkPeriod * Random.value;
        lastReact += reactPeriod * Random.value;

        StartCoroutine(SummoningSickness());

    }

    void FixedUpdate()
    {
        if (stats.IsDead) return;

        if (engagePlayer && !combat.IsAttacking)
        {
            OnAttackComplete();
        }

        if (disabled)
        {
            if (player != null)
            {
                if (trackSpeed > 0.0f && combat.IsAttacking && !combat.IsStunned)
                {
                    TurnEnemyToPlayer();
                }
                lastReact = Time.fixedTime;
                UpdateDistance();
            }
            return;
        }

        if (strafeCooldown > 0.0f)
        {
            strafeCooldown -= Time.fixedDeltaTime;
        }

        if (player == null || (Time.fixedTime - lastThought) > thinkPeriod)
        {
            lastThought = Time.fixedTime;
            Think();
        }

        if (player == null) return;
        if ((Time.fixedTime - lastReact) > reactPeriod) React();

        UpdateDistance();

        bool shouldAvoid = (avoidDistance != Vector3.zero && sqrDistance <= sqrDangerDistance);
        bool shouldStrafe = (!shouldAvoid && !engagePlayer && sqrDistance <= sqrAttackDistance);
        bool shouldAttack = (engagePlayer && sqrDistance <= sqrAttackDistance);


        if (shouldAvoid)
        {
            State = EnemyState.Avoid;
            Avoid(avoidDistance);
        }
        else if (shouldAttack)
        {
            State = EnemyState.Attack;
            if (!combat.IsAttacking && !combat.IsStunned)
            {
                TurnEnemyToPlayer();
                if (equipment != null) equipment.UsePrimary();

                lastAttackTime = Time.fixedTime;
                actualAttackRate = attackRate + (Random.value - 0.5f) * attackRateFluctuation;
            }
        }
        else if (shouldStrafe)
        {
            State = EnemyState.Strafe;
            Strafe(player.transform.position);
        }
        else
        {
            State = EnemyState.Seek;
            Seek(distance);
        }
    }

    public void Init(EnemyConfig config)
    {
        (stats as EnemyStats).Init(config.stats);
        (combat as EnemyCombat).Init(config.combat);

        moneyDrops = config.moneyDrops;

        RandomItem[] items = config.items;
        if (items != null)
        {
            RandomItem randomItem = GetRandomItem(items);
            Equipment item = randomItem.item;

            float damageOverride = randomItem.damageOverride;
            float healthOverride = randomItem.healthOverride;

            equipment.EquipItem(item);
            animator.SetEquipment(item);

            if (damageOverride > 0) stats.damage = damageOverride;
            if (healthOverride > 0)
            {
                stats.maxHealth = healthOverride;
                stats.CurrentHealth = healthOverride;
            }
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();

        agent.enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        animator.OnDeath();

        Player.instance.combat.AddMana(10f);
        if (moneyDrops != null) Player.instance.AddMoney(moneyDrops[Random.Range(0, moneyDrops.Length)]);
        if (TargetAcquisition.instance.CurrentEnemy == this) TargetAcquisition.instance.UnselectCurrentEnemy(true);

        Statistics.instance.AddKill();
        Destroy(gameObject, 5f);
    }

    private void UpdateDistance()
    {
        distance = (destination - transform.position);
        sqrDistance = distance.sqrMagnitude;
        if (sqrDistance > sqrAttackDistance) OnAttackComplete();
    }

    private void Think()
    {
        playerObject = GetPlayer();
        if (playerObject == null) return;

        player = Player.instance;

        if (player != null && player.stats.IsDead) return;
        if (avoider != null && avoider.enemy != null)
        {
            avoidDistance = avoider.enemy.transform.position - transform.position;
            avoidDistance = Vector3.Slerp(distance.normalized, avoidDistance.normalized, 0.5f);
        }
        else
        {
            avoidDistance = Vector3.zero;
        }

        if (!engagePlayer && strafeCooldown <= 0f)
        {
            strafeCooldown = strafeRate;
            strafeDir = 1.0f;
            if (Random.value > 0.5f) strafeDir = -1.0f;
        }
    }

    private void React()
    {
        lastReact = Time.fixedTime;
        distance = (destination - transform.position);
        sqrDistance = distance.sqrMagnitude;

        if (sqrDistance != 0 && sqrDistance <= sqrAttackDistance)
        {
            if (!engagePlayer)
            {
                playerCombat.OnRequestAttack(gameObject);
            }
        }
    }

    public void OnAllowAttack(GameObject target)
    {
        if (player != null && target == player.gameObject) engagePlayer = true;
    }

    private void OnAttackComplete()
    {
        engagePlayer = false;
        if (player != null)
        {
            playerCombat.OnCancelAttack(gameObject);
        }
    }

    private void Avoid(Vector3 distance)
    {
        Move(distance * -100);
    }

    private void Strafe(Vector3 playerPosition)
    {
        if (engagePlayer)
        {
            OnAttackComplete();
        }

        Vector3 offset = transform.position - playerPosition;
        Vector3 direction = Vector3.Cross(offset, Vector3.up);
        agent.SetDestination(transform.position + direction);
    }

    private void Seek(Vector3 distance)
    {
        Move(distance);
    }

    private void Move(Vector3 distance)
    {
        if (engagePlayer)
        {
            OnAttackComplete();
        }

        destination = player.transform.position;
        moveVec = distance.normalized;
        moveVec.y = 0.0f;

        agent.SetDestination(destination);
    }

    private void TurnEnemyToPlayer()
    {
        Vector3 lookDirection = (player.gameObject.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(lookDirection.x, 0, lookDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 1000f);
    }

    private RandomItem GetRandomItem(RandomItem[] items)
    {
        Dictionary<RandomItem, int> weights = new Dictionary<RandomItem, int>();
        foreach (RandomItem item in items) weights.Add(item, item.percentage);

        return WeightedRandomizer.From(weights).TakeOne();
    }

    public void SetCrosshairActive(bool active)
    {
        crosshair.SetActive(active);
    }

    private GameObject GetPlayer()
    {
        return Player.instance.gameObject;
    }

    private IEnumerator SummoningSickness()
    {
        this.OnDisable();
        yield return new WaitForSeconds(1.0f);
        this.OnEnable();
    }

    private float AttackCooldown
    {
        get
        {
            return Mathf.Max(actualAttackRate - (Time.fixedTime - lastAttackTime), 0f);
        }
    }

    private void OnDrawGizmos()
    {
        if (avoidDistance != Vector3.zero && sqrDistance <= sqrDangerDistance)
        {
            var radius = avoider.GetComponent<SphereCollider>().radius;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}