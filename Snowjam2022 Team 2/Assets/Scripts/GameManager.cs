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

    [SerializeField]
    private float freezeMultiplier;
    [SerializeField]
    private float freezeDamageInterval;
    private float freezeTimer;



    //lights
    [SerializeField] private Light2D globalLight;
    private float nightLight = 0.02f;
    private float dayLight = 0.6f;


    private PlayerController playerController;
    private GameUI gameUI; // Get sceneUI

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
            
        }
        if (!day)
        {
            waveTimer += Time.deltaTime;
            gameUI.SetTimeUI((1 - (waveTimer % waveInterval) / waveInterval) * 100);

            //enemies
            if (waveTimer >= waveNum * waveInterval)
            {
                spawnWave();
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
        playerController.ChangeFreeze((tempLevel - playerController.GetHeat()) * Time.deltaTime * freezeMultiplier);

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
        Debug.Log("this would be a wave spawn");
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
}
