using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{

    [CreateAssetMenu(
    menuName = "Single-Player CCG Kit/Effects/GainMagicResistance",
    fileName = "GainMagicResistance",
    order = 2)]
    public class GainMagicResistance : IntegerEffect, IEntityEffect
    {
        // Start is called before the first frame update
        public override string GetName()
        {
            return $"Gain {Value.ToString()} Shield";
        }

        public override void Resolve(RuntimeCharacter instigator, RuntimeCharacter target)
        {
            var magicResistance = target.MagicResistance;
            magicResistance.SetValue(magicResistance.Value + Value);
        }

    }
}

