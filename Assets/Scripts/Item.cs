using UnityEngine;

namespace DefaultNamespace
{
    public class Item : MonoBehaviour
    {
        [field: SerializeField]
        public ItemType Type { get; private set; }
        
        [field: SerializeField]
        public Status[] Statuses { get; private set; }
    }
}