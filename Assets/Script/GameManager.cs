using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool isGameActive = true;
    public TextMeshProUGUI currentScoreText;
    public GameObject tutorialObject;

    void Start()
    {
        if (currentScoreText != null) {
            currentScoreText.text = "CURRENT SCORE : " + FindFirstObjectByType<GameData>().score;
        }
        Invoke("DestroyTutorialObject", 5f);
    }

    private void DestroyTutorialObject()
    {
        if (tutorialObject != null)
        {
            Destroy(tutorialObject);
        }
    }

    public void playGame () {
        SceneManager.LoadSceneAsync(1);
    } 

    public void playAgain () {
        SceneManager.LoadScene("Main Menu");
    } 

    public void loadLevel2() {
        SceneManager.LoadScene("Level 2");
    }

    public void loadLevel3() {
        SceneManager.LoadScene("Level 3");
    }

    public void loadHeaven() {
        SceneManager.LoadScene("Heaven");
    }

    public void backToMainMenu () {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadSceneAsync(0);
    } 

    public void quitGame () {
        Application.Quit();
    }

    public void pause () {
        AudioListener.pause = true; // Optional: pause audio
        SceneManager.LoadScene("Pause", LoadSceneMode.Additive);
        Time.timeScale = 0f;
    }

    public void resumeGame () {
        SceneManager.UnloadSceneAsync("Pause");
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}
