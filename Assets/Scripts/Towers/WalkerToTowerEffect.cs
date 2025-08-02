using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace.Towers
{
    public struct WalkerToTowerEffectData
    {
        public Walker Walker;
        public Tower Tower;
        public bool StartAtWalker;
        
        public Action<Walker, Tower> OnComplete;
    }
    
    public class WalkerToTowerEffect : Effect<WalkerToTowerEffectData>
    {
        [SerializeField]
        private Animation _animation;

        [SerializeField]
        private string _resetClip;

        [SerializeField]
        private string _playClip;

        [SerializeField]
        private float _moveTime;
        
        [SerializeField]
        private AnimationCurve _moveCurve;

        [SerializeField]
        private AnimationCurve _offsetCurve;
        
        [SerializeField]
        private Vector2 _offset = new Vector2(0, 2f);

        public override void Reset()
        {
            base.Reset();
            if (string.IsNullOrEmpty(_resetClip))
                return;
            
            _animation.Play(_resetClip);
        }

        public override IEnumerator Run(WalkerToTowerEffectData data)
        {
            var startPos = data.StartAtWalker
                ? transform.position = data.Walker.transform.position
                : transform.position = data.Tower.DooberAnchor.position;
            
            var endPos = data.StartAtWalker 
                ? data.Tower.DooberAnchor.position
                : data.Walker.transform.position;
            
            
            
            _animation.Play(_playClip);
            while (_animation.IsPlaying(_playClip)) {
                transform.position = Vector3.Lerp(startPos, endPos, _moveCurve.Evaluate(_moveTime)) 
                                     + Vector3.Lerp(Vector3.zero, _offset, _offsetCurve.Evaluate(_moveTime));
                yield return null;
            }

            data.OnComplete?.Invoke(data.Walker, data.Tower);
        }
    }
}