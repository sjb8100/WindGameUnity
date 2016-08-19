﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIScript : MonoBehaviour {

    private static UIScript instance;

    List<GameObject> menus;

    [Header("Prefabs")]
    public GameObject eventSystemPrefab;
    public GameObject buildMenuPrefab;
    public GameObject resourcesMenuPrefab;
    public GameObject pauseMenuPrefab;
    public GameObject mainMenuPrefab;
    public GameObject radialMenuPrefab;
    public GameObject turorialPrefab;

    int menuActive;

    // Use this for initialization
    void Awake ()
    {
        CreateSingleton();
        menus = new List<GameObject>();
        InstantiateStartPrefabs();
        menuActive = -1;
        
    }

    // Create the singletone for the UIManager. Also checks if there is another present and logs and error.
    void CreateSingleton()
    {
        if (instance != null)
        {
            Debug.LogError("UIManager already exists while it should be instantiated only once.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Instantiate the starting prefabs as the children of the UIScript
    void InstantiateStartPrefabs()
    {
        GameObject obj = Instantiate(eventSystemPrefab);
        obj.transform.SetParent(transform);

        GameObject obj2 = Instantiate(resourcesMenuPrefab);
        obj2.transform.SetParent(transform);
        menus.Add(obj2);

        GameObject obj3 = Instantiate(pauseMenuPrefab);
        obj3.transform.SetParent(transform);
        obj3.SetActive(false);
        menus.Add(obj3);

        GameObject obj4 = Instantiate(buildMenuPrefab);
        obj4.transform.SetParent(transform);
        obj4.SetActive(false);
        menus.Add(obj4);
        
    }

    // Get the singleton instance
    public static UIScript GetInstance()
    {
        return instance;
    }

    public bool menuButtonPress()
    {
        if (menus[1].activeSelf)
        {
            menus[1].SetActive(false);
            GameResources.unPause();
            return true;
        } else
        {
            menus[1].SetActive(true);
            GameResources.pause();
            return false;
        }
    }
}
