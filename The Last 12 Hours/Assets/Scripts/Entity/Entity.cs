using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.IO;

[System.Serializable]
public abstract class Entity : MonoBehaviour
{
    public Player player => Player.Instance;
    public GameManager manager => GameManager.Instance;

    protected Rigidbody2D rb { get; private set; }
    protected SpriteRenderer sr { get; private set; }
    public Animator ani { get; private set; }

    // A*Pathfinding
    private AIPath aiPath { get; set; }
    public AIDestinationSetter destinationSetter { get; private set; }
    private bool isUsingPathfinding { get; set; }
    public GameObject AIFollow;


    [field: SerializeField]
    public int maxHealth { get; protected set; }
    private int _health;
    public int health
    {
        get => _health;
        set
        {
            _health = Math.Min(maxHealth, value);
            OnHealthChange?.Invoke();
        } 
    }

    public abstract int attack { get; }
    [field: SerializeField]
    public float visionDistance { get; protected set; } //is this pixels? 
    [field: SerializeField]
    public float speed { get; protected set; }
    [field: SerializeField]
    public bool canMove { get; set; }
    public bool isMoving { get; private set; }
    public Vector2 position
    {
        get => this.transform.position;
        set => this.transform.position = value;
    }

    public event Action OnStartMoving;
    public event Action OnStopMoving;
    public event Action OnDeath;
    public event Action<Entity, int> OnAttacked;
    public event Action<Entity> OnAttack;
    public event Action OnHealthChange;

    protected void RaiseAttack(Entity e) => OnAttack?.Invoke(e);

    public IEnumerable<T> GetNearby<T>(float distance) where T : MonoBehaviour =>
            Physics2D.OverlapCircleAll(transform.position, distance)
            .Select(x => x.GetComponent<T>())
            .Where(x => x != null)
            //.Select(x => new { obj = x, distance = Vector2.Distance(position, x.transform.position) })
            //.Where(x => x.distance <= distance)
            //.OrderBy(x => x.distance)
            //.Select(x => x.obj);
            .OrderBy(x => Vector2.Distance(position, x.transform.position));

    public virtual void ReceiveAttack(Entity source, int damage)
    {
        // is dead
        if (health <= 0) 
            return;

        health = Math.Max(0, health - damage);
        OnAttacked?.Invoke(source, damage);

        if (health <= 0)
            OnDeath?.Invoke();
    }

    protected virtual void Start()
    {
        // Gets the required A*Pathfinding components, if null use linear movement.
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        isUsingPathfinding = (aiPath && destinationSetter);
        if (isUsingPathfinding)
        {
            destinationSetter.target = player.transform;
            aiPath.maxSpeed = speed;
        }
    }

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // not all entities are animated
        Animator ani;
        if (TryGetComponent<Animator>(out ani))
            this.ani = ani;
    }

    protected void StopMovement() => ApplyMovement(Vector2.zero);

    protected void ApplyMovement(Vector2? movement = null)
    {
        if (!isUsingPathfinding && movement == null)
            throw new ArgumentNullException(nameof(movement), "movement vector cannot be null if pathfinding is disabled");

        if (!canMove)
        {
            if (isMoving)
            {
                if (isUsingPathfinding)
                    aiPath.canMove = false;
                else
                    rb.velocity = Vector2.zero;

                ani?.SetBool("isWalk", false);
                isMoving = false;
                OnStopMoving?.Invoke();
            }
            return;
        }

        Vector2 movement2;

        if (isUsingPathfinding)
        {
            // stop moving
            if (movement == Vector2.zero)
            {
                aiPath.canMove = false;
                movement2 = Vector2.zero;
            }
            else
            {
                aiPath.canMove = true;
                movement2 = aiPath.velocity;
            }
        }
        else
        {
            movement2 = (Vector2)movement;
            rb.velocity = movement2 * speed;
        }


        ani?.SetBool("isWalk", movement2.magnitude > 0);

        if (movement2.x > 0)
            sr.flipX = false;
        else if (movement2.x < 0)
            sr.flipX = true;

        if (!isMoving && movement2.magnitude > 0)
        {
            isMoving = true;
            OnStartMoving?.Invoke();
        }
        else if (isMoving && movement2.magnitude <= 0)
        {
            isMoving = false;
            OnStopMoving?.Invoke();
        }
    }
}
