using UnityEngine;
using Random = System.Random;

public class ShipMovement : MonoBehaviour
{
    public float degreesPerSec = 360f; // defines how quickly the ship can rotate
    public float movementVelocity = 2f; // defines the forward momentum of the ship
    public float maxVelocity = 10f; // defines the maximum speed the ship can hold
    public float drag = 5f; // defines how quickly the ship slows down, when not currently being accelerated
    public float lazerSpeed = 3f; // defines speed of the lasers
    public static bool powerUp = false;
    public Vector3 powerUpSize;
    public GameObject lazer;
    public GameObject player;
    public GameObject manager;
    static GameObject instantiatedLazer;
    Random rand = new Random();
    Rigidbody2D rb;
    Rigidbody2D rbl;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float rotAmount = degreesPerSec * Time.deltaTime; // amount by which the ship is to rotate
        float curRot = transform.localRotation.eulerAngles.z; // the ships current rotation

        if (Input.GetKey(KeyCode.UpArrow) && rb.linearVelocity.magnitude <= maxVelocity) // move up 
        {
            rb.linearDamping = 0;
            rb.AddForce(transform.up * movementVelocity);
        }
        // else if (Input.GetKey(KeyCode.DownArrow) && rb.linearVelocity.magnitude <= maxVelocity) // move down
        // {
        //     rb.linearDamping = 0;
        //     rb.AddForce(transform.up * - movementVelocity);
        // }
        else // if no momentum is added, the ship slows down
        {
            rb.linearDamping = drag;
        }

        if (Input.GetKey(KeyCode.LeftArrow)) // rotate left
        {
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, curRot + rotAmount));
        }
        if (Input.GetKey(KeyCode.RightArrow)) // rotate right
        {
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, curRot - rotAmount));
        }
        if(Input.GetKeyDown(KeyCode.Space)) // shoot laser
        {
            instantiatedLazer = Instantiate(lazer, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
            rbl = instantiatedLazer.GetComponent<Rigidbody2D>();
            rbl.AddForce(transform.up * lazerSpeed);
            instantiatedLazer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, curRot));

            if (powerUp) // if a power-up is collected, the ship shoots bigger lazers instead
            {
                instantiatedLazer.transform.localScale = powerUpSize;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) // FTL
        {
            player.transform.position = new Vector2(rand.Next(-6, 9), rand.Next(-4, 5));
        }
    }

    void OnTriggerEnter2D(Collider2D collider) // ship collides with another object
    {
        if (collider.CompareTag("Beam")) // the object is a beam and is destroyed alongside the ufo
        {
            Destroy(collider.gameObject);
            gameObject.SetActive(false);
            manager.GetComponent<LifeManager>().LoseALife();
        }
        else if (collider.CompareTag("UFO")) // the object is an ufo and only the ship is destroyed
        {
            gameObject.SetActive(false);
            manager.GetComponent<LifeManager>().LoseALife();
        }
    }
}
