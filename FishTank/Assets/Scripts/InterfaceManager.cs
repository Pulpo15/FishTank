using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour {
    [Header("Inventory")]
    public GameObject inventory;
    public Transform buttonsZone;
    public GameObject maintenance;

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
            if(inventory.activeSelf) {
                inventory.SetActive(false);
                Time.timeScale = 1f;
            } else if(!inventory.activeSelf) {
                inventory.SetActive(true);

                Time.timeScale = 0f;

                for(int i = 0; i < FishInventory.instance.fishList.Count; i++) {

                    Sprite sprite = Resources.Load<Sprite>("FishPng/" + FishInventory.instance.fishList[i].type);
                    buttonsList[i].image.sprite = sprite;
                    buttonsList[i].gameObject.SetActive(true);
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            ResetUse();
        }
    }
}
