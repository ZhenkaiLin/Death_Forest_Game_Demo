// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    [CreateAssetMenu(
        menuName = "Single-Player CCG Kit/Configuration/Non-player character",
        fileName = "NPCConfiguration",
        order = 1)]
    public class NonPlayableCharacterConfiguration : ScriptableObject
    {
        //战斗中会用到的基本属性
        public IntVariable Hp;
        public IntVariable Power;
        public IntVariable Magic;
        public IntVariable Armor;
        public IntVariable MagicResistance;

        public StatusVariable Status;

        public GameObject HpWidget;
        public GameObject StatusWidget;
        public GameObject IntentWidget;
        public GameObject AttributeWidget;
    }
}