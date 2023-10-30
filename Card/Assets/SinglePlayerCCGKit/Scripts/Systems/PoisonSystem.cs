using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    public class PoisonSystem : MonoBehaviour
    {
        // Start is called before the first frame update
        public Transform pivot;
        public GameObject poisonPrefab;
        private GameObject poison;
        public void OnEventRaised(StatusTemplate status, int value)
        {
            if(status.name=="Poison")
            {
                if(value>0)
                {
                    if(poison==null)
                    {
                        poison = Instantiate(poisonPrefab);
                        Transform transform = poison.GetComponent<Transform>();
                        transform.position = pivot.position;
                    }

                }
                else
                {
                    if (poison != null)
                        DestroyImmediate(poison);
                }
            }
        }
    }
}

