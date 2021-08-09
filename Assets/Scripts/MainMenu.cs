using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGame()
    {
        // Delete current save.
        LevelLoader.instance.LoadNewLevel("SceneChange2");
    }
    public void ContinueGame()
    {
        LevelLoader.instance.LoadNewLevel("SceneChange2");
    }
    public void Settings()
    {

    }
    public void QuitGame()
    {
        LevelLoader.instance.QuitGame();
    }
}
