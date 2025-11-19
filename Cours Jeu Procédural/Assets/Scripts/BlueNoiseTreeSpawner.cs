using System.Collections.Generic;
using UnityEngine;

namespace ToolBuddy.PCG.TreeSpawner
{
    [ExecuteAlways]
    public class BlueNoiseTreeSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject _treePrefab;

        [SerializeField]
        private int _seed;

        [SerializeField]
        private float _radius = 2;

        [SerializeField]
        private Vector2 _regionSize = Vector2.one * 100;

        [SerializeField]
        private Vector2 _regionOffset = Vector2.one * -50;

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

        #endregion

        private void Update()
        {
            if (_isDirty)
            {
                Generate();
                _isDirty = false;
            }
        }

        private void Generate()
        {
            DestroyChildren();

            Random.InitState(_seed);

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
                GameObject tree = Instantiate(
                    _treePrefab,
                    new Vector3(
                        _regionOffset.x + point.x,
                        0,
                        _regionOffset.y + point.y
                    ),
                    Quaternion.identity,
                    transform
                );

                tree.hideFlags = HideFlags.DontSave;
            }
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