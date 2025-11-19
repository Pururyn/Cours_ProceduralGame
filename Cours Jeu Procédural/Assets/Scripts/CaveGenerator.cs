using System.Collections;
using System.Diagnostics;
using SimplexNoise;
using UnityEditor;
using UnityEngine;

namespace ToolBuddy.PCG.SimpleMinecraftMap
{
    [ExecuteAlways]
    public class CaveGenerator : MonoBehaviour
    {
        [Header("Cave Settings")]
        [SerializeField]
        private int _cubeDimension = 30;

        [Range(0, 255)]
        [SerializeField]
        private int _densityThreshold = 128;

        [Header("Noise Settings")]
        [Range(
            0,
            .3f
        )]
        [SerializeField]
        private float _scale = 0.05f;

        [SerializeField]
        [Range(
            1,
            100
        )]
        [Tooltip("In milliseconds")]
        private int _frameBudget = 32;

        [SerializeField]
        private int _seed;

        [Header("Prefab")]
        [SerializeField]
        private GameObject _cubePrefab;

        #region Time Slicing

        private Coroutine _generationCoroutine;

        private void EditorUpdate()
        {
            if (!Application.isPlaying)
                EditorApplication.QueuePlayerLoopUpdate();
        }

        #endregion

        #region Dirtying handling

        private bool _isDirty;

        private void OnEnable()
        {
            EditorApplication.update += EditorUpdate;
            SetDirty();
        }

        private void OnDisable()
        {
            EditorApplication.update -= EditorUpdate;
            SetDirty();
        }

        private void OnValidate()
        {
            SetDirty();
        }

        private void Reset()
        {
            SetDirty();
        }

        private void SetDirty()
        {
            _isDirty = true;
            if (_generationCoroutine != null)
                StopCoroutine(_generationCoroutine);
        }

        private void Update()
        {
            if (_isDirty)
            {
                _generationCoroutine = StartCoroutine(GenerateTerrain());
                _isDirty = false;
            }
        }

        #endregion

        private IEnumerator GenerateTerrain()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            DestroyChildren();

            if (_cubePrefab == null)
                yield break;

            Noise.Seed = _seed;

            float[,,] noiseVolume = Noise.Calc3D(
                _cubeDimension,
                _cubeDimension,
                _cubeDimension,
                _scale
            );

            for (int x = 0; x < _cubeDimension; x++)
                for (int y = 0; y < _cubeDimension; y++)
                    for (int z = 0; z < _cubeDimension; z++)
                    {
                        float density = noiseVolume[x,
                            y,
                            z];

                        if (density >= _densityThreshold)
                            continue;

                        Vector3 position = new(
                            x,
                            y,
                            z
                        );

                        Instantiate(
                            _cubePrefab,
                            position,
                            Quaternion.identity,
                            transform
                        );

                        if (stopwatch.ElapsedMilliseconds >= _frameBudget)
                        {
                            yield return null;

                            stopwatch.Restart();
                        }
                    }

            _generationCoroutine = null;
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