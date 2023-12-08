using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public GameManager manager => GameManager.Instance;
    public Player player => Player.Instance;

    private bool _isOutlined = false;

    protected SpriteRenderer[] spriteRenderers { get; private set; }

    public bool isInteracting { get; protected set; }

    [field: SerializeField]
    public bool showOutline { get; set; } = true;

    [field: SerializeField]
    public AudioSource interactSound { get; private set; }

    public event Action OnInteractingChange;

    public virtual void Interact()
    {
        isInteracting = !isInteracting;
        if (interactSound) interactSound.Play();
        OnInteractingChange?.Invoke();
    }

    // Gets all the components type SpriteRenderer to the array
    protected virtual void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        if (showOutline)
        {
            // Constantly check if a player is near the interactable.
            bool isPlayerNear = Vector2.Distance(this.transform.position, player.position) <= player.interactDistance;
            //    Physics2D
            //    .OverlapCircleAll(player.position, player.interactDistance)
            //    .Any(c => c.gameObject == this.gameObject);

            //
            // we don't want to spam changing the renderer's material
            // so we only trigger a change here if required
            //
            if (_isOutlined && !isPlayerNear)
            {
                //Debug.Log("Disabling outline");
                SetOutline(manager.OutlineInteract.NoOutline);
                _isOutlined = false;
            }
            else if (!_isOutlined && isPlayerNear)
            {
                //Debug.Log("Enabling outline");
                SetOutline(manager.OutlineInteract.Outline);
                _isOutlined = true;
            }
        }
    }
    // This sets the outline of interactable.
    protected void SetOutline(Material outlineMaterial)
    {
        foreach (var renderer in spriteRenderers)
            renderer.material = outlineMaterial;
    }
}
