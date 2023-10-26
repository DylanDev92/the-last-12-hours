using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLevelInteractable : Interactable
{
    [field: SerializeField]
    public int nextLevel { get; private set; }

    void Start()
    {
        this.OnInteractingChange += () =>
        {
            if (this.isInteracting)
            {
                GameManager.LoadLevelScene(nextLevel);
            }
        };
    }
}
