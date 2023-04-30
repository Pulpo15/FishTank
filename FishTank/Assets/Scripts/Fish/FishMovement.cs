using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour {
    #region Macros
    const int MAX_POSITION = 1;
    const int MIN_POSITION = 0;
    #endregion

    #region Public
    public float minSpeed, maxSpeed;
    public Transform[] fishTankBounds;
    public string foodTag;
    #endregion

    #region Private
    private Vector3 targetPosition;
    private float randomSpeed = 2;
    private bool updateTarget;
    private GameObject foodTarget;
    #endregion

    private void GetTargetPosition(Vector3? foodPosition) {
        if(targetPosition != null && updateTarget) {
            Debug.Log("Xd");
            return; 
        }
        // *** Get new food position *** //
        if(foodPosition != null) {
            targetPosition = (Vector3)foodPosition;
            return;
        }
        // *** Check if there is food in FishTank *** //
        GameObject go = GameObject.FindGameObjectWithTag(foodTag);
        if(go != null) {
            //targetPosition = go.transform.position;
            SetTargetFood(go);
            return;
        }

        Bounds bounds = new Bounds();

        // *** Get defined vertex of FishTank *** //
        for(int i = 0; i < fishTankBounds.Length; i++) {
            bounds.Encapsulate(fishTankBounds[i].position);
        }

        // *** Get size of FishTank using defined vertex *** //
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        targetPosition = new Vector3(x, y, z);

        //Instantiate(fishTankBounds[0], targetPosition, Quaternion.identity);
    }

    private void SetTargetFood(GameObject food) {
        if(food.tag == foodTag && !updateTarget) {
            foodTarget = food;
            updateTarget = true;
        }
    }

    private void Start() {
        // *** Check assigned bounds *** //
        if(fishTankBounds.Length < 2) {
            Debug.LogWarning("Fish Tank Bounds Unassigned");
            gameObject.SetActive(false);
        }

        // *** Subscribe events *** //
        GameEvents.instance.onUpdateFishTarget += GetTargetPosition;
        GameEvents.instance.onFoodInstantiate += SetTargetFood;

        // *** Set fish target at the start *** //
        GameEvents.instance.UpdateFishTarget(null);
    }

    void Update() {
        if(targetPosition != null) {
            if(updateTarget && foodTarget == null) { 
                updateTarget = false; 
                GameEvents.instance.UpdateFishTarget(null); 
            } else if(updateTarget) targetPosition = foodTarget.transform.position;

            // *** If fish has reached it's target get a new one *** //
            if(transform.position == targetPosition) {
                randomSpeed = Random.Range(minSpeed, maxSpeed);
                GameEvents.instance.UpdateFishTarget(null);
            }
            // *** If fish has a target move towards it with random speed between assigned in editor *** //
            else {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition,
                    randomSpeed * Time.deltaTime);
                transform.LookAt(targetPosition);
            }            
        } else {
            randomSpeed = Random.Range(minSpeed, maxSpeed);
            GameEvents.instance.UpdateFishTarget(null);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag(foodTag)) {
            Destroy(other.gameObject);
            updateTarget = false;
            GameEvents.instance.UpdateFishTarget(null);
        }
    }
}
