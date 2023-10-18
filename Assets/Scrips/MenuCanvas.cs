using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCanvas : MonoBehaviour
{
    public void SwitchOffGame()
    {
        Application.Quit();


#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


    public void StartGame()
    {
        SceneManager.LoadScene("TerrainMap");
    }
}
