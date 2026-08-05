using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private Button startButton;
    private Button optionsButton;
    private Button quitButton;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        startButton = root.Q<Button>("startButton");
        quitButton = root.Q<Button>("quitButton");

        startButton.clicked += StartGame;
        quitButton.clicked += QuitGame;
    }

    void StartGame()
    {
        SceneManager.LoadScene("Nivel Principal");
    }

    void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit game");
    }
}