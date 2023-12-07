using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Player : Entity
{
    // Static reference to the instance
    public static Player Instance { get; private set; }
    private static Action _afterInstanceCallback;

    public static void AfterInstance(Action action)
    {
        if (Instance != null)
            action();
        else
            _afterInstanceCallback += action;
    }

    [SerializeField]
    public PlayerControls Controls;

    [field: SerializeField]
    public float interactDistance { get; private set; } = 5;

    [SerializeField]
    private Sound[] sounds;

    public GameObject hand { get; private set; }
    public SpriteRenderer handSprite { get; private set; }

    public override int attack => 1;
    public ItemType activeWeapon { get; private set; } = ItemType.Undefined;
    public int level { get; private set; }
    public bool isImmuneAttack { get; set; }
    public Inventory inventory { get; private set; } = new Inventory();

    public bool flashlight
    {
        get => GetComponentInChildren<Light2D>()?.enabled ?? false;
        set
        {
            var light = GetComponentInChildren<Light2D>();
            if (light)
                light.enabled = value;
        }
    }

    public event Action OnChangeWeapon;

    public void ChangeWeapon(ItemType type)
    {
        activeWeapon = type;

        if (type == ItemType.Undefined)
        {
            handSprite.enabled = false;
        }
        else
        {            
            // disable flashlight if weapon isn't flashlight
            if (type != ItemType.Flashlight)
                flashlight = false;

            handSprite.sprite = Item.GetSprite(type);
            handSprite.enabled = true;
        }

        OnChangeWeapon?.Invoke();
    }

    public void ResetInfo(bool fullReset = true)
    {
        health = maxHealth;
        if (fullReset)
        {
            isImmuneAttack = true;
            ChangeWeapon(ItemType.Undefined);
            inventory.Clear();
        }
    }

    protected override void Awake()
    {
        Debug.Log("Player Awake()");

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        hand = transform.Find("Hand").gameObject;
        handSprite = player.hand.GetComponentInChildren<SpriteRenderer>();

        Debug.Log(handSprite.name);

        OnStartMoving += () => PlaySound("Walking");
        OnStopMoving += () => StopSound("Walking");
        OnDeath += () => GameManager.LoadGameOverScene();

        ResetInfo();

        Sound.InitializeSounds(gameObject, sounds);

        // This is for not destroying the player when the scene is changed
        DontDestroyOnLoad(this);

        _afterInstanceCallback?.Invoke();
        _afterInstanceCallback = null;

        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        // Updates for handling input operations.
        UpdateInput();
        // Updates the hand position and handles different things about the hand
        UpdateHand();
    }


    public void UpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            InventoryUI.Enabled = !InventoryUI.Enabled;
        }

        if (Input.GetKeyUp(Controls.Pause))
        {
            GameManager.IsPaused = !GameManager.IsPaused;
        }

        // Interact with objects
        if (Input.GetKeyDown(Controls.Interact))
        {
            // Gets the list of interactables and then gets the first one if it's not null.
            var interactable = GetNearby<Interactable>(interactDistance).FirstOrDefault();
            interactable?.Interact();
        }

        // Turn on/off the flashlight
        if (Input.GetKeyDown(Controls.Flashlight))
        {
            if (this.activeWeapon != ItemType.Undefined)
            {
                var weapon = this.inventory.Get(this.activeWeapon);
                weapon?.UseWeaponAttack();
            }
        }
    }

    void FixedUpdate()
    {
        // Using FixedUpdate to get a Physics acurate movement.
        var movement = Controls.GetMovement();
        this.ApplyMovement(movement);
    }

    private void UpdateHand()
    {
        // Updates the hand rotation/position
        if (GameManager.Instance.MouseProperties is MouseProperties mouseProperties)
        {
            hand.transform.rotation = Quaternion.AngleAxis(mouseProperties.Angle, Vector3.forward);

            handSprite.flipY = !(mouseProperties.Angle <= 90 && mouseProperties.Angle >= -90);
        }
    }

    public void EnterLevel(int level)
    {
        this.level = level;

        // move to spawn point of level
        var spawn = GameObject.Find("Spawnpoint");
        if (spawn != null)
            transform.position = spawn.transform.position;

        this.isImmuneAttack = false;
    }

    public override void ReceiveAttack(Entity source, int damage)
    {
        base.ReceiveAttack(source, damage);
        Debug.Log($"player received damage, hp: {health} / {maxHealth}");
    }

    // Plays the sound based on the name, if doesn't match won't be played
    private void PlaySound(string soundName)
    {
        var sound = sounds.FirstOrDefault(x => x.Name == soundName);
        if (sound?.Source?.isPlaying == true)
            sound.Source.Play();
    }

    // Stops the sound based on the name, if doesn't match won't be stopped
    private void StopSound(string soundName)
    {
        var sound = sounds.FirstOrDefault(x => x.Name == soundName);
        if (sound?.Source?.isPlaying == false)
            sound.Source.Stop();
    }
}

[Serializable]
public class PlayerControls
{
    // Movement
    public KeyCode Up = KeyCode.W;
    public KeyCode Down = KeyCode.S;
    public KeyCode Right = KeyCode.D;
    public KeyCode Left = KeyCode.A;

    // Equipment
    public KeyCode HotbarFirst = KeyCode.Alpha1;
    public KeyCode HotbarSecond = KeyCode.Alpha2;
    public KeyCode HotbarThird = KeyCode.Alpha3;
    public KeyCode Inventory = KeyCode.Tab;
    public KeyCode Pause = KeyCode.Escape;

    // Flashlight
    public KeyCode Flashlight = KeyCode.F;

    // Use
    public KeyCode Reload = KeyCode.R;
    public KeyCode Heal = KeyCode.Q;
    public KeyCode Interact = KeyCode.E;

    public Vector2 GetMouseWorldPosition() => GameManager.Instance.camera.ScreenToWorldPoint(Input.mousePosition);

    public Vector2 GetMovement()
    {
        float vertical;
        float horizontal;

        // Vertical Movement
        if (Input.GetKey(Up))
        {
            vertical = 1f;
        }
        else if (Input.GetKey(Down))
        {
            vertical = -1f;
        }
        else
        {
            vertical = 0f;
        }

        // Horizontal Movement
        if (Input.GetKey(Right))
        {
            horizontal = 1f;
        }
        else if (Input.GetKey(Left))
        {
            horizontal = -1f;
        }
        else
        {
            horizontal = 0f;
        }

        return new Vector2(horizontal, vertical);
    }
}