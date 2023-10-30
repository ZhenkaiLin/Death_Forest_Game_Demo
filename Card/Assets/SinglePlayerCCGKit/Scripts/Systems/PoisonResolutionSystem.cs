// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// This system is responsible for managing the poison status on characters.
    /// It deals poison damage at the beginning of the turn, and reduces the poison
    /// stack at the end of the turn.
    /// </summary>
    public class PoisonResolutionSystem : BaseSystem
    {
        public void OnPlayerTurnBegan()
        {
            var player = Player.Character;
            var poison = player.Status.GetValue("Poison");
            if (poison > 0)
            {
                var playerMagicResistance = player.MagicResistance;
                var NewMR = (int)Mathf.Floor(playerMagicResistance.Value * 3 / 4);
                playerMagicResistance.SetValue(NewMR);

                var playerHp = player.Hp;
                var NewHP = playerHp.Value-(int)Mathf.Floor(2 + player.Hp.Value /20);
                playerHp.SetValue(NewHP);
            }
        }

        public void OnPlayerTurnEnded()
        {
            var player = Player.Character;
            var poison = player.Status.GetValue("Poison");
            if (poison > 0)
            {
                player.Status.SetValue("Poison", poison-1);
            }
        }

        public void OnEnemyTurnBegan()
        {
            var enemy = Enemy.Character;
            var poison = enemy.Status.GetValue("Poison");
            if (poison > 0)
            {
                var enemyMagicResistance = enemy.MagicResistance;
                var NewMR = (int)Mathf.Floor(enemyMagicResistance.Value * 3 / 4);
                enemyMagicResistance.SetValue(NewMR);

                var enemyHp = enemy.Hp;
                var NewHP = enemyHp.Value - (int)Mathf.Floor(2 + enemy.Hp.Value / 20);
                enemyHp.SetValue(NewHP);
            }
        }

        public void OnEnemyTurnEnded()
        {
            var enemy = Enemy.Character;
            var poison = enemy.Status.GetValue("Poison");
            if (poison > 0)
            {
                enemy.Status.SetValue("Poison", poison-1);
            }
        }
    }
}
