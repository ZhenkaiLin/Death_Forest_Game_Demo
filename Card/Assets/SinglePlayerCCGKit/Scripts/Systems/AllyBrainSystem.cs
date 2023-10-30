// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// This system is responsible for managing the AI of an enemy character, which mostly
    /// consists on following the patterns defined in the character's template.
    /// </summary>
    public class AllyBrainSystem : BaseSystem
    {
#pragma warning disable 649



#pragma warning restore 649

        private int currentPatternIdx;
        private int currentCount;

        private bool isThinking;
        private float accTime;
        private const float ThinkingTime = 1.0f;

        private List<Effect> currentEffects;
        //同伴系统新增:为了不同同伴意图显示独立 不被同一个事件触发 且可拓展
        //每个同伴gameobject上挂载一个AllyBrainSystem 拥有一个独立的事件
        //并再gameboost创建同伴widget时创建一个监听器并绑定事件 并在PlayerTurnBegan时触发

        public IntentChangeEvent intentChangeEvent;
        public EffectResolutionSystem effectResolutionSystem;
        public void OnPlayerTurnBegan()
        {
            var template = GetComponent<CharacterObject>().Template as AllyTemplate;

            if (currentPatternIdx >= template.Patterns.Count)
                currentPatternIdx = 0;

            Sprite sprite = null;
            var pattern = template.Patterns[currentPatternIdx];
            if (pattern is RepeatPattern repeatPattern)
            {
                currentCount += 1;
                if (currentCount == repeatPattern.Times)
                {
                    currentCount = 0;
                    currentPatternIdx += 1;
                }

                currentEffects = pattern.Effects;
                sprite = repeatPattern.Sprite;
            }
            else if (pattern is RepeatForeverPattern repeatForeverPattern)
            {
                currentEffects = pattern.Effects;
                sprite = repeatForeverPattern.Sprite;
            }
            else if (pattern is RandomPattern randomPattern)
            {
                var effects = new List<int>(100);
                var idx = 0;
                foreach (var probability in randomPattern.Probabilities)
                {
                    var amount = probability.Value;
                    for (var i = 0; i < amount; i++)
                    {
                        effects.Add(idx);
                    }

                    idx += 1;
                }

                var randomIdx = Random.Range(0, 100);
                var selectedEffect = randomPattern.Effects[effects[randomIdx]];
                currentEffects = new List<Effect> { selectedEffect };

                for (var i = 0; i < pattern.Effects.Count; i++)
                {
                    var effect = pattern.Effects[i];
                    if (effect == selectedEffect)
                    {
                        sprite = randomPattern.Probabilities[i].Sprite;
                        break;
                    }
                }

                currentPatternIdx += 1;
            }

            var currentEffect = currentEffects[0];
            intentChangeEvent.Raise(sprite, (currentEffect as IntegerEffect).Value);
        }

        public void OnAllyTurnBegan()
        {
            isThinking = true;
        }

        private void Update()
        {
            if (isThinking)
            {
                accTime += Time.deltaTime;
                if (accTime >= ThinkingTime)
                {
                    isThinking = false;
                    accTime = 0.0f;

                    effectResolutionSystem.ResolveEnemyEffects(currentEffects);

                }
            }
        }
    }
}
