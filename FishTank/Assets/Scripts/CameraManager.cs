using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    #region Macros
    const string TAG = "CamPosition";
    #endregion

    public GameObject fishTank;
    public float speedH = 2.0f;

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
        // *** Switch cameras *** //
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            GameEvents.instance.CamKeyPressed(0);
        } else if(Input.GetKeyDown(KeyCode.Alpha2)) {
            GameEvents.instance.CamKeyPressed(1);
        } else if(Input.GetKeyDown(KeyCode.Alpha3)) {
            GameEvents.instance.CamKeyPressed(2);
        }

        // *** Rotate camera using right click *** //
        if(Input.GetMouseButton(1)) {
            // *** Assign position to mouse axis *** //
            float posY = Input.GetAxis("Mouse X");
            float posZ = Input.GetAxis("Mouse Y");

            // *** Lock mouse *** //
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // *** Rotate *** //
            transform.RotateAround(fishTank.transform.position, new Vector3(0.0f, posY, posZ * -1), 80 * Time.deltaTime * speedH);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0.0f);

        }
        // *** Move camera using left click *** //
        else if(Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt)) { 
            // *** Assign position to mouse axis *** //
            float posZ = Input.GetAxis("Mouse X") * speedH * Time.deltaTime * 80f * -1;
            float posY = Input.GetAxis("Mouse Y") * speedH * Time.deltaTime * 80f * -1;

            transform.position = new Vector3(transform.position.x, transform.position.y + posY, transform.position.z + +posZ);

        } else {
            // *** Unlock mouse *** //
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } 

        // *** Zoom OUT/IN *** //
        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0) {
            transform.position += transform.forward * Time.deltaTime * speedH * 100;
        } else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0) {
            transform.position -= transform.forward * Time.deltaTime * speedH * 100;
        }
    }
}
