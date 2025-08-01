using System.Collections;
using UnityEngine;

namespace DefaultNamespace.Towers
{
    public abstract class Effect : MonoBehaviour
    {
        public virtual void Reset()
        {
            
        }
    }
    
    public abstract class Effect<Data> : Effect
    {
        public abstract IEnumerator Run(Data data);
    }
}