using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class InventoryItemSlot : MonoBehaviour, IPointerDownHandler
{
    private Outline _outline;

    public Player player => Player.Instance;
    public ItemType itemType { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        _outline = this.GetComponent<Outline>();
        player.OnChangeWeapon += player_OnChangeWeapon;

        // forcefully trigger so outlining is done.
        //player_OnChangeWepaon();
    }

    void OnDestroy()
    {
        player.OnChangeWeapon -= player_OnChangeWeapon;
    }

    private void player_OnChangeWeapon()
    {
        if (this.itemType == ItemType.Undefined)
            return;

        // when weapon is selected, highlight it
        if (this.itemType == player.activeWeapon)
        {
            if (_outline != null)
                _outline.enabled = true;
        }
        else
        {
            if (_outline != null)
                _outline.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.itemType == ItemType.Undefined)
            return;

        if(player.inventory.ContainsType(this.itemType))
        {
            var item = player.inventory.Get(this.itemType);
            if (item.Use() == true)
            {
                player.inventory.Remove(this.itemType, 1);
            } 
        }
    }
}
