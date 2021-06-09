using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Formation
{
    [Serializable]
    public class Formation
    {
        [SerializeField]
        private int identity;

        [SerializeField]
        private string name;

        [SerializeField]
        private List<Vector3> points = new List<Vector3>();

        public Formation(int identity, string name)
        {
            setIdentity(identity);
            this.name = name;
        }

        public Formation(string name, List<Vector3> points)
        {
            this.name = name;
            this.points = points;
        }

        public void setIdentity(int identity)
        {
            this.identity = identity;
        }

        public int getIdentity()
        {
            return identity;
        }

        public string getName()
        {
            return name;
        }

        public void addPoint(Vector3 point)
        {
            points.Add(point);
        }

        public List<Vector3> getPoints()
        {
            return points;
        }

        public Vector3 getPointByIndex(int pointIndex)
        {
            return points[pointIndex];
        }

        public Vector3 getCenterOfMass()
        {
            Vector3 totalPos = Vector3.zero;
            foreach (Vector3 point in points)
            {
                totalPos += point;
            }

            return totalPos / points.Count;
        }
    }
}
