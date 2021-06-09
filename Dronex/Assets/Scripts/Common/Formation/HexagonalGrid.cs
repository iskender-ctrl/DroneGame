using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Formation
{
    [Serializable]
    public class HexagonalPoint
    {
        [SerializeField]
        public float q;

        [SerializeField]
        public float r;

        public float len()
        {
            return (Mathf.Abs(q) + Mathf.Abs(r) + Math.Abs(-q - r)) / 2;
        }
    }

    [Serializable]
    public class HexagonalGridData
    {
        [SerializeField]
        public List<HexagonalPoint> points = new List<HexagonalPoint>();
    }
}