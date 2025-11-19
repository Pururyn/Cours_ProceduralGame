using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    [ExecuteAlways]
    [SerializeField] private int numberMax;
    [SerializeField] private int _seed;

    public Vector3 spawnAreaMin;
    public Vector3 spawnAreaMax;
    public GameObject[] trees;

    public GameObject treeGenerator;

    private bool _needsUpdate;

    private void OnValidate()
    {
        _needsUpdate = true;
    }

    private void Update()
    {
        if (_needsUpdate)
        {
            SpawnRandom();
            _needsUpdate = false;
        }
    }

    private void SpawnRandom()
    {
        
        Random.InitState(_seed); //Number of Seed (1 seed = 1 random map = same map if same seed)

        // Clear unwanted trees went we reduce the amount of trees
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }


        for (int i = 0; i < numberMax; i++)
        {
            float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float randomZ = Random.Range(spawnAreaMin.z, spawnAreaMax.z);

            Vector3 pos = new Vector3(randomX, 0f, randomZ);

            // random tree in list
            GameObject prefab = trees[Random.Range(0, trees.Length)];

            Instantiate(prefab, pos, Quaternion.identity, treeGenerator.transform);
        }
    }  
}