using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //https://youtu.be/ZExSz7x69j8
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float aggroRange = 5f;
    private LayerMask playerMask;
    private Rigidbody2D rb;
    private AudioManager audioManager;
    private Transform target;

    private Vector2 moveDirection;
    private bool isAggro;
    private bool isIdle;
    private float roamTimer, idleTimer;
    [SerializeField] private float roamMaxDuration = 4f, idleMaxDuration = 7f;
    private float roamDuration, idleDuration;

    [SerializeField] private int damage = 3;

    [SerializeField] private int health = 10;
    [SerializeField] private float stunLength = 0.15f;
    private float stunTimer;
    private bool stunned;

    public Animator animator;
    private bool doesAnimatorExist = true;

    private GameManager gameManager; // Get Game Manager

    private void Awake()
    {
        stunTimer = 0;
        stunned = false;
        rb = GetComponent<Rigidbody2D>();
        audioManager = AudioManager.manager;
        target = FindObjectOfType<PlayerController>().transform; //maybe have an aggro radius?
        playerMask = LayerMask.GetMask("Player");
        InitiateRoaming();

        try
        {
            animator = this.GetComponentsInChildren<Animator>()[0];
        }
        catch
        {
            doesAnimatorExist = false; // No animator exists, skip all animation code segments
        }
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.IsGameOver()) { return; } // Disable the enemy if game over

        if (stunned)
        {
            if (stunTimer >= stunLength)
            {
                stunTimer = 0;
                stunned = false;
            }
            else
            {
                stunTimer += Time.deltaTime;
            }
        }
        else
        {
            if (isIdle)
            {
                idleTimer += Time.deltaTime;
            }
            else
            {
                roamTimer += Time.deltaTime;
            }

            RaycastHit2D raycast = Physics2D.Raycast(transform.position, (target.position - transform.position).normalized, aggroRange, playerMask);
            // Player is in range --> target player
            if (aggroRange == 0f || raycast.collider != null)
            {
                isAggro = true;
                moveDirection = (target.position - transform.position).normalized;
                //rb.rotation = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            }
            // Player is not in range --> roam
            else
            {
                // Was just aggro --> start roaming again
                if (isAggro)
                {
                    isAggro = false;
                    InitiateRoaming();
                }
                // Wasn't just aggro --> continue roaming
                else HandleRoaming();
            }
        }
    }

    private void FixedUpdate()
    {
        if(!stunned && !gameManager.IsGameOver())
        {
            rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;

            // Animation
            if (doesAnimatorExist == true)
            {
                int animationDirection = 2;
                if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(rb.velocity.y))
                {
                    if (rb.velocity.x > 0) animationDirection = 0;
                    else animationDirection = 1;
                }
                else
                {
                    if (rb.velocity.y < 0) animationDirection = 2;
                    else animationDirection = 3;
                }
                animator.SetInteger("Direction", animationDirection);
            }
        } 
    }

    private void InitiateRoaming()
    {
        isAggro = false;
        moveDirection = Vector2.zero;
        roamTimer = 0f;
        idleTimer = 0f;
        idleDuration = Random.Range(0f, idleMaxDuration);
        isIdle = true;
    }

    private void HandleRoaming()
    {
        if (isIdle && idleTimer > idleDuration)
        {
            isIdle = false;
            idleTimer = 0f;
            roamDuration = Random.Range(0f, roamMaxDuration);
            
            moveDirection = Random.insideUnitCircle;
            //rb.rotation = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        }
        else if (!isIdle && roamTimer > roamDuration)
        {
            isIdle = true;
            roamTimer = 0f;
            idleDuration = Random.Range(0f, idleMaxDuration);

            moveDirection = Vector2.zero;
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.tag == "Player")
        {
            target.GetComponent<PlayerController>().ChangeHealth(-damage);
        }
    }

    public void GetHit(int damage)
    {
        health -= damage;
        stunned = true; //hitstun

        KnockBack();

        if (health <= 0)
        {
            Death();
        }
    }



    private void KnockBack()
    {
        float timer = 0;
        float kbmult = 15; //change this value to increase/decrease knockback
        while (timer < stunLength)
        {
            rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * kbmult * -1;
            timer += Time.deltaTime;
            kbmult -= Time.deltaTime;
        }
    }


    private void Death()
    {
        audioManager.PlayRandomSFX("Ice_Break");
        Destroy(gameObject);
        //Drops?
    }
}
