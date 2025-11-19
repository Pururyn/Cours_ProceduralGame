using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ToolBuddy.SimpleMinecraftMap
{
    [ExecuteAlways]
    public class TerrainGenerator : MonoBehaviour
    {
        [Header("Terrain Settings")]
        [SerializeField]
        private int _width = 75;

        [SerializeField]
        private int _length = 75;

        [SerializeField]
        private int _maxHeight = 20;

        [SerializeField]
        private GameObject _terrainTilePrefab;

        [Space]
        [Header("Noise Settings")]
        [SerializeField]
        private int _seed;

        [SerializeField]
        private float _scale = 1f;

        [SerializeField]
        [Range(
            1,
            8
        )]
        private int _octaveCount = 1;

        [SerializeField]
        [Range(
            2,
            20f
        )]
        private float _lacunarity = 2f;

        [SerializeField]
        [Range(
            0,
            0.5f
        )]
        private float _gain = 0.5f;

        private readonly List<GameObject> _children = new List<GameObject>();
        private bool _isDirty;


        private void OnEnable()
        {
            ClearChildren();
            _isDirty = true;
        }

        private void OnDisable()
        {
            ClearChildren();
            _isDirty = true;
        }

        private void OnValidate()
        {
            _isDirty = true;
        }

        private void Update()
        {
            if (_isDirty)
            {
                GenerateTerrain();
                _isDirty = false;
            }
        }

        private void GenerateTerrain()
        {
            Random.InitState(_seed);

            float perlinOffsetI = Random.Range(
                0f,
                100000f
            );
            float perlinOffsetJ = Random.Range(
                0f,
                100000f
            );

            Transform generatorTransform = transform;
            int tileCount = 0;
            for (int i = 0; i < _width; i++)
                for (int j = 0; j < _length; j++)
                {
                    int height = GetTileHeight(
                        perlinOffsetI,
                        i,
                        perlinOffsetJ,
                        j
                    );

                    for (int k = 0; k < height; k++)
                    {
                        GameObject tile;
                        if (tileCount < _children.Count)
                        {
                            tile = _children[tileCount].gameObject;
                            tile.SetActive(true);
                        }
                        else
                        {
                            tile = Instantiate(
                                _terrainTilePrefab,
                                generatorTransform
                            );
                            _children.Add(tile);
                        }

                        tile.transform.position = new(
                            i,
                            k,
                            j
                        );
                        tileCount++;
                    }
                }

            for (int i = tileCount; i < _children.Count; i++)
            {
                GameObject child = _children[i];
                if (child.activeSelf)
                    child.SetActive(false);
                else
                    break;
            }
        }

        private int GetTileHeight(
            float perlinOffsetI,
            int i,
            float perlinOffsetJ,
            int j)
        {
            float randomValue = 0;
            float maxValue = 0;
            for (int k = 0; k < _octaveCount; k++)
            {
                float frequencyFactor = Mathf.Pow(
                    _lacunarity,
                    k
                );
                float amplitudeFactor = Mathf.Pow(
                    _gain,
                    k
                );
                float octaveRandomValue = Mathf.PerlinNoise(
                                              perlinOffsetI + ((_scale * frequencyFactor * i) / _width),
                                              perlinOffsetJ + ((_scale * frequencyFactor * j) / _length)
                                          )
                                          * amplitudeFactor;
                randomValue += octaveRandomValue;
                maxValue += amplitudeFactor;
            }

            return Mathf.RoundToInt((randomValue / maxValue) * _maxHeight);
        }

        private void ClearChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                DestroyImmediate(child.gameObject);
            }

            _children.Clear();
        }
    }
}