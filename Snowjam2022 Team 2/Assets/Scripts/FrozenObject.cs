using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenObject : MonoBehaviour
{
    [SerializeField] GameObject frozenObject;
    [SerializeField] GameObject frozenSprite;
    [SerializeField] GameObject thawSprite;
    [SerializeField] float timeToMeltStage = 5;

    private bool melting;
    private float meltStageTimer;
    private int stage;
    private bool canMelt;

    private AudioManager audioManager;
    


    PlayerController playerController;

    // Start is called before the first frame update
    void Awake()
    {
        audioManager = GameObject.Find("GameSettings").GetComponent<AudioManager>();
        frozenObject.SetActive(false);
        stage = 2; //may need to change
        canMelt = false;
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        meltStageTimer = timeToMeltStage;
    }

    // Update is called once per frame
    void Update()
    {
        if(canMelt && playerController.UsingTorch())
        {
            meltStageTimer -= Time.deltaTime;
            if(meltStageTimer <= 0)
            {
                if(stage > 1)
                {
                    meltStageTimer = timeToMeltStage;
                    stage -= 1;
                    frozenSprite.SetActive(false);
                    thawSprite.SetActive(true);
                    audioManager.PlaySFX("Interact_IceMelt2");
                }
                else //fully melted
                {
                    audioManager.PlaySFX("Interact_IceMelt1");
                    frozenObject.SetActive(true);
                    thawSprite.SetActive(false);
                    gameObject.SetActive(false); //turn self off
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canMelt = true;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canMelt = false;
        }
    }



}
