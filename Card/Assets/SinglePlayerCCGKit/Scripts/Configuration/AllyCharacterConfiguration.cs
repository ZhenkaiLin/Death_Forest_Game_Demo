// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    [CreateAssetMenu(
        menuName = "Single-Player CCG Kit/Configuration/AllyCharacterConfiguration",
        fileName = "AllyCharacterConfiguration",
        order = 2)]
    public class AllyCharacterConfiguration : ScriptableObject
    {

        public GameObject HpWidget;
        public GameObject StatusWidget;
        public GameObject IntentWidget;
    }
}