using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Formation
{
    [Serializable]
    public class FormationData
    {
        [SerializeField]
        public List<Formation> formations = new List<Formation>();
    }
}
