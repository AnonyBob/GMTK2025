using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Status", menuName = "ScriptableObject/Status", order = 1)]
    public abstract class Status : ScriptableObject
    {
        public float Duration;
        
        public abstract void OnAdded(Walker walker);

        public abstract void OnUpdate(Walker walker, float deltaTime);
        
        public abstract void OnRemoved(Walker walker);
    }
}