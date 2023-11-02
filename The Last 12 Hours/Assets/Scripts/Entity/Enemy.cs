using System;
using System.Collections;
using System.IO;
using UnityEngine;


public class Enemy : Entity
{
    private float _nextAttackTime;
    private Vector2 _spawnLocation;


    [field: SerializeField]
    public float attackSpeed { get; protected set; }
    [field: SerializeField]
    public float attackRange { get; protected set; }
    [field: SerializeField]
    public float chaseRange { get; protected set; }

    public bool isChasing { get; protected set; }

    public override int attack => 1;

    public bool isPlayerInVision => Vector2.Distance(Player.Instance.position, this.rb.position) <= visionDistance;
    public bool isPlayerInChaseRange => Vector2.Distance(Player.Instance.position, this.rb.position) <= chaseRange;
    public bool isPlayerInAttackRange => Vector2.Distance(Player.Instance.position, this.rb.position) <= attackRange;
    public bool isAttackCooldown => _nextAttackTime > Time.time;

    protected override void Start()
    {
        health = maxHealth;
        _spawnLocation = this.position;

        this.OnDeath += Enemy_OnDeath;

        base.Start();
    }

    private void Enemy_OnDeath()
    {
        Debug.Log("Enemy is dead");
        this.ani?.SetTrigger("triggerDeath");
        DropReward();

        // 0.7 for rat

        manager.DelayCallback(TimeSpan.FromSeconds(1), () =>
        {
            // despawn the object
            this.gameObject.SetActive(false);

            manager.DelayCallback(TimeSpan.FromSeconds(10), () =>
            {
                //move obj
                this.position = _spawnLocation;
                
                //restore health
                this.health = maxHealth;

                //set active true
                this.gameObject.SetActive(true);

            });

            //Destroy(this.gameObject);
        });
    }

    protected virtual void DropReward()
    {

    }

    protected virtual void DropItem(ItemType type)
    {
        var prefab = Item.GetPrefab(type);
        if (prefab != null)
            Instantiate(prefab, this.transform.position, Quaternion.identity);
        else
            Debug.LogWarning($"Could not drop item {type} as no prefab exists!");
    }

    protected void UpdateAttackCooldown()
    {
        // add cd
        _nextAttackTime = Time.time + (float)attackSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (health <= 0)
        {
            isChasing = false;
            canMove = false;
            StopMovement();
        }

        if (isPlayerInVision && !isChasing)
            isChasing = true;

        if (isChasing)
        {
            if (isPlayerInAttackRange)
            {
                if (!isAttackCooldown)
                {
                    // if success update cd
                    if (Attack())
                    {
                        UpdateAttackCooldown();
                        StopMovement();
                    }
                }
                else
                {
                    // stop moving because close enuogh to player to attack
                    StopMovement();
                }
            }
            else if (isPlayerInChaseRange)
            {
                Vector2 movement = (player.position - rb.position).normalized;
                ApplyMovement(movement);
            }
            else // stop chasing
            {
                // chase range can be higher than vision range so that
                // once a monster has begun to chase the player it can chase further
                // than aggro vision
                // to avoid this behavior just set chase and vision to the same value
                isChasing = false;
                StopMovement();
            }
        }
    }

    public override void ReceiveAttack(Entity source, int damage)
    {
        Debug.Log("Enemy receive damage");
        base.ReceiveAttack(source, damage);
    }

    protected virtual bool Attack()
    {
        if (player.isImmuneAttack)
            return false;

        ani?.SetTrigger("triggerAttack");
        RaiseAttack(player);
        player.ReceiveAttack(this, this.attack);
        return true;
    }

    protected float GetAngle(Vector2 v1, Vector2 v2)
    {
        var angle = Mathf.Atan2(v1.y - v2.y, v1.x - v2.x);
        if (angle < 0)
            angle += 2 * Mathf.PI;
        angle *= Mathf.Rad2Deg;
        return angle;
    }

    protected bool IsInAngleRange(float f, float t, float w)
    {
        // https://stackoverflow.com/questions/71881043/how-to-check-angle-in-range

        f %= 360;
        t %= 360;
        w %= 360;

        while (f < 0) f += 360;
        while (t < f) t += 360;
        while (w < f) w += 360;


        return f <= w && w <= t;
    }
}