using UnityEngine;

namespace DefaultNamespace
{
    [ExecuteInEditMode]
    public class MatchSpriteColor : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _watch;

        [SerializeField]
        private SpriteRenderer[] _targets;
        
        private void Update()
        {
            var color = _watch.color;
            foreach (var target in _targets) {
                if (target != null) {
                    target.color = color;
                }
            }
        }
    }
}