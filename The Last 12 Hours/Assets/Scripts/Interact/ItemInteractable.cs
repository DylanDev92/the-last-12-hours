using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractable : Interactable
{
    public static event Action OnPickUp;

    // Start is called before the first frame update
    void Start()
    {
        OnInteractingChange += ItemInteractable_OnInteractingChange;
    }

    private void ItemInteractable_OnInteractingChange()
    {
        if (isInteracting)
        {
            var item = this.GetComponent<Item>();
            if (item == null)
            {
                Debug.LogWarning("ItemInteractable attached to game object, but object does not have Item component");
                return;
            }

            item.Pickup();
            OnPickUp?.Invoke();
        }
    }

    protected override void Update()
    {
        base.Update();
    }
}
