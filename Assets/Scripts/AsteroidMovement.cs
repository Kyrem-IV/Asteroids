using UnityEngine;
using UnityEditor;
using System.IO;
using Random = System.Random;
using System.Collections.Generic;
using System.Collections;

public class AsteroidMovement : MonoBehaviour
{
    public GameObject asteroid;
    public GameObject manager;
    public float bigAsteroidVelocity;
    public float midAsteroidVelocity;
    public float smallAsteroidVelocity;
    public static int asteroidPoints;
    public static int startingAsteroids = 4; // amount of asteroids to be spawned at the beginning of a new level
    public static int startingAsteroidsBackup = startingAsteroids;
    public int splittingAsteroids = 2; // amount of smaller asteroids to be spawned, when a bigger asteroid has been destroyed
    public static int asteroidsAmount; // current asteroids on screen
    public static bool newLevel = false;
    public static bool firstLevel = true;
    public Vector3 bigSize;
    public Vector3 mediumSize;
    public Vector3 smallSize;
    public static List<GameObject> asteroidCollection = new List<GameObject>();
    Random rand = new Random();
    Rigidbody2D rb;
    public float delay = 4f;
    
    void Start()
    {
        if (gameObject.name == "Manager") // spawn asteroids once at the start of the program
        {
            StartCoroutine(SpawnStartingAsteroids(0f));
        }
        else // when asteroid is spawned, give it velocity based on its size
        {
            rb = transform.GetComponent<Rigidbody2D>();

            if (gameObject.transform.localScale == bigSize)
            {
                rb.AddForce(transform.up * bigAsteroidVelocity);  
            }
            else if (gameObject.transform.localScale == mediumSize)
            {
                rb.AddForce(transform.up * midAsteroidVelocity);  
            }
            else
            {
                rb.AddForce(transform.up * smallAsteroidVelocity);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && gameObject.name == "Manager") // spawn asteroids on pressing q (for testing)
        {
            float xAxis = rand.Next(3, 7);
            float yAxis = rand.Next(2, 4);

            switch(rand.Next(0, 2))
            {
                case 0:
                    xAxis = xAxis * -1;
                    yAxis  = yAxis * -1;
                    break;
                case 1:
                    yAxis = yAxis * -1;
                    break;
                case 2:
                    xAxis = xAxis * -1;
                    break;
            }

            Vector2 position = new Vector2(xAxis, yAxis);
            SpawnAsteroids(startingAsteroids, bigSize, position);
        }

        if (newLevel) // start a new level
        {
            newLevel = false;
            manager.GetComponent<PointManager>().PointGain(5);
            Debug.Log("Level complete!");
            startingAsteroids++;
            StartCoroutine(SpawnStartingAsteroids(delay));
            UFOBehaviour.ufoPresent = false;
        }
        else if (asteroidsAmount == 0 && LifeManager.livesRemaining > 0 && !UFOBehaviour.ufoPresent && !firstLevel) // if level win conditions have been met, enable a new level
        {
            newLevel = true;
            UFOBehaviour.ufoPresent = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collider) // on collision with asteroid
    {
        if (gameObject.transform.localScale == bigSize) // if it is big, replace with mid ones
        {
            Vector2 position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            SpawnAsteroids(splittingAsteroids, mediumSize, position);
            asteroidsAmount--;
            asteroidPoints = 2;
        }
        else if (gameObject.transform.localScale == mediumSize) // if it is mid, replace with small ones
        {
            Vector2 position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            SpawnAsteroids(splittingAsteroids, smallSize, position);
            asteroidsAmount--;
            asteroidPoints = 3;
        }
        else // if it is small, remove it 
        {
            asteroidsAmount--;
            asteroidPoints = 4;
        }

        if(collider.CompareTag("Player")) // the ship gets hit, resulting in a lost life and for the ship to gain temporary invincibility
        {
            collider.gameObject.SetActive(false);
            manager.GetComponent<LifeManager>().LoseALife();
        }
        else // object that hit the asteroid wasnt the ship
        {
            if (collider.CompareTag("UFO")) // if it was an ufo, remove its presence
            {
                UFOBehaviour.ufoPresent = false;
            }
            else if (collider.CompareTag("Lazer")) // if it was a lazer, grant points for the destroyed asteroid
            {
                manager.GetComponent<PointManager>().PointGain(asteroidPoints);
            }
          
            Destroy(collider.gameObject); // either way destroy it
        }

        Destroy(gameObject); // asteroid explodes 
        Debug.Log(asteroidsAmount); // writes remaining asteroids to be destroyed in log
    }

    public void SpawnAsteroids(int amount, Vector3 size, Vector2 position) // spawns new asteroids on the screen based on a given amount, size and position
    {
        for (int x = 0; x < amount; x++)
        {
            GameObject newAsteroid = Instantiate(asteroid, position, Quaternion.identity);
            newAsteroid.transform.localScale = size;
            newAsteroid.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rand.Next(0, 360))); // gives asteroid a random rotation to allow different trajectories
            asteroidCollection.Add(newAsteroid); // keeps track of asteroids in a list
            asteroidsAmount++;
        }
        firstLevel = false; // bool prevents first frame of a new level to already meet the win conditions (no ufo/asteroids)
    }

    public IEnumerator SpawnStartingAsteroids(float delay) // spawns new asteroids at the start of a level after a given delay
    {
        yield return new WaitForSeconds(delay);

        for (int x = 0; x < startingAsteroids; x++) // defines a random position for each asteroid to be created
        {
            float xAxis = rand.Next(3, 7); // right edge (left when negative)
            float yAxis = rand.Next(2, 4); // upper edge (lower when negative)

            switch(rand.Next(0, 2))
            {
                case 0:
                    xAxis = xAxis * -1;
                    yAxis  = yAxis * -1;
                    break;
                case 1:
                    yAxis = yAxis * -1;
                    break;
                case 2:
                    xAxis = xAxis * -1;
                    break;
            }

            Vector2 position = new Vector2(xAxis, yAxis);
            SpawnAsteroids(1, bigSize, position); // spawns the asteroid at the newly defined position
        }
    }
}