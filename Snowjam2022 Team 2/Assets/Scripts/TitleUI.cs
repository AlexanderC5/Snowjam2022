using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles loading/transitioning scenes, along with Main Menu operations
/// </summary>
public class TitleUI : MonoBehaviour
{
    enum Screen
    {
        Title,
        Play,
        Help,
        Credits,
        Settings,
        Quit
    }

    // Variables
    [SerializeField] private string gameplaySceneName;
    [SerializeField] private GameObject fadeTransition;
    private Animator fadeAnimator;
    private Settings settings;

    private Screen selectedMenu;
    private List<Screen> previousMenus = new List<Screen>();
    [SerializeField] GameObject[] menus;
    private GameObject helpMenu;
    private GameObject creditsMenu;
    private GameObject settingsMenu;

    void Start()
    {
        fadeAnimator = fadeTransition.GetComponent<Animator>();
        settings = GameObject.FindGameObjectWithTag("Settings").GetComponent<Settings>();

        StartCoroutine(FadeButtonPressed("in"));
        previousMenus.Add(selectedMenu);

        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].SetActive(false);
            switch(menus[i].name)
            {
                case "Help":
                    helpMenu = menus[i];
                    break;
                case "Credits":
                    creditsMenu = menus[i];
                    break;
                case "Settings":
                    settingsMenu = menus[i];
                    break;
            }
        }
    }

    void Update()
    {
        fadeAnimator.speed = settings.animationSpeed;
    }

    public void Title()
    {
        previousMenus.Add(selectedMenu);
        selectedMenu = Screen.Title;
        StartCoroutine(FadeButtonPressed("both"));
    }

    public void Play()
    {
        previousMenus.Clear();
        selectedMenu = Screen.Play;
        StartCoroutine(FadeButtonPressed("both"));
    }

    public void Help()
    {
        previousMenus.Add(selectedMenu);
        selectedMenu = Screen.Help;
        StartCoroutine(FadeButtonPressed("both"));
    }
    public void Credits()
    {
        previousMenus.Add(selectedMenu);
        selectedMenu = Screen.Credits;
        StartCoroutine(FadeButtonPressed("both"));
    }
    public void Settings()
    {
        previousMenus.Add(selectedMenu);
        selectedMenu = Screen.Settings;
        StartCoroutine(FadeButtonPressed("both"));
    }
    public void Quit()
    {
        previousMenus.Clear();
        selectedMenu = Screen.Quit;
        StartCoroutine(FadeButtonPressed("both"));
    }

    public void Back() // Go back to the Title screen from the UI
    {
        selectedMenu = previousMenus[previousMenus.Count - 1];
        previousMenus.RemoveAt(previousMenus.Count - 1);

        StartCoroutine(FadeButtonPressed("both"));
    }

    IEnumerator FadeButtonPressed(string whichFade)
    {
        fadeTransition.SetActive(true);

        if (whichFade == "out" || whichFade == "both")
        {
            fadeAnimator.Play("FadeOut");
            yield return new WaitForSeconds(1f / settings.animationSpeed);

            switch (selectedMenu)
            {
                case Screen.Title:
                    // Close UI Interfaces
                    for (int i = 0; i < menus.Length; i++)
                    {
                        menus[i].SetActive(false);
                    }
                    break;
                case Screen.Play:
                    SceneManager.LoadScene(gameplaySceneName);
                    break;
                case Screen.Help:
                    helpMenu.SetActive(true);
                    break;
                case Screen.Credits:
                    creditsMenu.SetActive(true);
                    break;
                case Screen.Settings:
                    settingsMenu.SetActive(true);
                    break;
                case Screen.Quit:
                    Debug.Log("Game Quit by user");
                    Application.Quit();
                    StopAllCoroutines();
                    break;
            }
        }
        if (whichFade == "in" || whichFade == "both")
        {
            fadeAnimator.Play("FadeIn");
            yield return new WaitForSeconds(1f / settings.animationSpeed);
        }

        fadeTransition.SetActive(false);
    }
}