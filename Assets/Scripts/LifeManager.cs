using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;   
using System.Linq;
using System;
using TMPro;

public class LifeManager : MonoBehaviour
{
    public GameObject life;
    public GameObject player;
    public GameObject loseText;
    public GameObject canvas;
    public GameObject prompt;
    public GameObject manager;
    public int startingLives = 3;
    public static int maxLives = 10;
    float timer;
    public float interval = 1;
    public float originalLifeX = -7.5f;
    public float originalLifeY = 4.5f;
    public float respawnTimer = 1.5f; // defines how long it takes for the ship to respawn
    public float invincibility = 1.5f; 
    public static int livesRemaining;
    private static float lifeX;
    private static bool isRespawning = false;
    private static bool flicker = false;
    private static bool gameOverScreen = false;
    private static List<GameObject> lives = new List<GameObject>();
    private static List<GameObject> temporaryObjects = new List<GameObject>();
    PolygonCollider2D part;
    SpriteRenderer visual;

    void Start() // create lifes on start
    {
        part = player.GetComponent<PolygonCollider2D>();
        visual = player.GetComponent<SpriteRenderer>();
        lifeX = originalLifeX;

        for (int x = 0; x < startingLives; x++)
        {
            GainALife();
        }
        StartCoroutine(Respawn(0));
    }

    void Update()
    {
        if (isRespawning) // resets ship position on respawn
        {
            player.transform.position = new Vector3(0, 0, 0);
            player.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            StartCoroutine(Respawn(respawnTimer));
        }

        if (Input.GetKeyDown(KeyCode.O)) // cheat to gain a life for free
        {
            GainALife();
        }

        if (Input.GetKeyDown(KeyCode.Return) && gameOverScreen) // restarts the game after a game over
        {
            gameOverScreen = false;
            Restart();
        }

        if (flicker) // enables and disables the ships sprite renderer for a flicker effect for when it is invincible, after respawning
        {
            timer += Time.deltaTime;
            if (timer > interval)
            {
                visual.enabled = !visual.enabled;
                timer -= interval;
            }
        }
        else if (!visual.enabled)
        {
            visual.enabled = true;
        }
    }

    IEnumerator Respawn(float timer) // respawns the ship
    {
        yield return new WaitForSeconds(timer);

        player.SetActive(true);
        part.enabled = false;
        isRespawning = false;
        flicker = true;
        StartCoroutine("Invincible");
    }

    IEnumerator Invincible() // makes the ship during its respawn invincible 
    {
        yield return new WaitForSeconds(invincibility);
     
        part.enabled = true;
        flicker = false;
    }

    public void LoseALife() // makes the player lose a life
    {
        livesRemaining--;
        
        if (livesRemaining >= 0) // if lives are still remaining, just lose a life
        {
            Destroy(lives[lives.Count - 1]);
            lives.RemoveAt(lives.Count - 1);
            isRespawning = true;
            lifeX -= 0.6f;
        }
        else // otherwise go game over
        {
            GameOver();
        }
    }

    public void GainALife() // makes the player gain a life
    {
        if (livesRemaining < maxLives) // only as long as it wouldnt exceed the life limit
        {
            GameObject go = Instantiate(life, new Vector2(lifeX, originalLifeY), Quaternion.identity);
            lifeX += 0.6f;
            lives.Add(go);
            livesRemaining++;
        }
    }

    void GameOver() // makes the player go game over, prompting them to restart the game
    {
        GameObject go = Instantiate(loseText, new Vector2(0, 0), Quaternion.identity);
        GameObject stop = Instantiate(prompt, new Vector2(0, -150), Quaternion.identity);
        stop.transform.SetParent(PointManager.holder.transform, false);
        temporaryObjects.Add(go);
        temporaryObjects.Add(stop);

        gameOverScreen = true;
    }

    void Restart() // resets all necessary variables to their default states on a game restart
    {
        livesRemaining = 0;
        AsteroidMovement.asteroidsAmount = 0;
        AsteroidMovement.startingAsteroids = AsteroidMovement.startingAsteroidsBackup;
        for (int x = 0; x < startingLives; x++)
        {
            GainALife();
        }
        foreach (var t in AsteroidMovement.asteroidCollection)
        {
            Destroy(t);
        }
        foreach (var t in temporaryObjects)
        {
            Destroy(t);
        }

        AsteroidMovement asteroidMovement = manager.GetComponent<AsteroidMovement>();
        AsteroidMovement.firstLevel = true;
        StartCoroutine(asteroidMovement.SpawnStartingAsteroids(5f));
        player.SetActive(true);
        player.transform.position = new Vector3(0, 0, 0);
        player.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        UFOBehaviour.ufoPresent = false;
        PointManager.currentNewLifeThreshold = PointManager.newLifeThreshold;
        PointManager.currentPoints = 0;
        PointManager.counter.text = PointManager.currentPoints.ToString();
        if (UFOBehaviour.currentSaucer != null)
        {
            Destroy(UFOBehaviour.currentSaucer);
        }
    }
}
