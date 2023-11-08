using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeLevelInteractable : Interactable
{
    [field: SerializeField]
    public int nextLevel { get; private set; }

    [field: SerializeField]
    public ItemType requiredItem { get; private set; }

    void Start()
    {
        //
        this.OnInteractingChange += () =>
        {
            if (requiredItem != ItemType.Undefined)
            {
                //item not in inv = not ok
                if (!player.inventory.ContainsType(requiredItem))
                    return;
            }
            //reqitem null = ok
            //item in inv = ok 

            GameManager.LoadLevelScene(nextLevel);
        };
    }
}
