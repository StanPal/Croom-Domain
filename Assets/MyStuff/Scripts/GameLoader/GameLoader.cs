using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : AsyncLoader
{
    [SerializeField] private GameObject _spawnManager = null;
    [SerializeField] private GameObject _actionManager = null;
    [SerializeField] private int _sceneIndexToLoad = 1;
    private static int _sceneIndex = 0;
    private static GameLoader _instance; // The only singleton you should have.

    [SerializeField] private List<Component> _moduleComponents = new List<Component>();

    public static Transform SystemsParent { get { return _systemsParent; } }
    private static Transform _systemsParent;

    protected override void Awake()
    {
        Debug.Log("GameLoader Starting");

        // Saftey check
        if (_instance != null && _instance != this)
        {
            Debug.Log("A duplicate instance of the GameLoader was found, and will be ignored. Only one instance is permitted");
            Destroy(gameObject);
            return;
        }

        // Set reference to this instance
        _instance = this;

        // Make persistent
        DontDestroyOnLoad(gameObject);

        // Scene Index Check
        if (_sceneIndexToLoad < 0 || _sceneIndexToLoad >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"Invalid Scene Index {_sceneIndexToLoad} ... using default value of {_sceneIndex}");
        }
        //else
        //{
        //    _sceneIndex = _sceneIndexToLoad;
        //}

        // Setup System GameObject
        GameObject systemsGO = new GameObject("[Systems]");
        _systemsParent = systemsGO.transform;
        DontDestroyOnLoad(systemsGO);

        // Queue up loading routines
        Enqueue(IntializeCoreSystems(), 2);
        Enqueue(InitializeModularSystems(), 2);

        // Set completion callback
        CallOnComplete(OnComplete);
    }

    private IEnumerator IntializeCoreSystems()
    {
        // Setup Core Systems
        Debug.Log("Loading Core Systems");

        Instantiate(_spawnManager, SystemsParent);
        Instantiate(_actionManager, SystemsParent);


        yield return null;
    }

    private IEnumerator InitializeModularSystems()
    {
        // Setup Additional Systems as needed
        Debug.Log("Loading Modular Systems");

        foreach (var comp in _moduleComponents)
        {
            if (comp is IGameModule)
            {
                var module = comp as IGameModule;
                yield return module.LoadModule();
            }
        }

        yield return null;
    }

    private void OnComplete()
    {
        Debug.Log("GameLoader Completed");
        StartCoroutine(LoadInitialScene(_sceneIndex));
    }

    private IEnumerator LoadInitialScene(int index)
    {
        Debug.Log("GameLoader Starting Scene Load");
        yield return SceneManager.LoadSceneAsync(index);
    }
}