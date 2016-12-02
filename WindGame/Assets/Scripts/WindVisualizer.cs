﻿using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

public class WindVisualizer : MonoBehaviour
{
    public List<Vector2>[] uvs;
    List<List<Vector2>> threadUvs = new List<List<Vector2>>();
    List<List<Vector3>> threadVerts = new List<List<Vector3>>();
    TerrainController terrain;

    public List<GameObject> windSpeedChunks = new List<GameObject>();
    public static WindVisualizer instance;
    Thread newThread;
    Thread[] chunkThreads = new Thread[20];
    Task[] tasks;

    readonly static object lockUVS = new object();

    public float height = 0;

    //float averageTime;
    //int timesUSed = 0;

    void Start()
    {
        instance = this;
    }

    public void VisualizeWind()
    {
        terrain = TerrainController.thisTerrainController;
        uvs = new List<Vector2>[terrain.chunks.Count];

        StartCoroutine("_VisualizeWind");

    }

    public void StopVisualizeWind()
    {
        StopCoroutine("_VisualizeWind");

        foreach (GameObject windSpeedChunk in windSpeedChunks)
        {
            if (windSpeedChunk != null)
            {
                Destroy(windSpeedChunk.GetComponent<MeshFilter>().mesh);
                Destroy(windSpeedChunk);
            }
        }
        windSpeedChunks.Clear();
        threadUvs.Clear();
        threadVerts.Clear();

        if (newThread != null && newThread.IsAlive)
            newThread.Abort();

        windSpeedChunks.Clear();
        newThread = null;
    }

    IEnumerator _VisualizeWind()
    {
        CreateChunks();
        tasks = new Task[chunkThreads.Length];
        while (true)
        {
            for (int i = 0; i < chunkThreads.Length; i++)
            {

                if (tasks[i] == null || tasks[i].isDone)
                    tasks[i] = MyThreadPool.AddActionToQueue(CalculateChunk, i);
            }


            yield return null;

            for (int i = 0; i < windSpeedChunks.Count; i++)
            {
                if (uvs[i] == null)
                    break;

                lock (lockUVS)
                {
                    windSpeedChunks[i].GetComponent<MeshFilter>().mesh.uv = uvs[i].ToArray();
                }
            }

            //}
        }
    }


    void CreateChunks()
    {
        Texture windTexture = (Texture)Resources.Load("windTexture");
        List<Vector3> threadVectors = new List<Vector3>();

        for (int i = 0; i < terrain.chunks.Count; i++)
        {
            GameObject chunk = terrain.chunks[i].gameObject;
            GameObject newWindChunk = new GameObject("WindChunk");
            newWindChunk.transform.position = chunk.transform.position + Vector3.up * 0.5f;

            newWindChunk.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
            newWindChunk.GetComponent<MeshRenderer>().material.mainTexture = windTexture;
            Mesh mesh = chunk.GetComponent<MeshFilter>().mesh;
            
            newWindChunk.AddComponent<MeshFilter>().mesh = mesh;

            threadUvs.Add(newWindChunk.GetComponent<MeshFilter>().mesh.uv.ToList());
            Vector3[] meshverts = newWindChunk.GetComponent<MeshFilter>().mesh.vertices;
            threadVectors = new List<Vector3>();

            for (int j = 0; j < meshverts.Length; j++)
            {
                threadVectors.Add(meshverts[j] + chunk.transform.position);
            }
            threadVerts.Add(threadVectors);
            windSpeedChunks.Add(newWindChunk);
        }
    }

    public void UpdateChunks()
    {
        for(int i = 0; i < windSpeedChunks.Count; i++)
        {
            Mesh mesh = windSpeedChunks[i].GetComponent<MeshFilter>().mesh;
            mesh.vertices = terrain.chunks[i].GetComponent<MeshFilter>().mesh.vertices;
            mesh.normals = terrain.chunks[i].GetComponent<MeshFilter>().mesh.normals;
        }
    }

    void CalculateChunk(object i)
    {
        int operationsPerThread = Mathf.CeilToInt(windSpeedChunks.Count / chunkThreads.Length);
        int windCount = windSpeedChunks.Count;

        int startK = (int)i * operationsPerThread;
        int endK = ((int)i + 1) * operationsPerThread;

        for (int k = startK; k < endK; k++)
        {
            if (k >= windCount)
                break;

            List<Vector3> vert = threadVerts[k];
            List<Vector2> CurChunkUVs = new List<Vector2>();

            for (int j = 0; j < vert.Count; j++)
            {
                GridTile tile = GridTile.FindClosestGridTile(vert[j]);
                if (tile == null || tile.isOutsideBorder)
                    CurChunkUVs.Add(new Vector2(0, 1));
                else if (!tile.canSeeWind)
                    CurChunkUVs.Add(new Vector2(0.125f, 0.875f));
                else
                {
                    float curWind = WindController.GetWindAtTile(tile, height);
                    CurChunkUVs.Add(new Vector2(1 - curWind / WindController.magnitude, 1 - curWind / WindController.magnitude));
                }
            }

                uvs[k] = CurChunkUVs;
            
        }
    }

    void OnApplicationQuit()
    {
        foreach(Thread thread in chunkThreads)
        {
            if(thread != null)
                thread.Abort();
        }
        if(newThread!= null)
            newThread.Abort();
    }
}