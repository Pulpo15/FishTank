using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    #region Macros
    const string TAG = "CamPosition";
    #endregion

    public GameObject fishTank;

    private List<Transform> camPosition;

    // *** Move camera to delimited points *** //
    private IEnumerator MoveCamera(int id) {
        if(id < camPosition.Count) { // Check if id is valid
            // *** Using coroutine move camera every frame *** //
            while (transform.position != camPosition[id].position) {
                transform.position = Vector3.MoveTowards(transform.position, camPosition[id].position,
                    50 * Time.deltaTime);

                // *** Check distance to avoid infinite loop *** //
                float dist = Vector3.Distance(transform.position, camPosition[id].position);
                if(dist < 1f) transform.position = camPosition[id].position;

                transform.LookAt(fishTank.transform); // Aim camera to FishTank

                yield return 0; // Skip to next frame
            }
        } else {
            Debug.LogError("ID is higher than camera position count");
        }
        GameEvents.instance.moving = false;
    }

    private void ExecuteMoveCamera(int id) {
        StartCoroutine(MoveCamera(id));
    }

    private void Start() {
        // *** Subscribe event *** //
        GameEvents.instance.onCamKeyPressed += ExecuteMoveCamera;

        // *** Get all Camera positions from the fishtank *** //
        camPosition = new List<Transform>();

        foreach(Transform item in fishTank.transform) {
            if(item.tag == TAG) {
                for(int i = 0; i < item.childCount; i++) { 
                    camPosition.Add(item.GetChild(i));
                }
            }
        }

        // *** Set camera position to default *** //
        transform.position = camPosition[0].position;
        transform.rotation = camPosition[0].rotation;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            GameEvents.instance.CamKeyPressed(0);
        } else if(Input.GetKeyDown(KeyCode.Alpha2)) {
            GameEvents.instance.CamKeyPressed(1);
        } else if(Input.GetKeyDown(KeyCode.Alpha3)) {
            GameEvents.instance.CamKeyPressed(2);
        }
    }
}
