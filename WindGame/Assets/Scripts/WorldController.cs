﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
    The goal of this class is to keep track of all objects in the scene. 
    It should contain functions for adding and removing new objects as well as
    maintaining a list of all relevant objects.
**/

public class WorldController : MonoBehaviour
{
    TurbineManager turbManager; // Holder of turbines

    private static WorldController instance;

    [Header("Prefabs")]
    public GameObject weatherManagerPrefab;
    public GameObject terrainManagerPrefab;
    public GameObject turbineManagerPrefab;
    public GameObject buildingsManagerPrefab;
    public GameObject worldInteractionManagerPrefab;

    // Use this for initialization
    void Awake()
    {
        CreateSingleton();
        InstantiateStartPrefabs();
    }

    void Start()
    {
        turbManager = TurbineManager.GetInstance();
    }


    void Update()
    {
        float dt = Time.deltaTime * GameResources.getGameSpeed();
        turbManager.Update(dt);
    }

    // Create the singletone for the WorldManager. Also checks if there is another present and logs and error.
    void CreateSingleton()
    {
        if (instance != null)
        {
            Debug.LogError("WorldManager already exists while it should be instantiated only once.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Instantiate the starting prefabs as the children of the WorldManager
    void InstantiateStartPrefabs()
    {
        GameObject obj = Instantiate(terrainManagerPrefab);
        obj.transform.SetParent(transform);
        obj = Instantiate(weatherManagerPrefab);
        obj.transform.SetParent(transform);
        obj = Instantiate(turbineManagerPrefab);
        obj.transform.SetParent(transform);
        obj = Instantiate(buildingsManagerPrefab);
        obj.transform.SetParent(transform);
        obj = Instantiate(worldInteractionManagerPrefab);
        obj.transform.SetParent(transform);
    }

    // Get the singleton instance
    public static WorldController GetInstance()
    {
        return instance;
    }


    // Builder function, some class wants the world to add an object
    public void Add(GameObject something, Vector3 pos, Quaternion rotation, float scale, GridTileOccupant.OccupantType type, float size, Transform parent)
    {
        GameObject t = (GameObject)Instantiate(something, pos, rotation, parent);
        t.transform.localScale = Vector3.one * scale;
        t.tag = "turbine";

        AddToGridTiles(something, pos, size/2, type);

        if (type == GridTileOccupant.OccupantType.Turbine)
        {
            TurbineManager turbManager = TurbineManager.GetInstance();
            turbManager.AddTurbine(t); 
        }
    }

    // Function that determines if a tile has a object on it and return true if there is no objects on all the tiles in a circle with size as diameter.
    public bool CanBuild(Vector3 pos, float size, bool neglectTerrainObjects)
    {
        GridTile[] gridtiles = GridTile.FindGridTilesAround(pos, size/2);

        foreach (GridTile tile in gridtiles)
        {
            if (neglectTerrainObjects && tile.occupant != null && (tile.type == GridTileOccupant.OccupantType.Turbine || tile.type == GridTileOccupant.OccupantType.City))
                return false;
            else if (!neglectTerrainObjects && tile.occupant != null && (tile.type == GridTileOccupant.OccupantType.Turbine || tile.type == GridTileOccupant.OccupantType.City || tile.type == GridTileOccupant.OccupantType.TerrainGenerated))
                return false;
        }
        return true;
    }

    // Function that adds on object to all gridtiles in a certian circle radius around a tile with position point.
    void AddToGridTiles(GameObject something, Vector3 point, float circleRadius, GridTileOccupant.OccupantType type)
    {
        GridTile[] gridtiles = GridTile.FindGridTilesAround(point, circleRadius);

        foreach (GridTile tile in gridtiles)
        {
            if (tile.type == GridTileOccupant.OccupantType.TerrainGenerated)
                TerrainController.thisTerrainController.RemoveTerrainTileOccupant(tile);

            tile.occupant = new GridTileOccupant(something);
            tile.type = type;
        }
    }
}
