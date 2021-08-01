using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// William de Beer
/// </summary>
public class LevelLoader : MonoBehaviour
{
    #region Singleton

    private static LevelLoader _instance = null;
    public static LevelLoader instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject loader = new GameObject();
                _instance = loader.AddComponent<LevelLoader>();
                return loader.GetComponent<LevelLoader>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        if (_instance == this)
        {
            InitialiseFunc();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Second Instance of LevelLoader was created, this instance was destroyed.");
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    #endregion

    public static bool cheatsEnabled = false;
    public static bool loadingNextArea = false;

    public GameObject CompleteLoadUI;

    public Animator transition;

    public bool isTransitioning = false;
    public float transitionTime = 1.0f;

    private void InitialiseFunc()
    {


    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        loadingNextArea = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    private void OnLevelWasLoaded(int level)
    {
        GetComponentInChildren<Animator>().SetTrigger("Blink");
    }

    public void LoadNextLevel()
    {
        loadingNextArea = true;
        if (SceneManager.sceneCountInBuildSettings <= SceneManager.GetActiveScene().buildIndex + 1) // Check if index exceeds scene count
        {
            StartCoroutine(LoadLevel(0)); // Load menu
        }
        else
        {
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1)); // Loade next scene
        }
    }
    public void LoadNewLevel(string _name)
    {
        if (!isTransitioning)
            StartCoroutine(LoadLevel(SceneManager.GetSceneByName(_name).buildIndex));
    }
    public void ResetScene()
    {
        loadingNextArea = true;
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
    }

    public void LoadLevelAsync(int levelIndex, float maxTime)
    {
        //StartCoroutine(OperationLoadLevelAsync(levelIndex, maxTime));
    }

    //IEnumerator OperationLoadLevelAsync(int levelIndex, float maxTime)
    //{
    //    AsyncOperation gameLoad = SceneManager.LoadSceneAsync(levelIndex);
    //    gameLoad.allowSceneActivation = false;
    //    float time = 0.0f;

    //    while (!gameLoad.isDone)
    //    {
    //        time += Time.deltaTime;
    //        if (gameLoad.progress >= 0.9f)
    //        {
    //            CompleteLoadUI.SetActive(true);

    //            if (InputManager.GetInstance().GetKeyDown(InputManager.ButtonType.BUTTON_SOUTH, 0))
    //            {
    //                gameLoad.allowSceneActivation = true;
    //            }
    //            if (InputManager.GetInstance().GetKeyDown(InputManager.ButtonType.BUTTON_SOUTH, 1))
    //            {
    //                gameLoad.allowSceneActivation = true;
    //            }
    //            if (time >= maxTime)
    //            {
    //                gameLoad.allowSceneActivation = true;
    //            }
    //        }
    //        yield return new WaitForEndOfFrame();
    //    }

    //    CompleteLoadUI.SetActive(false);
    //    yield return null;
    //}

    IEnumerator LoadLevel(int levelIndex)
    {
        SaveSceneToSlot(GameManager.instance.m_saveSlot);

        isTransitioning = true;
        if (transition != null)
        {
            // Play Animation
            transition.SetTrigger("Start");

            // Wait to let animation finish playing
            yield return new WaitForSeconds(transitionTime);
        }
        isTransitioning = false;
        // Load Scene
        SceneManager.LoadScene(levelIndex);
        yield return new WaitForSeconds(transitionTime);
    }

    private void SaveSceneToSlot(SaveSlot slot)
    {
        slot.SaveObjects(GameObject.FindGameObjectsWithTag("SerializedObject"));
    }
}
