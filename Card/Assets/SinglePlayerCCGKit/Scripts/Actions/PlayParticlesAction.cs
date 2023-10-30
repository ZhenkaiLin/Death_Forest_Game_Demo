// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CCGKit
{
    public class PlayParticlesAction : EffectAction
    {
        public ParticleSystem Particles;
        public Vector3 Offset;
        public float duration = 2.0f;
        //等待改
        private ParticleSystem mypartical;

        public override string GetName()
        {
            return "Play particles";
        }

        public override void Execute(GameObject go)
        {
            if (mypartical != null) return;
            var pos = go.transform.position;
            mypartical = Instantiate(Particles);
            mypartical.transform.position = pos + Offset;
            mypartical.Play();
            if(duration>=0)
            {
                var autoKill = mypartical.gameObject.AddComponent<AutoKill>();
                autoKill.Duration = duration;
            }

        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var popupStyle = new GUIStyle(EditorStyles.popup);
            popupStyle.fixedWidth = 100;
            var numberFieldStyle = new GUIStyle(EditorStyles.numberField);
            numberFieldStyle.fixedWidth = 40;

            Particles = (ParticleSystem)EditorGUI.ObjectField(rect, "Particles", Particles, typeof(ParticleSystem), false);
            rect.y += RowHeight;

            Offset.x = EditorGUI.FloatField(rect, "X", Offset.x, numberFieldStyle);
            rect.y += RowHeight;
            Offset.y = EditorGUI.FloatField(rect, "Y", Offset.y, numberFieldStyle);
            rect.y += RowHeight;
            Offset.z = EditorGUI.FloatField(rect, "Z", Offset.z, numberFieldStyle);
            rect.y += RowHeight;
            duration = EditorGUI.FloatField(rect, "duration", duration, numberFieldStyle);
        }

        public override int GetNumRows()
        {
            return 5;
        }
#endif
    }
}
