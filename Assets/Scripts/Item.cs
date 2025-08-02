using UnityEngine;

namespace DefaultNamespace
{
    public class Item : MonoBehaviour
    {
        [field: SerializeField]
        public ItemType Type { get; private set; }

        [SerializeField]
        private SpriteRenderer _art;

        [SerializeField]
        private float _rotationMin = -15f;
        
        [SerializeField]
        private float _rotationMax = 15f;
        
        [field: SerializeField]
        public Status[] Statuses { get; private set; }

        private void Start()
        {
            _art.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(_rotationMin, _rotationMax));
        }
    }
}