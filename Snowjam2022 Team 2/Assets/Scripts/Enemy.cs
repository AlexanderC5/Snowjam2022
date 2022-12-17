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
    private Transform target;

    private Vector2 moveDirection;
    private bool isAggro;
    private bool isIdle;
    private float roamTimer, idleTimer;
    [SerializeField] private float roamMaxDuration = 4f, idleMaxDuration = 7f;
    private float roamDuration, idleDuration;

    [SerializeField] private int damage = 3;

    [SerializeField] private int health = 10;
    [SerializeField] private float stunLength = 0.25f;
    private float stunTimer;
    private bool stunned;

    private void Awake()
    {
        stunTimer = 0;
        stunned = false;
        rb = GetComponent<Rigidbody2D>();
        target = FindObjectOfType<PlayerController>().transform; //maybe have an aggro radius?
        playerMask = LayerMask.GetMask("Player");
        InitiateRoaming();
    }

    // Update is called once per frame
    void Update()
    {
        if (stunned)
        {
            if (stunTimer >= stunLength)
            {
                stunTimer = 0;
                stunned = true;
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
                rb.rotation = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
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
        rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
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
            rb.rotation = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
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
        if (health <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        Destroy(gameObject);
        //Drops?
    }
}
