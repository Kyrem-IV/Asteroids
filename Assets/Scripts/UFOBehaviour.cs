using UnityEngine;
using System.IO;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class UFOBehaviour : MonoBehaviour
{
    public GameObject saucer;
    public GameObject beam;
    public GameObject manager;
    public static GameObject currentSaucer;
    private GameObject instantiatedBeam;
    private Rigidbody2D myRigidBody;
    private Rigidbody2D rb;
    private Rigidbody2D rbl;
    public Vector3 smallSize;
    public int trajectoryChangeFrequency;
    public int smallSauceProbability;
    public float changeTrajectoryDelay;
    public float beamFrequency;
    public float sauceVelocity;
    public float smallSauceVelocity;
    public float beamVelocity;
    public float sauceSpawnIntervalMin;
    public float sauceSpawnIntervalMax;
    private float ufoX;
    private float ufoZ;
    private static int rand;
    public static bool ufoPresent = false;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        if (gameObject.name != "Manager")
        {
            SaucePhysics();
            StartCoroutine("ShootBeam");
        }
    }

    void Update()
    {
        // get the screen position of object in pixels
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        // get right side of the screen in world units
        float rightSideOfScreenInWorld = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x;
        float leftSideOfScreenInWorld = Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)).x;
        
        float topOfScreenInWorld = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).y;
        float bottomOfScreenInWorld = Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)).y;

        // if ufo is moving through left side of the screen, destroy it
        if (screenPos.x <= 0 && myRigidBody.linearVelocity.x < 0 && currentSaucer != null)
        {
    	    Destroy(currentSaucer);
            ufoPresent = false;
        }
        // if ufo is moving through right side of the screen, destroy it
        else if (screenPos.x >= Screen.width && myRigidBody.linearVelocity.x > 0 && currentSaucer != null)
        {
            Destroy(currentSaucer);
            ufoPresent = false;
        }
        
        // if ufo is moving through the top of screen, come back out at the bottom
        else if (screenPos.y >= Screen.height && myRigidBody.linearVelocity.y > 0)
        {
            transform.position = new Vector2(transform.position.x, bottomOfScreenInWorld);
        }
        // if ufo is moving through the bottom of screen, come back out at the top
        else if (screenPos.y <= 0 && myRigidBody.linearVelocity.y < 0)
        {
            transform.position = new Vector2(transform.position.x, topOfScreenInWorld);
        }

        if (gameObject.name == "UFO(Clone)") // if script is attached to an ufo object, shoot a beam occasionally
        {
            StartCoroutine("ShootBeam");
        }

        if (!ufoPresent && gameObject.name == "Manager") // if script is attached to the manager object, spawn an ufo occasionally, when there isnt one currently present
        {
            StartCoroutine("SpawnTimer");
            ufoPresent = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collider) // when the ufo collides with a lazer 
    {
        if (collider.CompareTag("Lazer"))
        {
            if (gameObject.transform.localScale == smallSize) // grant points based on the size of the ufo
            {
                manager.GetComponent<PointManager>().PointGain(1);
            }
            else
            {
                manager.GetComponent<PointManager>().PointGain(0);
            }

            Destroy(collider.gameObject); // destroy the lazer
            Destroy(gameObject); // destroy the ufo
            ufoPresent = false;
        }
    }

    void SaucePhysics() // gives the ufo its velocity based on its size
    {
        rb = transform.GetComponent<Rigidbody2D>();
        
        if (transform.localScale != smallSize)
        {
            rb.AddForce(transform.up * sauceVelocity);
        }
        else
        {
            rb.AddForce(transform.up * smallSauceVelocity);
        }
    }

    void UFOSpawner() // spawns an ufo object
    {
        if (Random.Range(0, 2) == 0) // defines where it spawns
        {
            ufoX = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x + 1; // ufo spawns on the right side of the screen
            ufoZ = 90; // rotate towards the left
        }
        else
        {
            ufoX = Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)).x - 1; // ufo spawns on the left side of the screen
            ufoZ = 270; // rotate towards the right
        }

        currentSaucer = Instantiate(saucer, new Vector2(ufoX, Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)).y + 1, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).y - 1)), Quaternion.identity);
        currentSaucer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, ufoZ)); // set rotation
    }

    IEnumerator SpawnTimer() // spawn the ufo after a random delay
    {
        yield return new WaitForSeconds(Random.Range(sauceSpawnIntervalMin, sauceSpawnIntervalMax));

        
        UFOSpawner();
        StopCoroutine("SpawnTimer");
        
        if (Random.Range(0, smallSauceProbability) == 0) // has a chance to make the ufo a small ufo
        {
            currentSaucer.transform.localScale = smallSize;
        }
    }

    IEnumerator ShootBeam() // ufo shoots a beam at a defined interval
    {
        yield return new WaitForSeconds(beamFrequency);

        instantiatedBeam = Instantiate(beam, new Vector2(transform.position.x, transform.position.y), Quaternion.identity); // instantiate a beam object
        rbl = instantiatedBeam.GetComponent<Rigidbody2D>();

        if (currentSaucer.transform.localScale == smallSize && GameObject.FindGameObjectWithTag("Player") != null) // while the ship isnt respawning and the ufo is small, shoot beams directly at the ship
        {
            var target = GameObject.FindGameObjectWithTag("Player");
            Vector3 offset = target.transform.position - instantiatedBeam.transform.position; // offset between ship and beam
            instantiatedBeam.transform.localRotation = Quaternion.LookRotation(Vector3.forward, offset);
        }
        else // otherwise shoot beam in a random direction
        {
            ufoZ = Random.Range(0, 361);
            instantiatedBeam.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, ufoZ));
        }

        rbl.AddForce(instantiatedBeam.transform.up * beamVelocity); // give beam its velocity

        if (Random.Range(0, trajectoryChangeFrequency) == 0) // after a beam has been shot, theres a chance of the ufo changing its trijectory
        {
            StartCoroutine("ChangeTrajectory");
        }

        StopCoroutine("ShootBeam");
    }

    IEnumerator ChangeTrajectory() // changes the trajectory of the ufo
    {
        yield return new WaitForSeconds(changeTrajectoryDelay);

        if (Random.Range(0, 2) == 0) // changes its trajectory to go up
        {
            rb = transform.GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;
            rb.AddForce((transform.up + transform.right) * sauceVelocity);
        }
        else // changes its trajectory to go down
        {
            rb = transform.GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;
            rb.AddForce((transform.up - transform.right) * sauceVelocity);
        }

        yield return new WaitForSeconds(changeTrajectoryDelay); // after another delay, the ufo goes straight again

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(transform.up * sauceVelocity);

        StopCoroutine("ChangeTrajectory");
    }
}

    // instantiate ufo at (random) intervals on the left or right of the screen // done
    // make them shoot lasers // done
    // they get hit by the asteroids // done
    // if they reach the edge of the screen they get destroyed instead of wrapping around // done
    // -> wrap only at the top and bottom of the screen // done
    // -> lasers still wrap around normally though // done
    // only one ufo on the screen at all times // done
    // move them in a straight line // done
    // -> changes y axis sometimes by flying diagonially up or down // done
    // reset data on new level/reset // done

    // add additional behaviour specific to small saucers // done
    // -> faster // done
    
    // -> shoot directly at the player instead of randomly in any direction // done