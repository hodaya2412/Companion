using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldItemRegistry : MonoBehaviour
{
    public static WorldItemRegistry Instance;

    
    private Dictionary<string, WorldItem> items = new Dictionary<string, WorldItem>();

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        
        RegisterAllWorldItemsInScene();

        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        RegisterAllWorldItemsInScene();
    }

    
    public void RegisterAllWorldItemsInScene()
    {
        var worldItems = FindObjectsByType<WorldItem>(FindObjectsSortMode.None);
        foreach (var item in worldItems)
        {
            Register(item);
        }
    }

    public void Register(WorldItem item)
    {
        if (item == null || string.IsNullOrEmpty(item.worldItemId)) return;

        if (!items.ContainsKey(item.worldItemId))
            items.Add(item.worldItemId, item);
        else
            items[item.worldItemId] = item; 
    }

  
    public WorldItem Get(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        if (items.TryGetValue(id, out var item))
            return item;

        return null;
    }

  
    public void Unregister(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        if (items.ContainsKey(id))
            items.Remove(id);
    }
}