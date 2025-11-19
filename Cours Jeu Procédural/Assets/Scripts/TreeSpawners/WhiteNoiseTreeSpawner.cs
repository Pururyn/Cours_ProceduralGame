using UnityEngine;

namespace ToolBuddy.PCG.TreeSpawners
{
    [ExecuteAlways]
    public class WhiteNoiseTreeSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject _treePrefab;

        [SerializeField]
        private int _numberOfTrees = 300;

        [SerializeField]
        private float _width = 100;

        [SerializeField]
        private float _length = 100;

        [SerializeField]
        private int _seed;

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
            DestroyChildren();

            Random.InitState(_seed);

            SpawnTrees();
        }

        private void SpawnTrees()
        {
            for (int i = 0; i < _numberOfTrees; i++)
            {
                float x = Random.Range(
                    -_width * 0.5f,
                    _width * 0.5f
                );
                float z = Random.Range(
                    -_length * 0.5f,
                    _length * 0.5f
                );
                Vector3 position = new Vector3(
                    x,
                    0,
                    z
                );
                Instantiate(
                    _treePrefab,
                    position,
                    Quaternion.identity,
                    transform
                );
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