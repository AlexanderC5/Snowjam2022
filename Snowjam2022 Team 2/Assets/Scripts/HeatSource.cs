using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatSource : MonoBehaviour
{

    [SerializeField]
    private int heatLevel;
    [SerializeField]
    private int maxHeat;
    [SerializeField]
    private int minHeat;

    
    private bool playerInRadius;
    public PlayerController player;


    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        playerInRadius = false; //change for starting campfire, probably?
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (heatLevel > 0) AudioManager.manager.PlaySFX("Ambient_Fire");
            player.ChangeHeat(heatLevel);
            playerInRadius = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            AudioManager.manager.StopSFXName("Ambient_Fire");
            player.ChangeHeat(heatLevel * -1);
            playerInRadius = false;
        }
    }

    public void ChangeHeatLevel(int heatToAdd)
    {
        int oldHeat = heatLevel;
        heatLevel += heatToAdd;
        if(heatLevel > maxHeat)
        {
            heatLevel = maxHeat;
        }
        if(playerInRadius)
        {
            player.ChangeHeat(heatLevel - oldHeat); //if they add fuel/fuel goes down/etc the player's heat level will update
        }
    }

    public int GetHeatLevel()
    {
        return heatLevel;
    }

    public int GetMinHeat()
    {
        return minHeat;
    }
    
    public int GetMaxHeat()
    {
        return maxHeat;
    }
}
