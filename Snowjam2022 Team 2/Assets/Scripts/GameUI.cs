using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles loading/transitioning scenes, along with Main Menu operations
/// </summary>
public class GameUI : MonoBehaviour
{
    enum Screen // Destination menu/scene that can be activated via buttons
    {
        Play,
        Title,
        Settings,
        Inventory
    }

    // Variables
    private Settings settings; // Global settings

    [SerializeField] private string titleSceneName; // For scene loading

    [SerializeField] private GameObject fadeTransition;
    private Animator fadeAnimator;
    [SerializeField] private GameObject menuTransition; // Animation to slide up from the bottom of the screen
    private bool currentlyTransitioning = false;
    private Animator menuAnimator;
    
    private Screen selectedMenu;
    [SerializeField] GameObject[] menus; // Stores settings and inventory menus
    private GameObject settingsMenu;
    private GameObject inventory;

    [SerializeField] private GameObject healthMask;
    [SerializeField] private float[] healthMaskRange = new float[2]; // Trial-and-error range to make the health bars look right
    [SerializeField] private GameObject temperatureMask;
    [SerializeField] private float[] temperatureMaskRange = new float[2];
    [SerializeField] private GameObject timeMask;
    [SerializeField] private float[] timeMaskRange = new float[2];

    void Start()
    {
        // Attach the main camera to the canvas to view all of the UI stuff
        try { gameObject.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>(); }
        catch { Debug.Log("A GameObject tagged 'MainCamera' with a Camera component is required to view the canvas animations!"); }
        
        // Locate the animators and settings Game Objects
        fadeAnimator = fadeTransition.GetComponent<Animator>();
        menuAnimator = menuTransition.GetComponent<Animator>();
        settings = GameObject.FindGameObjectWithTag("Settings").GetComponent<Settings>();

        // Add the menus from the inspector fields if they are valid
        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].SetActive(false);
            switch(menus[i].name)
            {
                case "Settings":
                    settingsMenu = menus[i];
                    break;
                case "Inventory":
                    inventory = menus[i];
                    break;
            }
        }

        // Start the scene by fading from black
        fadeAnimator.Play("FadeOut");
        fadeTransition.SetActive(false);
    }

    // Check for keyboard shortcuts every frame in Update()
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !currentlyTransitioning)
        {
            if (selectedMenu == Screen.Play) Inventory();
            else if (selectedMenu == Screen.Inventory) Back();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !currentlyTransitioning)
        {
            if (selectedMenu == Screen.Play) Settings();
            else if (selectedMenu == Screen.Inventory) Back();
            else if (selectedMenu == Screen.Settings) Back();
        }
    }

    // Set the values of UI Bars, ranging from 0-100%
    public void SetHealthUI(float percent) { healthMask.GetComponent<RectMask2D>().padding = new Vector4(healthMask.GetComponent<RectMask2D>().padding.x, healthMask.GetComponent<RectMask2D>().padding.y, percent / 100f * (healthMaskRange[1] - healthMaskRange[0]) + healthMaskRange[0], healthMask.GetComponent<RectMask2D>().padding.w); }
    public void SetTemperatureUI(float percent) { temperatureMask.GetComponent<RectMask2D>().padding = new Vector4(temperatureMask.GetComponent<RectMask2D>().padding.x, temperatureMask.GetComponent<RectMask2D>().padding.y, percent / 100f * (temperatureMaskRange[1] - temperatureMaskRange[0]) + temperatureMaskRange[0], temperatureMask.GetComponent<RectMask2D>().padding.w); }
    public void SetTimeUI(float percent) { timeMask.GetComponent<RectMask2D>().padding = new Vector4(timeMask.GetComponent<RectMask2D>().padding.x, timeMask.GetComponent<RectMask2D>().padding.y, percent / 100f * (timeMaskRange[1] - timeMaskRange[0]) + timeMaskRange[0], timeMask.GetComponent<RectMask2D>().padding.w); }
    
    // Select a different menu/scene
    public void Title() { selectedMenu = Screen.Title; StartCoroutine(Transition()); }
    public void Settings() { selectedMenu = Screen.Settings; StartCoroutine(Transition()); }
    public void Inventory() { selectedMenu = Screen.Inventory; StartCoroutine(Transition()); }
    public void Back() { selectedMenu = Screen.Play; StartCoroutine(Transition()); }

    // Animated Transition between scenes/menus
    IEnumerator Transition()
    {
        fadeAnimator.speed = settings.animationSpeed;
        menuAnimator.speed = settings.animationSpeed;

        currentlyTransitioning = true;
        switch (selectedMenu)
        {
            case Screen.Title:
                fadeTransition.SetActive(true);
                fadeAnimator.Play("FadeOut");
                yield return new WaitForSeconds(1f / settings.animationSpeed);
                SceneManager.LoadScene(titleSceneName);
                break;

            case Screen.Play:
                // Close UI Interfaces
                menuAnimator.Play("MenuHide");
                yield return new WaitForSeconds(1f / settings.animationSpeed);
                for (int i = 0; i < menus.Length; i++) { menus[i].SetActive(false); }
                break;

            case Screen.Settings:
                if (!inventory.activeSelf)
                {
                    settingsMenu.SetActive(true);
                    menuAnimator.Play("MenuShow");
                    yield return new WaitForSeconds(1f / settings.animationSpeed);
                }
                break;

            case Screen.Inventory:
                if (!settingsMenu.activeSelf)
                {
                    inventory.SetActive(true);
                    menuAnimator.Play("MenuShow");
                    yield return new WaitForSeconds(1f / settings.animationSpeed);
                }
                break;

        }
        currentlyTransitioning = false;
    }

    // Functions to change the global settings
    public void SetAnimationSpeed(float spd) { settings.SetAnimationSpeed(spd); }
    public void SetMasterVolume(float vol) { settings.SetMasterVolume(vol); }
    public void SetMusVolume(float vol) { settings.SetMusVolume(vol); }
    public void SetSfxVolume(float vol) { settings.SetSfxVolume(vol); }
}