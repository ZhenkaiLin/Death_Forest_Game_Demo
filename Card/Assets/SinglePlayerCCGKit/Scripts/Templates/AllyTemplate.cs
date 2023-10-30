// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    [Serializable]
    [CreateAssetMenu(
        menuName = "Single-Player CCG Kit/Templates/Ally",
        fileName = "Ally",
        order = 3)]
    public class AllyTemplate : CharacterTemplate
    {
        public int hp;
        public int power;
        public int magic;
        public int armor;
        public int magicResistance;

        public List<Pattern> Patterns = new List<Pattern>();
    }
}