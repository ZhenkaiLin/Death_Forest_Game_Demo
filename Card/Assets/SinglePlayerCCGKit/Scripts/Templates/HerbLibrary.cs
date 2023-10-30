using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    [Serializable]
    [CreateAssetMenu(
        menuName = "Single-Player CCG Kit/Templates/Herb Library",
        fileName = "HerbLibrary",
        order = 3)]
    public class HerbLibrary : ScriptableObject
    {
        public string Name;
        public List<HerbLibraryEntry> Entries = new List<HerbLibraryEntry>();
    }
}

