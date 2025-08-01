using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace DefaultNamespace.Towers
{
    public class EffectPool : MonoBehaviour
    {
        private readonly Dictionary<Effect, ObjectPool<Effect>> _effectPools = 
            new Dictionary<Effect, ObjectPool<Effect>>();

        public Effect GetEffect(Effect prefab)
        {
            if (!_effectPools.TryGetValue(prefab, out var pool)) {
                pool = new ObjectPool<Effect>(() => CreateEffect(prefab), actionOnDestroy: DestroyEffect);
                _effectPools[prefab] = pool;
            }

            var effect = pool.Get();
            effect.Reset();
            return effect;
        }

        public void ReturnEffect(Effect effect)
        {
            if (effect == null)
                return;
            
            if (_effectPools.TryGetValue(effect, out var pool)) {
                pool.Release(effect);
            }
            else {
                DestroyEffect(effect);
            }
        }

        public void RunEffect<TData>(Effect<TData> effect, TData data)
        {
            StartCoroutine(DoRunEffect(effect, data));
        }

        private IEnumerator DoRunEffect<TData>(Effect<TData> effect, TData data)
        {
            yield return effect.Run(data);
            ReturnEffect(effect);
        }
        
        private Effect CreateEffect(Effect prefab)
        {
            var effect = Instantiate(prefab, transform);
            return effect;
        }
        
        private void DestroyEffect(Effect effect)
        {
            if (effect == null) 
                return;
            Destroy(effect.gameObject);
        }
        
        private void OnDestroy()
        {
            foreach(var pool in _effectPools.Values) {
                pool.Dispose();
            }
        }
    }
}