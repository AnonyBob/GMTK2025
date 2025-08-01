using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "ItemType", menuName = "ScriptableObjectItemType", order = 1)]
    public class ItemType : ScriptableObject
    {
        public Item prefab;
    }
}