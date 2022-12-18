using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Interactable
{

    float chopTime;
    [SerializeField] int treeWoodAmount = 3;

    // Start is called before the first frame update
    void Start()
    {
        chopTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact(PlayerController playerController)
    {
        //do nothing
    }
    public override void HoldInteract(PlayerController playerController)
    {
        playerController.SetToolSprite("Axe");
        chopTime += Time.deltaTime; //the player calls this function off of update() so this works
        if(chopTime > playerController.GetChoppingTime())
        {
            for (int i = 1; i < treeWoodAmount; i++) playerController.AddItem("Wood"); // Add treeWoodAmount wood
            playerController.SetToolSprite("None");
            Destroy(gameObject);
        }
    }
}
