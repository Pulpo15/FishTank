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
    public GameObject inventory;
    public GameObject foodInventory;
    public GameObject maintenance;
    [Header("Inner Objects")]
    public Transform buttonsZone; // Object to assign fish button images

    public InterfaceManager instance;

    private List<Button> buttonsList;

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



    #region ActiveZones
    public void StoreInventory() {
        storeInventory.SetActive(true);
        inventory.SetActive(false);
        foodInventory.SetActive(false);
    }
    public void Inventory() {
        storeInventory.SetActive(false);
        inventory.SetActive(true);
        foodInventory.SetActive(false);
        Debug.Log(FishInventory.instance.fishList.Count);
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
