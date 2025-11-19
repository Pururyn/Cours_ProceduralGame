using System;
using UnityEngine;

namespace ToolBuddy.PCG.TreeSpawners
{
    [Serializable]
    public class WeightedGameObject
    {
        [field: SerializeField] public GameObject GameObject { get; set; }

        [field: SerializeField] public float Weight { get; set; } = 1;
    }
}