using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance;

    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject settingsMenu;
    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject inventoryMenu;

    [Serializable]
    private class InventoryUI
    {
        [SerializeField]
        private GameObject Inventory;
        [SerializeField]
        private GameObject Equipment;

        public Image[] InventorySlots { get { return Inventory.GetComponentsInChildren<Image>(); } }
        public Image[] EquipmentSlots { get { return Equipment.GetComponentsInChildren<Image>(); } }
    }
    [SerializeField]
    private InventoryUI inventoryUI;

    [SerializeField]
    private GameObject heartsUI;
    [SerializeField]
    private GameObject gameOverMenu;

    // Singleton for global acess and easier management
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    // Shows the hearts depending on the boolean
    public void ShowHeartUI(bool active)
    {
        heartsUI.SetActive(active);
    }

    // Shows the main menu depending on the boolean
    public void ShowMainMenu(bool active)
    {
        mainMenu.SetActive(active);
    }

    // Shows the settings menu depending on the boolean
    public void ShowPauseMenu()
    {
        bool isActive = !pauseMenu.activeSelf;
        GameManager.Instance.PauseGame(isActive);

        pauseMenu.SetActive(isActive);
    }


    // Shows the settings menu depending on the boolean
    public void ShowSettingsMenu()
    {
        bool isActive = !settingsMenu.activeSelf;
        settingsMenu.SetActive(isActive);
    }

    // Switches show or not inventory
    public void ShowInventory()
    {
        UpdateItemsUI();

        bool isActive = !inventoryMenu.activeSelf;
        inventoryMenu.SetActive(isActive);
    }

    // Method for leading to the next scene, handled by the GameManager.
    public void NextScene()
    {
        GameManager.NextScene();
    }

    // Updates the inventory items
    private void UpdateItemsUI()
    {
        for (int i = 0; i < Player.Instance.Inventory.Items.Count; i++)
        {
            if (Player.Instance.Inventory.Items[i] is Item item && item != null)
            {
                inventoryUI.InventorySlots[i].sprite = item.GetComponent<SpriteRenderer>().sprite;
            }
        }
    }
}