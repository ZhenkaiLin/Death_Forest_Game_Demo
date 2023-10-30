// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CCGKit
{
    /// <summary>
    /// The widget used to display a character's HP and shield.
    /// </summary>
    public class AttributeWidget : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private GameObject Power;    
        [SerializeField]
        private TextMeshProUGUI powerText;
        [SerializeField]
        private TextMeshProUGUI powerBorderText;

        [SerializeField]
        private GameObject Magic;
        [SerializeField]
        private TextMeshProUGUI magicText;
        [SerializeField]
        private TextMeshProUGUI magicBorderText;

        [SerializeField]
        private GameObject Armor;
        [SerializeField]
        private TextMeshProUGUI armorText;
        [SerializeField]
        private TextMeshProUGUI armorBorderText;

        [SerializeField]
        private GameObject MagicResistance;
        [SerializeField]
        private TextMeshProUGUI magicResistanceText;
        [SerializeField]
        private TextMeshProUGUI magicResistanceBorderText;
        [SerializeField]
#pragma warning restore 649
        private const float InitialDelay = 1.25f;
        private const float FadeInDuration = 0.8f;
        private const float FadeOutDuration = 0.5f;

        public void Initialize(IntVariable power, IntVariable magic, IntVariable armor, IntVariable magicResistance)
        {
            SetAttribute(power.Value, magic.Value, armor.Value, magicResistance.Value);
        }

        public void SetAttribute(int power, int magic, int armor, int magicResistance)
        {
            powerText.text = $"{power.ToString()}";
            powerBorderText.text = $"{power.ToString()}";

            magicText.text = $"{magic.ToString()}";
            magicBorderText.text = $"{magic.ToString()}";

            armorText.text = $"{armor.ToString()}";
            armorBorderText.text = $"{armor.ToString()}";

            magicResistanceText.text = $"{magicResistance.ToString()}";
            magicResistanceBorderText.text = $"{magicResistance.ToString()}";
        }

        public void OnAttributePowerChanged(int power)
        {
            var Color = new Color32(155, 162, 38, 255);
            if (power < (int)float.Parse(powerText.text))
                Color = new Color32(164, 0, 5, 255);
            powerText.text = $"{power.ToString()}";
            powerBorderText.text = $"{power.ToString()}";
            powerText.color = Color;
            powerBorderText.color = Color;
        }

        public void OnAttributeMagicChanged(int magic)
        {
            var Color = new Color32(155, 162, 38, 255);
            if (magic < (int)float.Parse(magicText.text))
                Color = new Color32(164, 0, 5, 255);
            magicText.text = $"{magic.ToString()}";
            magicBorderText.text = $"{magic.ToString()}";
            magicText.color = Color;
            magicBorderText.color = Color;
        }

        public void OnAttributeArmorChanged(int armor)
        {
            var Color = new Color32(155, 162, 38, 255);
            if (armor < (int)float.Parse(armorText.text))
                Color = new Color32(164, 0, 5, 255);
            armorText.text = $"{armor.ToString()}";
            armorBorderText.text = $"{armor.ToString()}";
            armorText.color = Color;
            armorBorderText.color = Color;
        }

        public void OnAttributeMagicResistanceChanged(int magicResistance)
        {
            var Color = new Color32(155, 162, 38, 255);
            if (magicResistance < (int)float.Parse(magicResistanceText.text))
                Color = new Color32(164, 0, 5, 255);
            magicResistanceText.text = $"{magicResistance.ToString()}";
            magicResistanceBorderText.text = $"{magicResistance.ToString()}";
            magicResistanceText.color = Color;
            magicResistanceBorderText.color = Color;
        }

    }

}
