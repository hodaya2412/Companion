using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldItemRegistry : MonoBehaviour
{
    public static WorldItemRegistry Instance;

    // כל הפריטים הרשומים לפי ID
    private Dictionary<string, WorldItem> items = new Dictionary<string, WorldItem>();

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // רישום פריטים קיימים בסצנה הנוכחית
        RegisterAllWorldItemsInScene();

        // מאזין למעבר סצנות
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // רישום פריטים חדשים בסצנה נטענת
        RegisterAllWorldItemsInScene();
    }

    /// <summary>
    /// רישום כל WorldItems בסצנה הנוכחית
    /// </summary>
    public void RegisterAllWorldItemsInScene()
    {
        var worldItems = FindObjectsByType<WorldItem>(FindObjectsSortMode.None);
        foreach (var item in worldItems)
        {
            Register(item);
        }
    }

    /// <summary>
    /// רישום פריט יחיד (ניתן לקרוא גם מדינמיקה)
    /// </summary>
    public void Register(WorldItem item)
    {
        if (item == null || string.IsNullOrEmpty(item.worldItemId)) return;

        if (!items.ContainsKey(item.worldItemId))
            items.Add(item.worldItemId, item);
        else
            items[item.worldItemId] = item; // עדכון אם כבר קיים
    }

    /// <summary>
    /// קבלת פריט לפי ID
    /// </summary>
    public WorldItem Get(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        if (items.TryGetValue(id, out var item))
            return item;

        return null;
    }

    /// <summary>
    /// הסרת פריט מהרשימה (למשל אחרי שקיבלנו אותו לאינוונטורי)
    /// </summary>
    public void Unregister(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        if (items.ContainsKey(id))
            items.Remove(id);
    }
}