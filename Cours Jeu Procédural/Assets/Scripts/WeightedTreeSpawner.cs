using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace ToolBuddy.PCG.TreeSpawners
{
    [ExecuteAlways]
    public class WeightedTreeSpawner : MonoBehaviour
    {
        [SerializeField]
        private WeightedGameObject[] _treePrefabs;

        [SerializeField]
        private int _seed;

        [SerializeField]
        private float _radius = 1;

        [SerializeField]
        private Vector2 _regionSize = Vector2.one;

        [SerializeField]
        private Vector2 _regionOffset = Vector2.one;

        [SerializeField]
        private int _rejectionSamples = 30;

        #region Dirtiness handling

        private bool _isDirty;

        private void OnEnable()
        {
            _isDirty = true;
        }

        private void OnDisable()
        {
            _isDirty = true;
        }

        private void Reset()
        {
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
                Generate();
                _isDirty = false;
            }
        }

        #endregion

        private void Generate()
        {
            Random.InitState(_seed);

            DestroyChildren();

            if (_treePrefabs.Length == 0)
                return;

            List<Vector2> spawnPositions = PoissonDiscSampling.GeneratePoints(
                _radius,
                _regionSize,
                _rejectionSamples
            );

            SpawnTrees(spawnPositions);
        }

        private void SpawnTrees(
            List<Vector2> treePositions)
        {
            foreach (Vector2 point in treePositions)
            {
                Vector3 position = new(
                    _regionOffset.x + point.x,
                    0,
                    _regionOffset.y + point.y
                );

                GameObject tree = Instantiate(
                    _treePrefabs[GetTreeIndex()].GameObject,
                    position,
                    Quaternion.identity,
                    transform
                );

                tree.hideFlags = HideFlags.DontSave;
            }
        }

        private int GetTreeIndex()
        {
            Assert.IsTrue(_treePrefabs.All(p => p.Weight >= 0));

            float totalWeight = _treePrefabs.Sum(p => p.Weight);

            float randomValue = Random.Range(
                0f,
                totalWeight
            );

            float weightAccumulator = 0f;
            for (int index = 0; index < _treePrefabs.Length; index++)
            {
                WeightedGameObject treePrefab = _treePrefabs[index];
                weightAccumulator += treePrefab.Weight;
                if (randomValue <= weightAccumulator)
                    return index;
            }

            return _treePrefabs.Length - 1;
        }

        private void DestroyChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                DestroyImmediate(child.gameObject);
            }
        }
    }
}