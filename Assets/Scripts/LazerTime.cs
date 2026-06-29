using UnityEngine;

public class LazerTime : MonoBehaviour
{
    public float lazerLife = 1.7f; // defines how long the laser lasts on the screen before being removed

    void Update()
    {
        lazerLife -= Time.deltaTime;
        
        if (lazerLife <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
