// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;

namespace CCGKit
{
    /// <summary>
    /// The type corresponding to the "Gain X shield" card effect.
    /// </summary>
    [Serializable]
    public class GainArmorEffect : IntegerEffect, IEntityEffect
    {
        public enum GainType {Armor, MagicResistance, Power, Magic };
        public GainType gainType;

        public override string GetName()
        {
            if (gainType == GainType.Armor) 
                return $"Gain {Value.ToString()} Armor";
            else if (gainType == GainType.MagicResistance)
                return $"Gain {Value.ToString()} MagicResistance";
            else if(gainType == GainType.Magic)
                return $"Gain {Value.ToString()} Magic";
            else
                return $"Gain {Value.ToString()} Poewr";
        }

        public override void Resolve(RuntimeCharacter instigator, RuntimeCharacter target)
        {
            var instigatorArmor = instigator.Armor;
            var instigatorMagicResistance = instigator.MagicResistance;
            if (gainType == GainType.Armor)
            {
                instigatorArmor = instigator.Armor;
                instigatorArmor.SetValue(instigatorArmor.Value + Value);
            }
            else if (gainType == GainType.MagicResistance)
            {
                instigatorMagicResistance = instigator.MagicResistance;
                instigatorMagicResistance.SetValue(instigatorMagicResistance.Value + Value);
            }
        }
    }
}
