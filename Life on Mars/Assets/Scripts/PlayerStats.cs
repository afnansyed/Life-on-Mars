using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // PLAYER
    [SerializeField] private float health = 100.0f;
    [SerializeField] private float oxygen = 100.0f;
    [SerializeField] private float walkspeed = 10.0f;

    // RESOURCES
    [SerializeField] private int rocks_Type1 = 0;
    [SerializeField] private int rocks_Type2 = 0;
    [SerializeField] private int rocks_Type3 = 0;
    [SerializeField] private int seeds_Type1 = 0;
    [SerializeField] private int seeds_Type2 = 0;
    [SerializeField] private int seeds_Type3 = 0;

    // CONSUMABLES
    [SerializeField] private int oxygenTanks = 0;
    [SerializeField] private int gasTanks = 0;
    [SerializeField] private int foodTemp = 0;

    // TOOLS
    [SerializeField] private bool hasSuit = false;
    [SerializeField] private bool hasHammer = false;
    [SerializeField] private bool hasWrench = false;
    [SerializeField] private bool hasJetpack = false;

    // WORLD
    [SerializeField] private int treesPlanted = 0;


    // OTHER (external)
    //public GameObject XR_mover;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //XR_mover.GetComponent<LocomotionMediator>().moveSpeed = walkspeed;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
