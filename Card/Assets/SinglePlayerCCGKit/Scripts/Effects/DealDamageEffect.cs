// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The type corresponding to the "Deal X damage" card effect.
    /// </summary>
    ///     

    [Serializable]
    [CreateAssetMenu(
    menuName = "Single-Player CCG Kit/Effects/DealDamage",
    fileName = "DealDamage",
    order = 3)]
    public class DealDamageEffect : IntegerEffect, IEntityEffect
    {
        public enum damageType { Physical, SunderPhysical, Magic, SunderMagic };
        public damageType DamageType;
        public override string GetName()
        {
            if (DamageType == damageType.Physical)
                return $"Deal {Value.ToString()} extra Physical Damage";
            else if (DamageType == damageType.SunderPhysical) 
                return $"Deal {Value.ToString()} extra Sunder Physical damage";
            else if (DamageType == damageType.Magic)
                return $"Deal {Value.ToString()} extra Magic damage";
            else
                return $"Deal {Value.ToString()} extra Sunder Magic damage";
        }
        public override void Resolve(RuntimeCharacter instigator, RuntimeCharacter target)
        {
            var targetHp = target.Hp;
            var targetArmor = target.Armor;
            var targetMagicResistance = target.MagicResistance;
            var instigatorPower = instigator.Power;
            var instigatorMagic = instigator.Magic;

            var hp = targetHp.Value;
            if (DamageType == damageType.Physical || DamageType == damageType.SunderPhysical)
            {
                //角色造成物理伤害计算
                var PhysicalDamage = Value + instigatorPower.Value;

                //目标收到物理伤害计算
                //强化状态
                var angry = instigator.Status.GetValue("Strength");
                if (angry > 0)
                    PhysicalDamage = (int)Mathf.Floor(PhysicalDamage * 1.5f);
                //虚弱状态
                var weak = instigator.Status.GetValue("Weak");
                if (weak > 0)
                    PhysicalDamage = (int)Mathf.Floor(PhysicalDamage * 0.75f);

                //更新目标的HP值
                var newHp = 0;
                if (DamageType == damageType.Physical)
                {
                    if (PhysicalDamage >= targetArmor.Value && PhysicalDamage > 0)
                        newHp = hp - (PhysicalDamage - targetArmor.Value);
                    else
                        newHp = hp;
                }
                else
                    newHp = hp - PhysicalDamage;
                if (newHp < 0)
                    newHp = 0;
                targetHp.SetValue(newHp);
            }
            else
            {
                //角色造成魔法伤害计算
                var MagicDamage = Value + instigatorMagic.Value;
                //目标收到魔法伤害计算
                //愤怒状态
                var angry = instigator.Status.GetValue("Strength");
                if (angry > 0)
                    MagicDamage = (int)Mathf.Floor(MagicDamage * 1.5f);

                //更新目标的HP值
                var newHp = 0;
                if (DamageType == damageType.Magic)
                {
                    if (MagicDamage >= targetArmor.Value && MagicDamage > 0)
                        newHp = hp - (MagicDamage - targetMagicResistance.Value);
                    else
                        newHp = hp;
                }
                else
                    newHp = hp - MagicDamage;
                if (newHp < 0)
                    newHp = 0;
                targetHp.SetValue(newHp);
            }
        }
    }
}
