using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PowerUpManager : MonoBehaviour
{
    public float powerUpTime;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player")) // on collision with the ship, the ship is upgraded
        {
            ShipMovement.powerUp = true;
            StartCoroutine("PowerUpTimer");
        }
    }

    public IEnumerator PowerUpTimer() // the upgrade is removed after a defined time
    {
        yield return new WaitForSeconds(powerUpTime);
        
        ShipMovement.powerUp = false;
    }
}
