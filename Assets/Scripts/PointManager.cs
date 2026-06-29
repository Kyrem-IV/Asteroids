using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PointManager : MonoBehaviour
{
    public static int currentPoints;
    public int bigSaucePoints;
    public int smallSaucePoints;
    public int bigAsteroidPoints;
    public int midAsteroidPoints;
    public int smallAsteroidPoints;
    public int levelBonus;
    public static int newLifeThreshold = 20;
    public static int currentNewLifeThreshold;
    public TextMeshProUGUI txt;
    public GameObject canvas;
    public GameObject manager;
    public static TextMeshProUGUI counter;
    public static GameObject holder;

    void Start()
    {
        currentNewLifeThreshold = newLifeThreshold;
        holder = Instantiate(canvas, new Vector2(0, 0), Quaternion.identity); // canvas
        counter = Instantiate(txt, new Vector2(330, 250), Quaternion.identity); // point counter
        counter.transform.SetParent(holder.transform, false);
    }

    public void PointGain(int points) // gains points 
    {
        switch(points)
        {
            case 0:
                currentPoints += bigSaucePoints;
                break;
            case 1:
                currentPoints += smallSaucePoints;
                break;
            case 2:
                currentPoints += bigAsteroidPoints;
                break;
            case 3:
                currentPoints += midAsteroidPoints;
                break;
            case 4:
                currentPoints += smallAsteroidPoints;
                break;
            case 5:
                currentPoints += levelBonus;
                break;
        }

        if (currentPoints >= currentNewLifeThreshold && LifeManager.livesRemaining < LifeManager.maxLives) // gains a live, when a defined pointsthreshold has been passed
        {
            currentNewLifeThreshold += newLifeThreshold;
            manager.GetComponent<LifeManager>().GainALife();
        }

        counter.text = currentPoints.ToString(); // updates the text to reflect on the new points
    }
}
