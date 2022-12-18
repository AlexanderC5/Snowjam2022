using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    private Dictionary<string, int> inv = new Dictionary<string, int>(); //inventory

    //crafting
    [SerializeField]
    private List<CraftableItem> craftList;

    //private List<Interactable> interactList = new List<Interactable>(); //if this is needed, attatch a script to the objects that inform the player of ontriggerleave() so you can remove the right one?
    private Interactable lastInteract; //tracks what you can interact with (most recent collision with an interactable)
    //private string lastInteractName; useless, unless pick up items are overlapping for some reason


    [SerializeField]
    private float heatLevel; //how warm the player is

    [SerializeField]
    private int choppingTime; //time to chop down a tree

    [SerializeField] //need to see it in editor for testing
    private int health; //health. 
    private int maxHealth;
    public bool isInvulnerable {get; private set;}
    private float invulnTimer;
    private float invulnDuration;
    [SerializeField] //so you can see freeze levels in editor
    private float freeze; //how frozen the player is.
    private float maxFreeze;

    private GameUI gameUI; // Get sceneUI

    //torch
    int usingTorch;
    [SerializeField] int torchBurnTime = 30;

    //player movement
    public float moveSpeed = 5f;

    public Rigidbody2D rb;

    Vector2 movement;

    public Animator animator;


    //lighting
    [SerializeField] GameObject torchLight;
    [SerializeField] GameObject baseLight;


    //attacking
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackCooldown = 0.5f;
    private float attackCooldownTimer;


    //fishing
    private bool catchChance;
    private bool fishing;

    //alert
    [SerializeField] GameObject alert;


    // Start is called before the first frame update

        //changed to awake for efficiency
    void Awake()
    {
        torchLight.SetActive(false);
        baseLight.SetActive(true);
        usingTorch = 0;
        health = 100; //who knows, maybe up this
        maxHealth = 100;
        isInvulnerable = false;
        invulnTimer = 0f;
        invulnDuration = 1f; //could change
        freeze = 0;
        maxFreeze = 100;

        heatLevel = 0; //maybe change
        inv["Torch"] = 0; //this one needs to be here 
        //inv["Wood"] = 0; 
        //inv["Stick"] = 0;

        //numbers for testing, mostly
        inv["Wood"] = 3;
        inv["Torch"] = 5;
        inv["Stick"] = 2;

        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();

        gameUI = GameObject.Find("Canvas").GetComponent<GameUI>();
    }

    // Update is called once per frame
    void Update()
    {
        //damage cooldown
        if (isInvulnerable)
        {
            invulnTimer += Time.deltaTime;
        }

        //movement
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
        //Debug.Log(heatLevel);
        if (Input.GetKeyDown(KeyCode.E) && lastInteract != null)// && interactList.Count > 0)
        {
            lastInteract.Interact(this);
            //interactList[0].Interact(this);
            //interactList.RemoveAt(0);
        }
        if (Input.GetKey(KeyCode.E) && lastInteract != null)// && interactList.Count > 0)
        {
            lastInteract.HoldInteract(this); //used for tree chopping/other long interactions
        }
        //torch
        if (Input.GetKeyDown(KeyCode.Q) && inv["Torch"] > 0)
        {
            StartCoroutine(UseTorch()); //TODO
        }

        if(Input.GetMouseButtonDown(0) && attackCooldownTimer <= 0)
        {
            Attack();
            attackCooldownTimer = attackCooldown;
        }
        if(attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
        // Update UI
        gameUI.SetHealthUI(health);
        gameUI.SetTemperatureUI(100-freeze);


        //fishing

        //cancel fish if move
        if (fishing && (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0))
        {
            Debug.Log("fishing canceled :(((");
            fishing = false;
        }
        if (catchChance && Input.GetMouseButtonDown(0))
        {
            AddItem("Fish");
            catchChance = false;
            fishing = false;
        }

    }

    //move player
    void FixedUpdate()
    {
        // Movement
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Interactable")
        {
            lastInteract = collision.GetComponent<Interactable>();
            Debug.Log(lastInteract.GetName());
            //lastInteractName = lastInteract.GetName();
            //interactList.Insert(0, obj);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Interactable")
        {
            lastInteract = null;
        }
    }


    //inventory
    public void AddItem(string item)
    {
        try
        {
            inv[item] += 1;
        }
        catch
        {
            inv[item] = 1;
        }
        Debug.Log(inv[item]);
    }

    public void RemoveItem(string item)
    {
        try
        {
            if(inv[item] > 0)
            {
                inv[item] -= 1;
            }
        }
        catch
        {
            return;
        }
    }

    public int GetItem(string item)
    {
        try
        {
            return inv[item];
        }
        catch
        {
            return 0;
        }

    }

    public Dictionary<string, int> GetDict()
    {
        try
        {
            return inv;
        }
        catch
        {
            return null;
        }

    }

    //torch
    private IEnumerator UseTorch()
    {
        inv["Torch"] -= 1;
        Debug.Log("torch moment");
        torchLight.SetActive(true);
        baseLight.SetActive(false);
        heatLevel += 0.5f;
        usingTorch += 1; //using an int so if you start burning a second torch it "resets" the timer.
        yield return new WaitForSeconds(torchBurnTime);
        heatLevel -= 0.5f;
        usingTorch -= 1;
        torchLight.SetActive(false);
        baseLight.SetActive(true);
    }

    public bool UsingTorch()
    {
        if (usingTorch > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
            
    }

    //heat
    public float GetHeat()
    {
        return heatLevel;
    }

    public void ChangeHeat(float heatToAdd)
    {
        heatLevel += heatToAdd;
    }


    //trees
    public int GetChoppingTime()
    {
        return choppingTime;
    }


    //health/damage
    public int GetHealth()
    {
        return health;
    }

    public void ChangeHealth(int healthToAdd)
    {
        if (isInvulnerable && healthToAdd < 0) {} // If invuln + taking damage, don't add damage
        else health += healthToAdd;
        if (healthToAdd < 0) // Implements cooldown from taking damage
        {
            if (!isInvulnerable)
            {
                isInvulnerable = true;
            }
            else if (invulnTimer > invulnDuration)
            {
                isInvulnerable = false;
                invulnTimer = 0f;
            }
        }
        
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        else if (health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Debug.Log("You Died. RIP.");
    }

    //freezing
    public void ChangeFreeze(float freezeToAdd)
    {
        //Debug.Log(freezeToAdd);
        freeze += freezeToAdd;
        if (freeze < 0)
        {
            freeze = 0;
        }
        else if (freeze > maxFreeze)
        {
            freeze = maxFreeze;
        }
    }

    public float GetFreeze()
    {
        return freeze;
    }

    //crafting

    public bool Craft(CraftableItem item)
    {
        bool success = true;
        Dictionary<string, int> tempInv = inv;

        foreach (string material in item.requiredMaterials)
        {
            try
            {
                if(tempInv[material] > 0)
                {
                    tempInv[material] -= 1;
                }
                else
                {
                    success = false;
                    break;
                }
            }
            catch
            {
                success = false;
                break;
            }
        }
        if(success)
        {
            if (tempInv.ContainsKey(item.itemName))
            {
                tempInv[item.itemName] += 1; //todo; handle the different types
            }
            else
            {
                tempInv.Add(item.itemName, 1);
            }
                
            
            inv = tempInv;
        }
        return success;
    }

    public List<CraftableItem> getCraftableItems()
    {
        return craftList;
    }


    //attack

    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        
        foreach(Collider2D enemy in hitEnemies)
        {
            /*
            if (enemy.tag == "enemy")
            {

            }*/
            enemy.GetComponent<Enemy>().GetHit(attackDamage);
        }
       
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }


    //alert (it's fishin' time)

public void Fish()
    {
        fishing = true;
        StartCoroutine(StartFish());
    }
    


    private IEnumerator StartFish()
    {
        
        yield return new WaitForSeconds(Random.Range(5, 10));
        if(fishing)
        {
            catchChance = true;
            Debug.Log("FISH TIME");
            StartCoroutine(Alert());
            yield return new WaitForSeconds(1);
            catchChance = false;
            fishing = false;
        }
    }
    
    private IEnumerator Alert()
    {
        alert.SetActive(true);
        yield return new WaitForSeconds(1);
        alert.SetActive(false);
    }
}



