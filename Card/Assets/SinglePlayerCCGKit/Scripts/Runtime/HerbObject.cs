using DG.Tweening;
using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace CCGKit
{
    /// <summary>
    /// This component is linked to the actual GameObjects corresponding to the cards that
    /// are in the player's hand.
    /// </summary>
    public class HerbObject : MonoBehaviour
    {
        //����UI
#pragma warning disable 649
        [SerializeField]
        private TextMeshPro numText;

        [SerializeField]
        private SpriteRenderer picture;
        [SerializeField]
        private SpriteRenderer glow;

#pragma warning restore 649
        //��������
        public HerbTemplate Template;

        //�����Ƿ�ɽ���
        private bool interactable=true;

        private CardWithArrowSelectionSystem cardWithArrowSelectionSystem;

        private int num;

        private bool beingHighlighted;
        private bool beingUnhighlighted;

        //TODO��������Ϸ�����Բ�ҩ�������ı� дherbsystem����boost
        private void Awake()
        {
            SetGlowEnabled(false);
        }

        private void Start()
        {
            //TODO:��ϵͳ
            cardWithArrowSelectionSystem = FindObjectOfType<CardWithArrowSelectionSystem>();
        }


        public void SetInfo(HerbLibraryEntry herbEntry)
        {
            Template = herbEntry.herb;
            num = herbEntry.numCopies;
            numText.text = num.ToString();
            glow.sprite = Template.Glow;
            glow.color = Template.GlowColor;
            picture.material = Template.Material;
            picture.sprite = Template.Picture;
        }
        public void Reduce()
        {
            num--;
            numText.text = num.ToString();
            if (num==0)
            {
                //����
                DestroyImmediate(gameObject);
            }
        }

        public void SetGlowEnabled(bool glowEnabled)
        {
            glow.enabled = glowEnabled;
        }

        public void SetGlowEnabled(int playerMana)
        {
            glow.enabled = playerMana >0;
        }

        public bool IsGlowEnabled()
        {
            return glow.enabled;
        }

        public void SetInteractable(bool value)
        {
            interactable = value;
        }

        private void OnMouseEnter()
        {
            if (interactable)
            {
                HighlightHerb();
            }
        }

        private void OnMouseExit()
        {
            if (interactable)
            {
                UnHighlightHerb();
            }
        }

        public void HighlightHerb()
        {
            if (cardWithArrowSelectionSystem.HasSelectedCard() )
            {
                return;
            }

            if (beingHighlighted)
            {
                return;
            }

            beingHighlighted = true;
            SetGlowEnabled(true);
        }

        public void UnHighlightHerb()
        {
            if (cardWithArrowSelectionSystem.HasSelectedCard() )
            {
                return;
            }

            if (!beingHighlighted)
            {
                return;
            }

            beingHighlighted = false;
            SetGlowEnabled(false);
        }
        public void Reset()
        {
            beingHighlighted = false;
            SetGlowEnabled(false);
        }
    }
}
