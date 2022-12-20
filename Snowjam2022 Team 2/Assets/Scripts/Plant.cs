using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Interactable
{

    float collectTime;
    // Start is called before the first frame update
    void Start()
    {
        collectTime = 0;
    }

    // Update is called once per frame

    public override void Interact(PlayerController playerController)
    {
        AudioManager.manager.PlaySFX("Interact_Plant");
    }
    public override void HoldInteract(PlayerController playerController)
    {
        collectTime += Time.deltaTime; //the player calls this function off of update() so this works
        if(collectTime > 1) //1 second to harvest a plant
        {
            AudioManager.manager.PlaySFX("Interact_Pickup");
            AudioManager.manager.StopSFXName("Interact_Plant");
            float random = Random.Range(1, 10);
            if(random < 9)
            {
                playerController.AddItem("Plant Matter");
                playerController.AddItem("Plant Matter");
            }
            else
            {
                playerController.AddItem("Plant Matter");
                playerController.AddItem("Herbs"); //% chance to get herbs
            }
            Destroy(gameObject);
        }
    }
    public override void StopInteract(PlayerController playerController)
    {
        AudioManager.manager.StopSFXName("Interact_Plant");
    }
}
