using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{

    public Canvas exitMenu;
    public Button playButton;
    public Button exitButton;
    public Text title;

    void Start()
    {
        exitMenu = exitMenu.GetComponent<Canvas>();
        playButton = playButton.GetComponent<Button>();
        exitButton = exitButton.GetComponent<Button>();
        title = title.GetComponent<Text>();

        title.text = "<color=red>P</color> <color=blue>K</color><color=yellow>A</color><color=green>O</color> <color=red>P</color><color=blue>O</color><color=yellow>J</color><color=green>A</color><color=red>M</color>";

        exitMenu.enabled = false;
    }

	public void ChangeToScene(string scene)
    {
        SceneManager.LoadScene(scene);
	}

    public void ExitPress()
    {
        exitMenu.enabled = true;
        playButton.enabled = false;
        exitButton.enabled = false;
    }

    public void NoPress()
    {
        exitMenu.enabled = false;
        playButton.enabled = true;
        exitButton.enabled = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
