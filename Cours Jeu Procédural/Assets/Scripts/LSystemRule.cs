using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ToolBuddy.PCG.LSystems
{
    [Serializable]
    public record LSystemRule
    {
        [field: SerializeField] public char Predecessor { get; set; }
        [field: SerializeField] public string Successor { get; set; }

        public LSystemRule(
            char predecessor,
            string successor)
        {
            Predecessor = predecessor;
            Successor = successor;
        }
    }
}