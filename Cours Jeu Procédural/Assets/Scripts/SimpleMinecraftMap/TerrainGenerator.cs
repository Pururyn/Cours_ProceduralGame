using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ToolBuddy.PCG.SimpleMinecraftMap
{
    [ExecuteAlways]
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField]
        private TerrainConfiguration _configuration;

        private readonly List<GameObject> _childrenGameObjects = new();
        private readonly List<Transform> _childrenTransforms = new();
        private readonly ProfilerMarker _generateTerrainMarker = new("TerrainGenerator.GetTileHeight");

        private GameObject _terrainTilePrefab;

        #region Dirtiness handling

        private bool _isDirty;

        private void OnEnable()
        {
            ClearChildren();
            _isDirty = true;
            SubscribeToConfigurationChanges();
            _terrainTilePrefab = Resources.Load<GameObject>("Cube");
        }

        private void OnDisable()
        {
            ClearChildren();
            _isDirty = true;
            UnsubscribeFromConfigurationChanges();
            _terrainTilePrefab = null;
        }

        private void Reset()
        {
            _isDirty = true;
        }

        private void OnValidate()
        {
            _isDirty = true;
            UnsubscribeFromConfigurationChanges();
            SubscribeToConfigurationChanges();
        }

        private void Update()
        {
            if (_isDirty)
            {
                GenerateTerrain();
                _isDirty = false;
            }
        }

        #endregion

        private void GenerateTerrain()
        {
            int tileCount;

            if (_configuration)
                tileCount = GenerateTiles();
            else
                tileCount = 0;

            DisableUnusedTiles(tileCount);
        }

        private int GenerateTiles()
        {
            Random.InitState(_configuration.Seed);

            // 100000f is an arbitrary large number.
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
            for (int i = 0; i < _configuration.Length; i++)
            for (int j = 0; j < _configuration.Width; j++)
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
                    if (tileCount >= _childrenGameObjects.Count)
                    {
                        tile = Instantiate(
                            _terrainTilePrefab,
                            generatorTransform
                        );
                        tile.hideFlags = HideFlags.DontSave;
                        _childrenGameObjects.Add(tile);
                        _childrenTransforms.Add(tile.transform);
                    }
                    else
                    {
                        tile = _childrenGameObjects[tileCount];
                        tile.SetActive(true);
                    }

                    _childrenTransforms[tileCount].position = new(
                        i,
                        k,
                        j
                    );
                    tileCount++;
                }
            }

            return tileCount;
        }

        private int GetTileHeight(
            float perlinOffsetI,
            int i,
            float perlinOffsetJ,
            int j)
        {
            using (_generateTerrainMarker.Auto())
            {
                float randomValue = 0;
                float maxValue = 0;
                for (int k = 0; k < _configuration.OctaveCount; k++)
                {
                    float frequencyFactor = Mathf.Pow(
                        _configuration.Lacunarity,
                        k
                    );
                    float amplitudeFactor = Mathf.Pow(
                        _configuration.Gain,
                        k
                    );
                    float octaveRandomValue = Mathf.PerlinNoise(
                                                  perlinOffsetI
                                                  + ((_configuration.Scale * frequencyFactor * i) / _configuration.Length),
                                                  perlinOffsetJ
                                                  + ((_configuration.Scale * frequencyFactor * j) / _configuration.Width)
                                              )
                                              * amplitudeFactor;
                    randomValue += octaveRandomValue;
                    maxValue += amplitudeFactor;
                }

                int maxHeight = GetMaxHeight(
                    perlinOffsetI,
                    i,
                    perlinOffsetJ,
                    j
                );
                return Mathf.RoundToInt((randomValue / maxValue) * maxHeight);
            }
        }

        private int GetMaxHeight(
            float perlinOffsetI,
            int i,
            float perlinOffsetJ,
            int j)
        {
            float value = Mathf.PerlinNoise(
                (perlinOffsetI * 2) + ((_configuration.Scale * i) / _configuration.Length),
                (perlinOffsetJ * 2) + ((_configuration.Scale * j) / _configuration.Width)
            );

            return Mathf.RoundToInt(_configuration.MaxHeight * _configuration.HeightModifier.Evaluate(value));
        }

        private void DisableUnusedTiles(
            int usedTileCount)
        {
            for (int i = usedTileCount; i < _childrenGameObjects.Count; i++)
            {
                GameObject child = _childrenGameObjects[i];
                if (child.activeSelf)
                    child.SetActive(false);
                else
                    break;
            }
        }

        private void ClearChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                DestroyImmediate(child.gameObject);
            }

            _childrenGameObjects.Clear();
            _childrenTransforms.Clear();
        }

        #region Configuration Change Tracking

        private void SubscribeToConfigurationChanges()
        {
            if (_configuration != null)
                _configuration.OnConfigurationChanged += OnConfigurationChanged;
        }

        private void UnsubscribeFromConfigurationChanges()
        {
            if (_configuration != null)
                _configuration.OnConfigurationChanged -= OnConfigurationChanged;
        }

        private void OnConfigurationChanged(
            TerrainConfiguration sender)
        {
            if (_configuration != sender)
                sender.OnConfigurationChanged -= OnConfigurationChanged;
            else
                _isDirty = true;
        }

        #endregion
    }
}