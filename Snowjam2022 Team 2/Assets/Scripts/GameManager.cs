using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{

    private float timer;
    private float tempLevel;
    private int waveNum;

    [SerializeField]
    private float waveInterval = 10;

    private float waveTimer;

    //[SerializeField]
    //private int[] tempChangeTimings;

    [SerializeField] private int[] dayTemperatures;
    [SerializeField] private int dayLength;
    [SerializeField] private int nightLength;
    private bool day;
    private float dayNightTimer;
    private int dayCount;

    // Temperature
    [SerializeField]
    private float freezeMultiplier;
    [SerializeField]
    private float freezeDamageInterval;
    private float freezeTimer;

    // Lights
    [SerializeField] private Light2D globalLight;
    private float nightLight = 0.02f;
    private float dayLight = 0.6f;

    // Player
    private PlayerController playerController;
    private bool isGameOver;
    [SerializeField] bool disableGameOver = false; // For easier testing of other features

    // Enemies
    [SerializeField] private GameObject basicEnemy;
    [SerializeField] private GameObject basicEnemyWander;
    private GameObject eliteEnemy;

    // Access UI
    private GameUI gameUI;

    //Access audio
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Awake()
    {

        dayCount = 0;
        day = true;
        freezeTimer = 0;
        waveTimer = 0;
        tempLevel = 0;
        waveNum = 1;
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        gameUI = GameObject.Find("Canvas").GetComponent<GameUI>();
        audioManager = GameObject.Find("GameSettings").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {

        dayNightTimer += Time.deltaTime;
        if(day && dayNightTimer >= dayLength)
        {
            day = false;
            dayNightTimer = 0;
            StartCoroutine(fadeNight());
            waveTimer = 0;
            waveNum = 1;
            tempLevel += 1; //colder at night
            audioManager.StopMusic();
            audioManager.PlaySFX("TimeChange_ToNight");
            StartCoroutine(playMusicDelayed("OST_Night1"));
            
        }
        if(!day && dayNightTimer >= nightLength)
        {
            day = true;
            dayNightTimer = 0;
            StartCoroutine(fadeDay());
            waveTimer = 0;
            waveNum = 1;
            dayCount += 1;
            if(dayCount < dayTemperatures.Length)
            {
                tempLevel = dayTemperatures[dayCount];
            }
            else
            {
                tempLevel -= 1;
            }
            audioManager.StopMusic();
            audioManager.PlaySFX("TimeChange_ToDay");
            StartCoroutine(playMusicDelayed("OST_Title"));
            

        }
        if (!day)
        {
            waveTimer += Time.deltaTime;
            gameUI.SetTimeUI((1 - (waveTimer % waveInterval) / waveInterval) * 100);

            //enemies
            if (waveTimer >= waveNum * waveInterval)
            {
                spawnWave();
                waveTimer = 0;
            }
        }


        //temperature decrease
        /*
        if (tempLevel < tempChangeTimings.Length)
        {
            if (timer >= tempChangeTimings[tempLevel])
            {
                tempDecrease();
            }
        }
        */

        //freeze (or thaw) the player
        if(playerController.clothesUpgraded && (tempLevel - playerController.GetHeat()) > 0)
        {
            playerController.ChangeFreeze((tempLevel - playerController.GetHeat()) * Time.deltaTime * freezeMultiplier * 0.33f);
        }
        else
        {
            playerController.ChangeFreeze((tempLevel - playerController.GetHeat()) * Time.deltaTime * freezeMultiplier);
        }
        //check freeze damage after freezing player
        if(playerController.GetFreeze() >= 100)
        {
            freezeTimer += Time.deltaTime;
            if (freezeTimer >= freezeDamageInterval)
            {
                playerController.ChangeHealth(-1);
                freezeTimer = 0;
            }
        }
    }

    
    private IEnumerator playMusicDelayed(string song)
    {
        yield return new WaitForSeconds(5);
        audioManager.PlayMusic(song);
    }


    private void tempChange(int toChange)
    {
        //TODO: send out a signal that the temperature decreased
        tempLevel += toChange; //note - templevel increasing means it's colder now
        Debug.Log("it's colder (or warmer) now!");
    }

    public float getTime()
    {
        return timer;
    }

    public float getTemp()
    {
        return tempLevel;
    }

    public void spawnWave()
    {

        int enemyNum = Random.Range(3, 5);
        Vector3 playerPosition = playerController.transform.position;
        for (int i = 0; i < enemyNum; i++)
        {
            Debug.Log("spawning aggro");
            Vector3 offset = new Vector3(Mathf.Cos(Random.Range(-Mathf.PI, Mathf.PI)), Mathf.Sin(Random.Range(-Mathf.PI, Mathf.PI))) * Random.Range(30, 45);
            Instantiate(basicEnemy, playerPosition + offset, basicEnemy.transform.rotation);
        }
        enemyNum = Random.Range(3, 8);
        for (int i = 0; i < enemyNum; i++)
        {
            Vector3 offset = new Vector3(Mathf.Cos(Random.Range(-Mathf.PI, Mathf.PI)), Mathf.Sin(Random.Range(-Mathf.PI, Mathf.PI))) * Random.Range(70, 120);
            Instantiate(basicEnemyWander, playerPosition + offset, basicEnemyWander.transform.rotation);
        }
        //Debug.Log("this would be a wave spawn");
        waveNum += 1;
    }

    IEnumerator fadeNight()
    {
        while (globalLight.intensity > nightLight)
        {
            globalLight.intensity -= 0.001f;
            yield return new WaitForEndOfFrame();
        }
        
    }

    IEnumerator fadeDay()
    {
        while (globalLight.intensity < dayLight)
        {
            globalLight.intensity += 0.001f;
            yield return new WaitForEndOfFrame();
        }
    }

    public bool IsGameOver() { return isGameOver; }
    public void SetGameOver(bool b) { if (!disableGameOver) isGameOver = b; }
}
