using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseScreens : MonoBehaviour
{
    [SerializeField] GameObject winScreen, loseScreen;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) //Fire1 is mouse 1st click
        {
            ShowWin();
        }
        if (Input.GetKeyDown(KeyCode.P)) //Fire1 is mouse 1st click
        {
            ShowLose();
        }
    }
    public void ShowLose()
    {
        loseScreen.SetActive(true);
    }

    public void ShowWin()
    {
        winScreen.SetActive(true);
    }

    public void Lobby()
    {
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}