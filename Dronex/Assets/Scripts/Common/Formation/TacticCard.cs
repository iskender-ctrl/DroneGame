using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Formation
{
    public class TacticCard
    {
        private Formation formation;
    }

    [Serializable]
    public class SelectedTacticCards
    {
        [SerializeField]
        public List<int> selectedTacticCards = new List<int>();

        public void addTacticCard(int tacticCardIndex)
        {
            selectedTacticCards.Add(tacticCardIndex);
        }
    }
}
