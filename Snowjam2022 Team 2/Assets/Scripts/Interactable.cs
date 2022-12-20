using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    [SerializeField]
    private string itemName;

    //defaults to picking up
    public virtual void Interact(PlayerController playerController)//GameObject player)
    {
        //PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.AddItem(itemName);
        AudioManager.manager.PlaySFX("Interact_Pickup");
        Destroy(gameObject); //remove self from world
    }

    public virtual void HoldInteract(PlayerController playerController)
    {
        //pass
    }

    public virtual void StopInteract(PlayerController playerController)
    {
        //pass
    }

    public string GetName()
    {
        return itemName;
    }
}
