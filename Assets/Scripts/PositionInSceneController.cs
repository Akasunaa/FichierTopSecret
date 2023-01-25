using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;

[SingletonOptions(name: "PositionInSceneController")]
public class PositionInSceneController : Singleton<PositionInSceneController>
{

    /// <summary>
    /// Made to watch the value we got from _locToPos when the level is loaded.
    /// </summary>
    [SerializeField] private Vector3 newPos;

    #region Fields

    /// <summary>
    /// Structure containing the position of the player in the levels at the time he left them.
    /// </summary>
    private readonly Dictionary<string, Vector3> _locToPos = new();

    /// <summary>
    /// A reference of the player GameObject.
    /// </summary>
    private GameObject _player;

    #endregion
    
    #region Protected Methods
    
    protected override void OnAwake()
    {
        base.OnAwake();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    protected override void OnDestruction()
    {
        base.OnDestruction();
        _locToPos.Clear();
    }

    #endregion
    
    #region Private Methods

    /// <summary>
    /// Called when the scene is unloaded by the engine.
    /// </summary>
    /// <param name="arg0"></param>
    private void OnSceneUnloaded(Scene arg0)
    {
        _player = null;
    }
    
    /// <summary>
    /// Called when a scene is loaded by the engine.
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="loadSceneMode"></param>
    private void OnSceneLoaded(Scene arg0, LoadSceneMode loadSceneMode)
    {
        // get player reference
        _player = GameObject.FindGameObjectWithTag("Player");

        // check if the position was stocked, exits the function if not
        if (!GetLevelPosition(SceneManager.GetActiveScene().name, out newPos)) return;
        
        // else if it exists just affect it to the player
        _player.transform.position = newPos;
    }

    #endregion

    #region Public Methods
    
    /// <summary>
    /// Method to call just before the engine changed scene, in order to store the player's position in the currently
    /// loaded scene.
    /// </summary>
    public void OnPlayerExitedLevel()
    {
        var levelName = SceneManager.GetActiveScene().name; 
        AddLevelPosition(levelName, _player.transform.position);
    }

    /// <summary>
    /// Get the position where the player should spawn in the designated level, if you have set it earlier.
    /// </summary>
    /// <param name="levelName"> The level name.</param>
    /// <param name="newPlayerPosition"></param>
    /// <returns> A Vector3 containing the player's spawn position if found, otherwise a Vector3 which z = -1000. </returns>
    public bool GetLevelPosition(string levelName, out Vector3 newPlayerPosition)
    {
        return _locToPos.TryGetValue(levelName.Trim().ToLower(), out newPlayerPosition);
    }

    /// <summary>
    /// Store the position at the levelName key.
    /// </summary>
    /// <param name="levelName"> The level name associated with the player's position you want to store. </param>
    /// <param name="position"> The position you want to store with that level name. </param>
    public void AddLevelPosition(string levelName, Vector3 position)
    {
        levelName = levelName.Trim().ToLower();
        if (_locToPos.TryAdd(levelName, position)) return;
        _locToPos[levelName] = position;
    }
    
    #endregion
    
}

