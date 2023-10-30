using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    [Serializable]
    [CreateAssetMenu(
        menuName = "Single-Player CCG Kit/Templates/Herb",
        fileName = "HerbTemplate",
        order = 0)]
    public class HerbTemplate : ScriptableObject
    {
        public int Id;
        public string Name;
        public Material Material;
        public Sprite Picture;
        public Sprite Glow;
        public Color GlowColor;
        public List<Effect> Effects = new List<Effect>();
    }
}
