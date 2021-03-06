﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GridTile{

    public Vector3 position;
    public float biome;
    public List<GridTileOccupant> occupants;
    public bool underWater;
    public bool isOutsideBorder;
    public bool canSeeWind;
    public List<GridNode> gridNodes;
    public Chunk chunk;
    public List<GridTile> windEffectTiles;

    public GridTile(Vector3 position, Chunk chunk, List<GridNode> gridNodes, float biome, bool isUnderWater, bool isOutsideBorder, List<GridTileOccupant> occupants)
    {
        this.position = position;
        this.biome = biome;
        this.occupants = occupants;
        this.gridNodes = gridNodes;
        this.underWater = isUnderWater;
        this.chunk = chunk;
        this.isOutsideBorder = isOutsideBorder;
    }

    public void AddOccupant(GridTileOccupant occupant)
    {
        occupants.Add(occupant);
    }
    
    public void RemoveOccupant(GridTileOccupant occupant)
    {
        occupants.Remove(occupant);
    }

    public static GridTile FindClosestGridTile(Vector3 point)
    {
        
        GridTile[,] world = TerrainController.thisTerrainController.world;
        int tileSize = TerrainController.thisTerrainController.tileSize;

        int x = (int)((point.x + tileSize / 2) / tileSize);
        int z = (int)((point.z + tileSize / 2) / tileSize);
        if (world == null)
            return null;

        if (x >= world.GetLength(0) || x < 0)
            return null;

        if (z >= world.GetLength(1) || z < 0)
            return null;

        return world[x,z];
    }

    // Find all GridTiles in a radius around point
    public static List<GridTile> FindGridTilesAround(Vector3 point, float circleRadius)
    {

        List<GridTile> gridTiles = new List<GridTile>();
        GridTile middleTile = FindClosestGridTile(point);
        TerrainController terrain = TerrainController.thisTerrainController;

        float startTile = -circleRadius;
        float endTile = circleRadius;

        if (middleTile == null)
            return gridTiles;

        for (float i = startTile-1; i < endTile; i += terrain.tileSize)
        {
            for (float j = startTile-1; j < endTile; j += terrain.tileSize)
            {
                GridTile tile = FindClosestGridTile(new Vector3(middleTile.position.x + i, 0, middleTile.position.z + j));
                if (tile == null)
                    continue;

                if (Vector3.Distance(new Vector3(tile.position.x, 0, tile.position.z), new Vector3(point.x, 0, point.z)) < circleRadius)
                {
                    gridTiles.Add(FindClosestGridTile(tile.position));
                }
            }
        }

        return gridTiles;
    }

    // Find all GridTiles in a radius around point with an added option to skip tiles in between tiles that are returned
    public static List<GridTile> FindAnnulusAround(Vector3 point, int radius, int annulusTileWidth)
    {
        List<GridTile> gridTiles = new List<GridTile>();
        GridTile middleTile = FindClosestGridTile(point);
        TerrainController terrain = TerrainController.thisTerrainController;
        float startTile = -radius * terrain.tileSize;
        float endTile = radius * terrain.tileSize;

        float halfWidth = (float)annulusTileWidth / 2;

        for (float i = startTile; i < endTile; i += terrain.tileSize)
        {
            for (float j = startTile; j < endTile; j += terrain.tileSize)
            {
                GridTile tile = FindClosestGridTile(new Vector3(middleTile.position.x + i,0 , middleTile.position.z + j));

                if (tile == null)
                    continue;

                if (Vector2.Distance(new Vector2(tile.position.x, tile.position.z), new Vector2(middleTile.position.x, middleTile.position.z)) < ((radius + halfWidth) * terrain.tileSize)  &&
                    Vector2.Distance(new Vector2(tile.position.x, tile.position.z), new Vector2(middleTile.position.x, middleTile.position.z)) > ((radius - halfWidth) * terrain.tileSize))
                {
                    gridTiles.Add(tile);
                }
                else
                {
                    if (j < 0 && Vector2.Distance(new Vector2(tile.position.x, tile.position.z), new Vector2(middleTile.position.x, middleTile.position.z)) < ((radius - halfWidth) * terrain.tileSize))
                        j = -j;
                    else if (j > 0 && Vector2.Distance(new Vector2(tile.position.x, tile.position.z), new Vector2(middleTile.position.x, middleTile.position.z)) > ((radius + halfWidth) * terrain.tileSize))
                        break;
                }
            }
        }
        return gridTiles;
    }

    public void SaveWindTiles()
    {
        if(!isOutsideBorder)
            windEffectTiles =  FindGridTilesAround(position, 100);
    }

}
