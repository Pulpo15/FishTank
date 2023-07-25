using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    #region Macros
    const string TAG = "CamPosition";
    #endregion

    #region Public
    public float speedH;
    public bool invertV;
    public bool invertH;
    public Transform generalPosition;
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

#nullable enable

    // *** Move camera to delimited points *** //
    private IEnumerator MoveCamera(Transform target) {
        GameEvents.instance.moving = true;
        if (target != null) {
            while(transform.position != target.position) {
                // *** Move Camera *** //
                transform.position = Vector3.MoveTowards(transform.position, target.position,
                    50 * Time.deltaTime);

                // *** Rotate Camera *** //
                transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, 5f * Time.deltaTime);

                // *** Check distance to avoid infinite loop *** //
                float dist = Vector3.Distance(transform.position, target.position);
                if(dist < 0.3f) {
                    // *** Set final position & rotation *** //
                    transform.position = target.position;
                    transform.rotation = target.rotation;
                }

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
            StartCoroutine(MoveCamera(camPosition[id]));
        }
    }

    private void SetFishtankCamera() {
        // *** Get all Camera positions from the fishtank *** //
        camPosition = new List<Transform>();

        foreach(Transform item in FishTankSelector.fishTankManager.transform) {
            if(item.tag == TAG) {
                for(int i = 0; i < item.childCount; i++) {
                    camPosition.Add(item.GetChild(i));
                }
            }
        }

        StopAllCoroutines();

        // *** Set camera position to default *** //
        StartCoroutine(MoveCamera(camPosition[0]));

        rotationY = -98f;
        currentRotation = new Vector3(0, rotationY, 0);
    }
    private void SetGeneralCamera() {

        StopAllCoroutines();

        // *** Set camera position to default *** //
        StartCoroutine(MoveCamera(generalPosition));

        //transform.position = generalPosition.position;
        //transform.rotation = generalPosition.rotation;
    }

    private void MoveCameraArroundFishtank() {

        // *** FishTank not selected, reset position *** //
        if(FishTankSelector.fishTankManager == null) {
            if (rotationX != 0f || rotationY != 0f) {
                rotationY = 0f;
                rotationX = 0f;
            }
            return; 
        }

        // *** Rotate camera using middle click *** //
        if(Input.GetMouseButton(2) && FishTankSelector.fishTankManager != null && !GameEvents.instance.moving) {
            // *** Unlock mouse *** //
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // *** Assign position to mouse axis *** //
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * speedH * 100;
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * speedH * 100;

            // Invert Axis
            if(invertV) mouseY *= -1;
            if(invertH) mouseX *= -1;

            // *** Add rotation plus mouse position *** //
            rotationY += mouseX;
            rotationX += mouseY;

            // *** Clamp camera position to limit movement *** //
            rotationX = Mathf.Clamp(rotationX, -10f, 60f);
            rotationY = Mathf.Clamp(rotationY, -120f, -60f);
            //Debug.Log(rotationY);

            // *** Current frame rotation to assign *** //
            Vector3 nextRotation = new Vector3(rotationX, rotationY);

            // *** Assign desired rotation with smooth *** //
            currentRotation = Vector3.SmoothDamp(currentRotation, nextRotation, ref smoothVelocity, smoothTime);
            transform.localEulerAngles = currentRotation;

            transform.position = FishTankSelector.fishTankManager.transform.position - transform.forward * distanceFromTarget;
        } else {
            // *** Unlock mouse *** //
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void Zoom() {
        // *** FishTank not selected, reset Zoom *** //
        if(FishTankSelector.fishTankManager == null || GameEvents.instance.moving) {
            if(distanceFromTarget != 15f) {
                distanceFromTarget = 15f;
            }
            return;
        }

        // *** Zoom OUT/IN *** //
        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0) {
            if(distanceFromTarget > 10f) { // Clamp Zoom
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

    private void Start() {
        // *** Subscribe event *** //
        GameEvents.instance.onCamKeyPressed += ExecuteMoveCamera;
        GameEvents.instance.onFishTankUpdated += SetFishtankCamera;
        GameEvents.instance.onFishTankRemoved += SetGeneralCamera;

        SetGeneralCamera();
    }

    private void Update() {

        MoveCameraArroundFishtank();

        Zoom();

    }
}
