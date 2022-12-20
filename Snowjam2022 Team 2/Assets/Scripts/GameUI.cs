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
        Inventory,
        GameOver,
        Restart
    }

    // Variables
    private Settings settings; // Global settings
    private GameManager gameManager; // Game manager to check game over
    private AudioManager audioManager;

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
    public bool isMenuOpen {get; private set;}

    [SerializeField] private GameObject gameOverScreen;

    [SerializeField] private GameObject healthMask;
    [SerializeField] private float[] healthMaskRange = new float[2]; // Trial-and-error range to make the health bars look right
    [SerializeField] private GameObject temperatureMask;
    [SerializeField] private float[] temperatureMaskRange = new float[2];
    private GameObject timeUIBar;
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
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        audioManager = AudioManager.manager; 
        isMenuOpen = false;

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

        gameOverScreen.SetActive(false);
        timeUIBar = timeMask.gameObject.transform.parent.gameObject;

        // Start the scene by fading from black
        fadeAnimator.Play("FadeOut");
        fadeTransition.SetActive(false);

        // Initialize volume/animation slider listeners
        Slider[] sliders = Resources.FindObjectsOfTypeAll<Slider>();
        foreach (Slider slider in sliders)
        {
            if (slider.transform.parent.parent.name != "Settings") continue;
            slider.onValueChanged.RemoveAllListeners();
            switch (slider.transform.parent.name)
            {
                case "Option1": //master
                    slider.value = Settings.Instance.volumeMaster;
                    slider.onValueChanged.AddListener(Settings.Instance.SetVolumeMaster);
                    break;
                case "Option2": //music
                    slider.value = Settings.Instance.volumeMusic;
                    slider.onValueChanged.AddListener(Settings.Instance.SetVolumeMusic);
                    break;
                case "Option3": //sfx
                    slider.value = Settings.Instance.volumeSFX;
                    slider.onValueChanged.AddListener(Settings.Instance.SetVolumeSFX);
                    break;
                case "Option4": //animspeed
                    slider.value = Settings.Instance.animationSpeed;
                    slider.onValueChanged.AddListener(Settings.Instance.SetAnimationSpeed);
                    break;
            }
        }
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
            if (selectedMenu == Screen.Play) SettingsMenu();
            else if (selectedMenu == Screen.Inventory) Back();
            else if (selectedMenu == Screen.Settings) Back(); 
        }

        // Check Game over
        if (gameManager.IsGameOver()) { GameOver(); isMenuOpen = true; }

        if (gameManager.GetDay()) { SetTimeUI(false); }
        else { SetTimeUI(true); }
    }

    // Set the values of UI Bars, ranging from 0-100%
    public void SetHealthUI(float percent) { healthMask.GetComponent<RectMask2D>().padding = new Vector4(healthMask.GetComponent<RectMask2D>().padding.x, healthMask.GetComponent<RectMask2D>().padding.y, percent / 100f * (healthMaskRange[1] - healthMaskRange[0]) + healthMaskRange[0], healthMask.GetComponent<RectMask2D>().padding.w); }
    public void SetTemperatureUI(float percent) { temperatureMask.GetComponent<RectMask2D>().padding = new Vector4(temperatureMask.GetComponent<RectMask2D>().padding.x, temperatureMask.GetComponent<RectMask2D>().padding.y, percent / 100f * (temperatureMaskRange[1] - temperatureMaskRange[0]) + temperatureMaskRange[0], temperatureMask.GetComponent<RectMask2D>().padding.w); }
    public void SetTimeUI(float percent) { if (timeUIBar.activeSelf) timeMask.GetComponent<RectMask2D>().padding = new Vector4(timeMask.GetComponent<RectMask2D>().padding.x, timeMask.GetComponent<RectMask2D>().padding.y, percent / 100f * (timeMaskRange[1] - timeMaskRange[0]) + timeMaskRange[0], timeMask.GetComponent<RectMask2D>().padding.w); }

    public void SetTimeUI(bool active) { timeUIBar.SetActive(active); }

    
    // Select a different menu/scene
    public void Title() { 
        selectedMenu = Screen.Title;
        audioManager.PlaySFX("UI_MenuButton");
        isMenuOpen = true;
        StartCoroutine(Transition());
    }
    
    public void SettingsMenu() {
        selectedMenu = Screen.Settings;
        isMenuOpen = true;
        audioManager.PlaySFX("UI_Config");
        StartCoroutine(Transition());
    }
    
    public void Inventory() {
        selectedMenu = Screen.Inventory;
        isMenuOpen = true;
        audioManager.PlaySFX("UI_Inventory");
        StartCoroutine(Transition());
    }
    
    public void Back() {
        selectedMenu = Screen.Play;
        isMenuOpen = false;
        audioManager.PlaySFX("UI_Cancel");
        StartCoroutine(Transition());
    }
    
    public void GameOver() {
        selectedMenu = Screen.GameOver;
        isMenuOpen = true;
        StartCoroutine(Transition());
    }
    
    public void Restart() {
        selectedMenu = Screen.Restart;
        isMenuOpen = false;
        StartCoroutine(Transition());
    }

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
            case Screen.GameOver:
                if (inventory.activeSelf || settingsMenu.activeSelf) { menuAnimator.Play("MenuHide"); }
                gameOverScreen.SetActive(true);
                gameOverScreen.GetComponent<Animator>().Play("GameOver");
                yield return new WaitForSeconds(1f / settings.animationSpeed);
                for (int i = 0; i < menus.Length; i++) { menus[i].SetActive(false); }
                break;
            case Screen.Restart:
                fadeTransition.SetActive(true);
                fadeAnimator.Play("FadeOut");
                yield return new WaitForSeconds(1f / settings.animationSpeed);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
        currentlyTransitioning = false;
    }

    // ALREADY EXISTS IN SETTINGS.CS
    /* Functions to change the global settings
    public void SetAnimationSpeed(float spd) { settings.SetAnimationSpeed(spd); }
    public void SetMasterVolume(float vol) { settings.SetMasterVolume(vol); }
    public void SetMusVolume(float vol) { settings.SetMusVolume(vol); }
    public void SetSfxVolume(float vol) { settings.SetSfxVolume(vol); } */
}