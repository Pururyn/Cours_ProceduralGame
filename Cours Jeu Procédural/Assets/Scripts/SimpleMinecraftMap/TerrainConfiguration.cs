using System;
using UnityEngine;

namespace ToolBuddy.PCG.SimpleMinecraftMap
{
    [CreateAssetMenu(
        fileName = "TerrainConfiguration",
        menuName = "Scriptable Objects/TerrainConfiguration"
    )]
    public class TerrainConfiguration : ScriptableObject
    {
        public event Action<TerrainConfiguration> OnConfigurationChanged;

        [Header("Terrain Settings")]
        [SerializeField]
        public int Length = 75;

        [SerializeField]
        public int Width = 75;

        [Space]
        [Header("Noise Settings")]
        [SerializeField]
        public int Seed;

        [SerializeField]
        public int MaxHeight = 20;

        [SerializeField]
        public AnimationCurve HeightModifier = new(
            new Keyframe(
                0,
                1
            ),
            new Keyframe(
                1,
                1
            )
        );

        [SerializeField]
        public float Scale = 1f;

        [SerializeField]
        [Range(
            1,
            8
        )]
        public int OctaveCount = 1;

        [SerializeField]
        [Range(
            2,
            20f
        )]
        public float Lacunarity = 2f;

        [SerializeField]
        [Range(
            0,
            0.5f
        )]
        public float Gain = 0.5f;

        private void Reset()
        {
            OnConfigurationChanged?.Invoke(this);
        }

        private void OnValidate()
        {
            OnConfigurationChanged?.Invoke(this);
        }
    }
}