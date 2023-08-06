using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour {

    public GameObject canvas;
    [Header("Canvas Zones")]
    public GameObject storeInventory;
    public GameObject typeSelector;
    public GameObject inventory;
    public GameObject foodInventory;
    public GameObject maintenance;
    [Header("Inner Objects")]
    public Transform buttonsZone; // Object to assign fish button images for inventory
    public Transform storeButtonsZone; // Object to assign fish buttons image for store
    public Transform typeButtonsZone; // Object to assign buttons image for type store

    static public InterfaceManager instance;

    private List<Button> buttonsList;
    private List<Button> storeButtonList;
    private List<Button> typeButtonList;

    #region Maintenance
    public void UseFood() {
        if(!FeedManager.instance.useFood) {
            FeedManager.instance.useFood = true;
            FishTankSelector.fishTankManager.useRemover = false;
            FishTankSelector.fishTankManager.GetComponent<Collider>().enabled = true;
        }
        else if(FeedManager.instance.useFood) {
            FeedManager.instance.useFood = false;
        }
    }

    public void UseRemover() {
        if(!FishTankSelector.fishTankManager.useRemover) {
            FishTankSelector.fishTankManager.useRemover = true;
            FeedManager.instance.useFood = false;
            FishTankSelector.fishTankManager.GetComponent<Collider>().enabled = false;
        }
        else if(FishTankSelector.fishTankManager.useRemover) {
            FishTankSelector.fishTankManager.useRemover = false;
            FishTankSelector.fishTankManager.GetComponent<Collider>().enabled = true;
        }
    }
    #endregion

    #region Store
    public void ActiveTypeSelector() {
        typeSelector.SetActive(true);

        List<BreedManager.FishType> fishTypes = new List<BreedManager.FishType>();

        for(int i = 0; i < FishInventory.instance.fishList.Count; i++) {
            if(!fishTypes.Contains(FishInventory.instance.fishList[i].type)) {
                fishTypes.Add(FishInventory.instance.fishList[i].type);
            }
        }

        for(int i = 0; i < fishTypes.Count; i++) {
            Sprite sprite = Resources.Load<Sprite>("FishPng/" + fishTypes[i]);
            typeButtonList[i].image.sprite = sprite;
            typeButtonList[i].gameObject.SetActive(true);
        }

    } 

    private void SetFishStore(Image image) {
        typeSelector.SetActive(false);

        for(int i = 0; i < storeButtonList.Count; i++) {
            if(!storeButtonList[i].gameObject.activeSelf) {
                Sprite sprite = Resources.Load<Sprite>("FishPng/" + image.sprite.name);
                storeButtonList[i-1].image.sprite = sprite;
                storeButtonList[i].gameObject.SetActive(true);
                break;
            }
        }
    }

    #endregion

    #region ActiveZones
    public void StoreInventory() {
        storeInventory.SetActive(true);
        inventory.SetActive(false);
        foodInventory.SetActive(false);

        storeButtonList[0].gameObject.SetActive(true);
    }
    public void Inventory() {
        storeInventory.SetActive(false);
        inventory.SetActive(true);
        foodInventory.SetActive(false);

        for(int i = 0; i < FishInventory.instance.fishList.Count; i++) {

            Sprite sprite = Resources.Load<Sprite>("FishPng/" + FishInventory.instance.fishList[i].type);
            buttonsList[i].image.sprite = sprite;
            buttonsList[i].gameObject.SetActive(true);
        }
    }
    public void FoodInventory() {
        storeInventory.SetActive(false);
        inventory.SetActive(false);
        foodInventory.SetActive(true);
    }
    #endregion

    public void ResetUse() {
        if (FishTankSelector.fishTankManager != null) {
            FishTankSelector.fishTankManager.useRemover = false;
            FeedManager.instance.useFood = false;
        }
    }

    private void SetFishTankCanvas() {
        maintenance.SetActive(true);
    }

    private void RemoveFishTankCanvas() {
        maintenance.SetActive(false);
    }

    private void Awake() {
        instance = this;
    }

    private void Start() {
        GameEvents.instance.onFishTankUpdated += SetFishTankCanvas;
        GameEvents.instance.onFishTankRemoved += RemoveFishTankCanvas;
        GameEvents.instance.onFishButtonClicked += SetFishStore;

        // *** Add Inventory buttons to list to change sprite *** //
        buttonsList = new List<Button>();

        for(int i = 0; i < buttonsZone.childCount; i++) {
            Transform horizontalZone = buttonsZone.GetChild(i).transform;

            for(int j = 0; j < horizontalZone.childCount; j++) {
                Button button = horizontalZone.GetChild(j).GetComponent<Button>();
                button.GetComponentInChildren<TextMeshProUGUI>().text = "";
                buttonsList.Add(button);
                button.gameObject.SetActive(false);
            }
        }

        // *** Add store buttons to list to change sprite *** //
        storeButtonList = new List<Button>();

        for(int j = 0; j < storeButtonsZone.childCount; j++) {
            Button button = storeButtonsZone.GetChild(j).GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = "";
            storeButtonList.Add(button);
            button.gameObject.SetActive(false);
        }

        for(int i = 0; i < storeButtonsZone.childCount; i++) {
            Transform horizontalZone = storeButtonsZone.GetChild(i).transform;

        }

        // *** Add type selector buttons to list to change sprite *** //
        typeButtonList = new List<Button>();

        for(int i = 0; i < typeButtonsZone.childCount; i++) {
            Transform horizontalZone = typeButtonsZone.GetChild(i).transform;

            for(int j = 0; j < horizontalZone.childCount; j++) {
                Button button = horizontalZone.GetChild(j).GetComponent<Button>();
                button.GetComponentInChildren<TextMeshProUGUI>().text = "";
                typeButtonList.Add(button);
                button.gameObject.SetActive(false);
            }
        }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Tab)) {
            if(canvas.activeSelf) {
                canvas.SetActive(false);
                Time.timeScale = 1f;
            } else if(!canvas.activeSelf) {
                canvas.SetActive(true);
                Time.timeScale = 0f;
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            ResetUse();
        }
    }
}
