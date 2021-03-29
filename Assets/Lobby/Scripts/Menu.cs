using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] public GameObject menu;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Debug.Log("MENU PRESSED");
            menu.SetActive(true);
        }
    }

    public void Controls()
    {
        Debug.Log("CONTROLS PRESSED");
    }

    public void Close()
    {
        menu.SetActive(false);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
