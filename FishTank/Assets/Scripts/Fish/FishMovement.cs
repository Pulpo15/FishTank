using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour {
    #region Macros
    const string BOUNDS_TAG = "FishTankBounds";
    const string FISHTANK_TAG = "FishTank";
    #endregion

    #region Public
    public float minSpeed, maxSpeed;
    public string foodTag;
    public float maxEatTime;
    public BreedManager.FishType fishType;
    public bool female = false;
    [HideInInspector]
    public GameObject partner;
    #endregion

    #region Private
    private GameObject fishTank;
    private Vector3 targetPosition;
    private float randomSpeed = 2;
    private float eatTime;
    private bool updateTarget;
    private bool canEat = true;
    private bool canBreed = true;
    private GameObject foodTarget;
    private BreedManager breedManager;
    private List<Transform> fishTankBounds;
    #endregion

    private void GetTargetPosition(Vector3? foodPosition) {
        // *** 
        if(targetPosition != null && updateTarget) {
            //Debug.Log("Test");
            return;
        }
        // *** Get new food position *** //
        if(foodPosition != null) {
            targetPosition = (Vector3)foodPosition;
            return;
        }
        // *** Check if there is food in FishTank *** //
        if(canEat) {
            GameObject go = GameObject.FindGameObjectWithTag(foodTag);
            if(go != null) {
                //targetPosition = go.transform.position;
                SetTargetFood(go);
                return;
            }
        }

        Bounds bounds = new Bounds();

        // *** Get defined vertex of FishTank *** //
        for(int i = 0; i < fishTankBounds.Count; i++) {
            bounds.Encapsulate(fishTankBounds[i].position);
        }

        // *** Get size of FishTank using defined vertex *** //
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        targetPosition = new Vector3(x, y, z);

        //Instantiate(fishTankBounds[0], targetPosition, Quaternion.identity);
    }

    // *** If food is correct & can breed set target *** //
    private void SetTargetFood(GameObject food) {
        if(food.tag == foodTag && !updateTarget && canEat) {
            foodTarget = food;
            updateTarget = true;
        }
    }

    private void Start() {
        // *** Assign Fish Tank *** //
        fishTank = GameObject.FindGameObjectWithTag(FISHTANK_TAG);

        // *** Assign Fish Tank Bounds *** //
        fishTankBounds = new List<Transform>();

        for(int i = 0; i < fishTank.transform.childCount; i++) {
            if(fishTank.transform.GetChild(i).tag == BOUNDS_TAG) {
                for(int x = 0; x < fishTank.transform.GetChild(i).transform.childCount; x++) {
                    fishTankBounds.Add(fishTank.transform.GetChild(i).transform.GetChild(x));
                }
            }
        }

        // *** Check assigned bounds *** //
        if(fishTankBounds.Count < 2) {
            Debug.LogWarning("Fish Tank Bounds Unassigned : " + gameObject.name);
            gameObject.SetActive(false);
        }

        // *** Assign fish Breed Manager *** //
        breedManager = gameObject.GetComponent<BreedManager>();

        // *** Assign time to eat again *** //
        eatTime = maxEatTime;

        // *** Subscribe events *** //
        GameEvents.instance.onUpdateFishTarget += GetTargetPosition;
        GameEvents.instance.onFoodInstantiate += SetTargetFood;

        // *** Set fish target at the start *** //
        GetTargetPosition(null);
    }

    void Update() {
        if(targetPosition != null) {
            if(updateTarget && foodTarget == null) { 
                updateTarget = false;
                GetTargetPosition(null); 
            } else if(updateTarget) targetPosition = foodTarget.transform.position;

            // *** If fish has reached it's target get a new one *** //
            if(transform.position == targetPosition) {
                randomSpeed = Random.Range(minSpeed, maxSpeed);
                GetTargetPosition(null);
            }
            // *** If fish has a target move towards it with random speed between assigned in editor *** //
            else {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition,
                    randomSpeed * Time.deltaTime);
                transform.LookAt(targetPosition);
            }            
        } else {
            randomSpeed = Random.Range(minSpeed, maxSpeed);
            GetTargetPosition(null);
        }

        if(!canEat) {
            eatTime -= Time.deltaTime;
            if(eatTime <= 0) {
                canEat = true;
                eatTime = maxEatTime;
            }
        }

        if(partner != null && canBreed) {
            GetTargetPosition(partner.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag(foodTag)) {
            Destroy(other.gameObject);
            updateTarget = false;
            canEat = false;
            GetTargetPosition(null);
            GameEvents.instance.SearchCouple(fishType, gameObject);
        } else if(other.gameObject == partner) {
            canBreed = false;
            partner = null;

            if (female) GameEvents.instance.BreedNewFish(fishType);
        }
    }
}
