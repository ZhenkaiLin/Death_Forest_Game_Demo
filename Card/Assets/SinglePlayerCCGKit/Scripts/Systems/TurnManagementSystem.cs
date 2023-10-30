// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// This system is responsible for managing the game's turn sequence, which always follows
    /// the order:
    ///     - Player turn.
    ///     - Enemies turn.
    /// It creates event-like entities with the Event component attached to them when the turn
    /// sequence advances (beginning of player turn, end of player turn, beginning of enemies
    /// turn, end of enemies turn).
    /// </summary>
    public class TurnManagementSystem : MonoBehaviour,AllyBaseSystem
    {
        public GameEvent PlayerTurnBegan;
        public GameEvent PlayerTurnEnded;
        public GameEvent EnemyTurnBegan;
        public GameEvent EnemyTurnEnded;
        public GameEvent AllyTurnBegan;
        public GameEvent AllyTurnEnded;

        //数据交接新增
        public DataExchangeSystem dataExchangeSystem;

        private bool isEnemyTurn;
        private float enemyAccTime;

        private bool isEndOfGame;

        private const float EnemyTurnDuration = 3.0f;

        //同伴系统新增+重构
        private bool isAllyTurn=false;
        private float allyAccTime=0;
        private const float allyTurnDuration = 3.0f;
        private int allyidx;
        protected CharacterObject[] allys=null;
        
        public void allyInitialize(CharacterObject[] allysIn)
        {
            allys = allysIn;
        }

        protected void Update()
        {
            if (isEnemyTurn)
            {
                enemyAccTime += Time.deltaTime;
                if (enemyAccTime >= EnemyTurnDuration)
                {
                    enemyAccTime = 0.0f;
                    EndEnemyTurn();
                    BeginPlayerTurn();
                }
            }
            //同伴系统新增 不用事件触发同伴回合开始 而是直接调用 否则需要各自加一个事件
            else if(isAllyTurn)
            {
                //Assertion保护
                if (allys.Length==0)
                {
                    EndAllyTurn();
                    return;
                }
                if(allyAccTime==0)
                {

                    var allybrain=allys[allyidx].GetComponent<AllyBrainSystem>();
                    allybrain.OnAllyTurnBegan();
                    allyidx++;
                    if(allyidx>=allys.Length)
                    {
                        EndAllyTurn();
                    }
                }
                allyAccTime += Time.deltaTime;
                if(allyAccTime>=allyTurnDuration)
                {
                    allyAccTime = 0.0f;
                }
            }
        }

        public void BeginGame()
        {
            BeginPlayerTurn();
        }

        public void BeginPlayerTurn()
        {
            PlayerTurnBegan.Raise();
        }

        public void EndPlayerTurn()
        {
            PlayerTurnEnded.Raise();
            BeginAllyTurn();
        }
        //同伴系统新增+重构
        public void BeginAllyTurn()
        {
            isAllyTurn = true;
            allyidx = 0;
            AllyTurnBegan.Raise();
            //EndAllyTurn();
        }

        public void EndAllyTurn()
        {
            isAllyTurn = false;
            AllyTurnEnded.Raise();
            BeginEnemyTurn();
        }

        public void BeginEnemyTurn()
        {
            EnemyTurnBegan.Raise();
            isEnemyTurn = true;
        }

        public void EndEnemyTurn()
        {
            EnemyTurnEnded.Raise();
            isEnemyTurn = false;
        }

        public void SetEndOfGame(bool value)
        {
            //数据交接新增
            dataExchangeSystem.uploadData();

            isEndOfGame = value;
        }

        public bool IsEndOfGame()
        {

            return isEndOfGame;
        }
    }
}
