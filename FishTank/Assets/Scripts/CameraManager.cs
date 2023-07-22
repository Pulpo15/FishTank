using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    #region Macros
    const string TAG = "CamPosition";
    #endregion

    #region Public
    public GameObject fishTank;
    public float speedH;
    public bool invertV;
    public bool invertH;
    #endregion

    #region Private
    private List<Transform> camPosition;
    private float rotationY;
    private float rotationX;
    private Vector3 currentRotation;
    private Vector3 smoothVelocity = Vector3.zero;
    private float smoothTime = 0.1f;
    private float distanceFromTarget = 15.0f;
    #endregion 

    // *** Move camera to delimited points *** //
    private IEnumerator MoveCamera(int id) {
        if(id < camPosition.Count) { // Check if id is valid
                                     // *** Using coroutine move camera every frame *** //
            while(transform.position != camPosition[id].position) {
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
        if(Input.GetMouseButton(1)) {
            
        } else {
            StartCoroutine(MoveCamera(id));
        }
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

        rotationY = -98f;
        currentRotation = new Vector3(0, rotationY, 0);
    }

    private void Update() {
        // *** Rotate camera using right click *** //
        if(Input.GetMouseButton(2) && fishTank != null) {
            // *** Unlock mouse *** //
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            //// *** Assign position to mouse axis *** //
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * speedH * 100;
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * speedH * 100;

            // Invert Axis
            if(invertV) mouseY *= -1;
            if(invertH) mouseX *= -1;

            rotationY += mouseX;
            rotationX += mouseY;

            rotationX = Mathf.Clamp(rotationX, -10f, 60f);

            Vector3 nextRotation = new Vector3(rotationX, rotationY);

            currentRotation = Vector3.SmoothDamp(currentRotation, nextRotation, ref smoothVelocity, smoothTime);
            transform.localEulerAngles = currentRotation;

            transform.position = fishTank.transform.position - transform.forward * distanceFromTarget;
        } else {
            // *** Unlock mouse *** //
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // *** Zoom OUT/IN *** //
        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0) {
            if(distanceFromTarget > 1f) { // Clamp Zoom
                transform.position += transform.forward * Time.deltaTime * speedH * 100;
                distanceFromTarget -= Time.deltaTime * speedH * 100;
            }
        } else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0) {
            if(distanceFromTarget < 40f) { // Clamp Zoom
                transform.position -= transform.forward * Time.deltaTime * speedH * 100;
                distanceFromTarget += Time.deltaTime * speedH * 100;
            }
        }
    }
}
