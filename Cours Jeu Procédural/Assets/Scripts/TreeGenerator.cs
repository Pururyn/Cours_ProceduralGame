using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    [SerializeField] private int numberMax;

    public Vector3 spawnAreaMin;
    public Vector3 spawnAreaMax;
    public GameObject[] trees; 

    void Start()
    {
        for (int i = 0; i < numberMax; i++)
        {
            SpawnRandom();
        }
    }

    void SpawnRandom()
    {
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomZ = Random.Range(spawnAreaMin.z, spawnAreaMax.z);

        Vector3 pos = new Vector3(randomX, 0f, randomZ);

        // random tree in list
        GameObject prefab = trees[Random.Range(0, trees.Length)];

        Instantiate(prefab, pos, Quaternion.identity);
    }
}