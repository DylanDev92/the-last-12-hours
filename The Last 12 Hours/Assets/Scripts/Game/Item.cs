using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum ItemType
{
    Undefined = 0,
    Ammo,
    Battery,
    Bandage, 
    Flashlight,
    Axe,
    Knife,
    Gun
}
public class Item : MonoBehaviour
{
    private static ItemType[] AUTO_CONSUME_TYPES = new ItemType[] { };
    private static ItemType[] CONSUMABLES_TYPES = new[] { ItemType.Battery, ItemType.Bandage };
    private static ItemType[] EQUIPMENT_TYPES = new[] { ItemType.Flashlight, ItemType.Axe, ItemType.Knife, ItemType.Gun };

    public static bool IsEquipment(ItemType type) => EQUIPMENT_TYPES.Contains(type);
    public static bool IsConsumable(ItemType type) => CONSUMABLES_TYPES.Contains(type);
    public static bool IsAutoConsume(ItemType type) => AUTO_CONSUME_TYPES.Contains(type);
    public static Sprite GetSprite(ItemType type)
    {
        var manager = GameManager.Instance;
        switch (type)
        {
            case ItemType.Axe: return manager.ItemSprites.Axe;
            case ItemType.Gun: return manager.ItemSprites.Gun;
            case ItemType.Knife: return manager.ItemSprites.Knife;
            case ItemType.Flashlight: return manager.ItemSprites.Flashlight;
            case ItemType.Bandage: return manager.ItemSprites.Bandage;
            case ItemType.Ammo: return manager.ItemSprites.Ammo;
        }
        return null;
    }
    public static GameObject GetPrefab(ItemType type)
    {
        var manager = GameManager.Instance;
        switch (type)
        {
            case ItemType.Bandage: return manager.Prefabs.BandagePrefab;
        }
        return null;
    }


    public Player player => Player.Instance;

    [field: SerializeField]
    public ItemType type { get; private set; }
    [field: SerializeField]
    public int amount { get; set; }
    public string itemName => type.ToString();
    public bool isEquipment => IsEquipment(type);
    public bool isConsumable => IsConsumable(type);
    public bool isAutoConsume => IsAutoConsume(type);
    public Sprite sprite => GetSprite(type);

    public void Start()
    {
        var sprite = this.sprite;
        if (sprite != null)
            GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void Pickup()
    {
        Debug.Log("Player is picking up " + itemName);

        if (isAutoConsume)
        {
            Debug.Log($"Auto consuming item ${type}");
            if (!this.Use())
                return; // do not want object destroyed it we couldn't use it
        }
        else
        {
            player.inventory.Add(this);
        }

        // remove from world
        Destroy(this.gameObject);
    }

    public void UseWeaponAttack()
    {
        switch (this.type)
        {
            case ItemType.Flashlight:
                {
                    // toggle the flashlight
                    player.flashlight = !player.flashlight;
                    break;
                }
            case ItemType.Axe:
            case ItemType.Knife:
                {
                    player.ani?.SetTrigger("triggerStab");

                    var enemy = player.GetNearby<Enemy>(2f).FirstOrDefault();
                    if (enemy != null)
                    {
                        Debug.Log($"Enemy found, dist={Vector2.Distance(player.position, enemy.position)}");
                        enemy.ReceiveAttack(player, player.attack);
                    }
                    else
                    {
                        Debug.Log("No enemy found to attack");
                    }
                    break;
                }
            case ItemType.Gun:
                {
                    if (player.inventory.ContainsType(ItemType.Ammo))
                    {
                        var ammo = player.inventory.Get(ItemType.Ammo);
                        if (ammo.amount > 0)
                        {
                            player.ani?.SetTrigger("triggerShoot");

                            var mousePositionWorld = (Vector2)GameManager.Instance.camera.ScreenToWorldPoint(Input.mousePosition);
                            var direction = (mousePositionWorld - player.position).normalized;
                            var hit = Physics2D
                                .RaycastAll(player.position, direction, 3f)
                                .Select(o => o.collider.GetComponent<Enemy>())
                                .Where(e => e != null)
                                .OrderBy(e => Vector2.Distance(player.position, e.position))
                                .ToList();

                            // maybe add piercing to the gun? idk
                            Debug.Log($"{hit.Count} potential enemies hit, but only hitting the first.");
                            var enemy = hit.FirstOrDefault();
                            enemy?.ReceiveAttack(player, player.attack);

                            player.inventory.Remove(ammo.type, 1);
                            Debug.Log($"Gun shot, remaining ammo: {ammo.amount}");
                        }
                    }
                    break;
                }
        }
    }

    public bool Use()
    {
        Debug.Log("Player is using item " + itemName);

        if (isEquipment)
        {
            player.ChangeWeapon(this.type);
            return false;
        }
        else
        {
            switch (type)
            {
                case ItemType.Bandage:
                    {
                        if (player.health == player.maxHealth)
                        {
                            Debug.Log("Ignored using bandage because health is full");
                            return false;
                        }

                        Debug.Log("Increasing playing hp");
                        player.health += 2;
                        break;
                    }
                default:
                    {
                        Debug.Log($"No item usage implemented for {type}");
                        break;
                    }
            }
            return true;
        }
    }
}
