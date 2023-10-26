using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoolInteraction : Interactable
{
    [SerializeField]
    private GameObject firstState;

    [SerializeField]
    private GameObject secondState;

    void Start()
    {
        this.OnInteractingChange += BoolInteraction_OnInteractingChange;
    }

    private void BoolInteraction_OnInteractingChange()
    {
        var collider = this.GetComponent<BoxCollider2D>();
        if (collider != null)
            collider.isTrigger = !collider.isTrigger;

        firstState.SetActive(!this.isInteracting);
        secondState.SetActive(this.isInteracting);
    }

}
