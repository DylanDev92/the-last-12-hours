using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Player player => Player.Instance;

    [SerializeField]
    public Sprite fullHealthSprite;
    
    [SerializeField]
    public Sprite halfHealthSprite;
    
    [SerializeField] 
    public Sprite noHealthSprite;

    // Start is called before the first frame update
    void Start()
    {
        player.OnHealthChange += UpdateHealth;
        UpdateHealth();
    }

    void OnDestroy()
    {
        player.OnHealthChange -= UpdateHealth;
    }

    void UpdateHealth()
    {
        var hearts = GameObject
            .FindGameObjectsWithTag("HP")
            .OrderBy(obj => int.Parse(obj.name))
            .Select(obj => obj.GetComponent<Image>())
            .ToArray();

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null)
                continue;

            int health = (i + 1) * 2;
            if (player.health >= health)
                hearts[i].sprite = fullHealthSprite;
            else if (player.health >= health - 1)
                hearts[i].sprite = halfHealthSprite;
            else
                hearts[i].sprite = noHealthSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
