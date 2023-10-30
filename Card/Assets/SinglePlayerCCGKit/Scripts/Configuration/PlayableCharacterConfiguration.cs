// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    [CreateAssetMenu(
        menuName = "Single-Player CCG Kit/Configuration/Player character",
        fileName = "PCConfiguration",
        order = 0)]
    public class PlayableCharacterConfiguration : ScriptableObject
    {
        //玩家战斗中的基本属性
        public IntVariable Hp;
        public IntVariable Power;
        public IntVariable Magic;
        public IntVariable Armor;
        public IntVariable MagicResistance;
        public IntVariable Level;
        public IntVariable Mana;
        //战斗中的状态
        public StatusVariable Status;

        public GameObject HpWidget;
        public GameObject StatusWidget;
        public GameObject AttributeWidget;
    }
}