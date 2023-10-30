
// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening; 
using UnityEngine;
using UnityEngine.Rendering;

namespace CCGKit
{
    /// <summary>
    /// This system creates a targeting arrow when the player selects a card from his hand that
    /// contains a targeted effect. This arrow allows the player to easily select the target of
    /// the card's effect.
    /// </summary>
    public class CardWithArrowSelectionSystem : CardSelectionSystem
    {
        private RaycastHit2D[] raycastResults = new RaycastHit2D[1];
        private GameObject highlightedCard;

        private TargetingArrow targetingArrow;

        private GameObject selectedEnemy;

        private Vector3 prevClickPos;

        private bool isArrowCreated;

        private const float CardSelectionDetectionOffset = 2.2f;
        private const float CardSelectionAnimationTime = 0.2f;
        private const float CardSelectionCanceledAnimationTime = 0.2f;
        private const float SelectedCardYOffset = -4.0f;

        //草药系统增加字段
        
        LayerMask herbLayer;
        LayerMask allyLayer;
        GameObject selectedHerb;
        private GameObject selectedAlly;
        private const float herbSelectionDetectionOffset = -0.2f;
        protected override void Start()
        {
            base.Start();
            targetingArrow = Object.FindObjectOfType<TargetingArrow>();
            herbLayer = 1 << LayerMask.NameToLayer("Herb");
            allyLayer = 1 << LayerMask.NameToLayer("Ally");
        }
        //草药系统重构
        private void Update()
        {
            if (TurnManagementSystem.IsEndOfGame())
                return;

            if (HandPresentationSystem.IsAnimating())
                return;

            if (Input.GetMouseButtonDown(0))
            {
                DetectCardSelection();
                DetectHerbSelection();
                DetectEnemySelection();
                DetectAllySelection();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                DetectEnemySelection();
                DetectAllySelection();

            }
            else if (Input.GetMouseButtonDown(1))
            {
                DetectCardUnselection();
                DetectHerbUnselection();
            }

            if (SelectedCard != null)
                UpdateTargetingArrow();
            else if (selectedHerb != null)
                UpdateHerbTargetingArrow();
        }

        private void DetectCardSelection()
        {
            // Checks if the player clicked over a card.
            var mousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hitInfo = Physics2D.Raycast(mousePos, Vector3.forward, Mathf.Infinity, CardLayer);
            if (hitInfo.collider != null)
            {
                var card = hitInfo.collider.GetComponent<CardObject>();
                var cardTemplate = card.Template;
                if (CardUtils.CardCanBePlayed(cardTemplate, PlayerMana) &&
                    CardUtils.CardHasTargetableEffect(cardTemplate))
                {
                    SelectedCard = hitInfo.collider.gameObject;
                    SelectedCard.GetComponent<SortingGroup>().sortingOrder += 10;
                    prevClickPos = mousePos;
                }
            }
        }

        private void DetectCardUnselection()
        {
            if (SelectedCard != null)
            {
                var card = SelectedCard.GetComponent<CardObject>();
                SelectedCard.transform.DOKill();
                card.Reset(() =>
                {
                    SelectedCard = null;
                    isArrowCreated = false;
                });

                targetingArrow.EnableArrow(false);
            }
        }

        private void DetectEnemySelection()
        {
            if (SelectedCard != null)
            {
                // Checks if the player clicked over an enemy.
                var mousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
                var hitInfo = Physics2D.Raycast(mousePos, Vector3.forward, Mathf.Infinity, EnemyLayer);
                if (hitInfo.collider != null)
                {
                    selectedEnemy = hitInfo.collider.gameObject;
                    PlaySelectedCard();

                    SelectedCard = null;

                    isArrowCreated = false;
                    targetingArrow.EnableArrow(false);
                }
            }
        }

        
        private void UpdateTargetingArrow()
        {
            var mousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            var diffY = mousePos.y - prevClickPos.y;
            if (!isArrowCreated && diffY > CardSelectionDetectionOffset)
            {
                isArrowCreated = true;

                var pos = SelectedCard.transform.position;

                SelectedCard.transform.DOKill();
                var seq = DOTween.Sequence();
                seq.AppendCallback(() =>
                    {
                        SelectedCard.transform.DOMove(new Vector3(0, SelectedCardYOffset, pos.z),
                            CardSelectionAnimationTime);
                        SelectedCard.transform.DORotate(Vector3.zero, CardSelectionAnimationTime);
                    });
                seq.AppendInterval(0.05f);
                //草药系统重构
                targetingArrow.startpos.x = 0;
                targetingArrow.startpos.y = -4;
                //
                seq.OnComplete(() => { targetingArrow.EnableArrow(true); });
            }
        }

        protected override void PlaySelectedCard()
        {
            base.PlaySelectedCard();

            //TODO:效果执行对象待改为由箭头的ui交互指定的目标
            var card = SelectedCard.GetComponent<CardObject>().RuntimeCard;
            EffectResolutionSystem.ResolveCardEffects(card, Enemy.Character);
        }

        public bool IsArrowActive()
        {
            return isArrowCreated;
        }
        //草药选择系统增加功能
        private void DetectHerbSelection()
        {
            // Checks if the player clicked over a card.
            var mousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hitInfo = Physics2D.Raycast(mousePos, Vector3.forward, Mathf.Infinity, herbLayer);
            if (hitInfo.collider != null)
            {
                selectedHerb = hitInfo.collider.gameObject;
                prevClickPos = mousePos;
            }
        }
        private void DetectHerbUnselection()
        {
            if (selectedHerb != null)
            {
                var herb = selectedHerb.GetComponent<HerbObject>();
                herb.Reset();
                selectedHerb = null;

                isArrowCreated = false;
                targetingArrow.EnableArrow(false);
            }
        }

        private void DetectAllySelection()
        {
            if (selectedHerb != null)
            {
                // Checks if the player clicked over an Ally
                var mousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
                var hitInfo = Physics2D.Raycast(mousePos, Vector3.forward, Mathf.Infinity, allyLayer);
                if (hitInfo.collider != null)
                {
                    //TODO：拓展为卡牌也可对友军使用 草药也可对敌军使用
                    selectedAlly = hitInfo.collider.gameObject;
                    if (selectedHerb != null) PlaySelectedHerb();

                    selectedHerb = null;

                    isArrowCreated = false;
                    targetingArrow.EnableArrow(false);
                }
            }
        }
        protected void PlaySelectedHerb()
        {
            var herbObject = selectedHerb.GetComponent<HerbObject>();
            var herbTemplate = herbObject.Template;

            //TODO:效果执行对象待改为由箭头的ui交互指定的目标
            EffectResolutionSystem.ResolveEffects(herbTemplate.Effects, Player.Character);
            herbObject.Reduce();
        }
        private void UpdateHerbTargetingArrow()
        {
            var mousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            var diffY = mousePos.y - prevClickPos.y;
            if (!isArrowCreated && diffY < herbSelectionDetectionOffset)
            {
                isArrowCreated = true;
                Transform transform = selectedHerb.GetComponent<Transform>();
                targetingArrow.startpos = transform.position;
                targetingArrow.EnableArrow(true); 
            }
        }    
    }
}
