using System;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
    where T : Singleton<T>
{
    #region Fields

    private static T _instance;

#if UNITY_EDITOR
    // ReSharper disable once StaticMemberInGenericType
    private static int _destroyedFrameCount = -1;
#endif

    #endregion

    #region Properties

    public static bool hasInstance => _instance != null
#if UNITY_EDITOR
        || !Application.isPlaying && FindObjectOfType<T>()
#endif
        ;

    public static T instance
    {
        get
        {
            if (_instance != null) return _instance;

            var singletonOptionsAttributes = typeof(T).GetCustomAttributes(typeof(SingletonOptionsAttribute), true);
            var singletonOptionsAttribute = (SingletonOptionsAttribute)(singletonOptionsAttributes.Length > 0 ? singletonOptionsAttributes[0] : null);

            var name = !string.IsNullOrEmpty(singletonOptionsAttribute?.name) ? singletonOptionsAttribute.name.ToUpper() : typeof(T).Name.ToUpper();
            if (singletonOptionsAttribute is { isPrefab: true })
            {
                name = $"Prefabs/[{name}]";
            }

            var asset = FindObjectOfType<T>();
            if (asset != null)
            {
                if (Application.isPlaying)
                {
                    asset.Awake();
                }
                else
                {
                    _instance = asset;
                }
                
                return _instance;
            }

            GameObject go;
            if (singletonOptionsAttribute is { isPrefab: true })
            {
#if UNITY_EDITOR
                asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>($"Assets/Resources/{name}.prefab");
                go = asset.gameObject;
#else
                go = (GameObject)Resources.Load(name);
                asset = go.GetComponent<T>();
#endif
                if (asset.dontInstantiate)
                {
                    asset.Awake();
                }
                else asset = Instantiate(go).GetComponent<T>();

                _instance = asset;
            }
            else
            {
                go = new GameObject($"[{name.ToUpperInvariant()}]");

                _instance = go.GetComponent<T>();

                if (_instance == null)
                    _instance = go.AddComponent<T>();
            }

            return _instance;
        }
    }

    public virtual bool useDontDestroyOnLoad => true;

    public virtual bool dontInstantiate => false;

    protected bool isNotTheSingletonInstance => _instance != null && _instance != this;

    #endregion

    #region Protected Methods

    /// <summary>
    /// Method called at the end of Awake, made for users to add some code while not breaking Singleton.Awake function.
    /// </summary>
    protected virtual void OnAwake() { }

    #endregion

    #region Unity Event Functions

    protected void Awake()
    {
        
        // For [ExecuteInEditMode] objects
        if (!Application.isPlaying) return;

        if (_instance != null)
        {
            if (_instance != this)
            {
                DestroyImmediate(gameObject);
            }
            
            return;
        }

        _instance = (T)this;
        
#if UNITY_EDITOR
        if (_destroyedFrameCount == Time.frameCount)
        {
            Debug.LogWarningFormat(this, "Singleton '{0}' destroyed and instantiated at the same frame. It might be a cleanup issue. Check the callstack.", typeof(T));
        }
#endif

        if (useDontDestroyOnLoad && !dontInstantiate) DontDestroyOnLoad(gameObject);

        OnAwake();
    }

    protected virtual void OnDestroy()
    {
        if (_instance != this) return;
        _instance = null;
#if UNITY_EDITOR
        _destroyedFrameCount = Time.frameCount;
#endif
    }

    #endregion
}

[AttributeUsage(AttributeTargets.Class)]
public class SingletonOptionsAttribute : Attribute
{
    public SingletonOptionsAttribute(string name, bool isPrefab = false)
    {
        this.name = name;
        this.isPrefab = isPrefab;
    }

    public string name { get; }

    public bool isPrefab { get; }
}