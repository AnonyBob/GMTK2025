using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace DefaultNamespace
{
    public class Catalogue : MonoBehaviour
    {
        private static readonly int OpenBool = Animator.StringToHash("Open");

        [SerializeField]
        private CatalogueItem _itemPrefab;

        [SerializeField]
        private RectTransform _itemContainer;

        [SerializeField] 
        private Animator _animator;
        
        [field: SerializeField]
        public RectTransform DragParent { get; private set; }

        [field: SerializeField]
        public float ActivateThreshold { get; private set; } = 10f;

        private readonly List<CatalogueItem> _items = new List<CatalogueItem>();

        private void Start()
        {
            _animator.SetBool(OpenBool, false);
            _animator.Play("Close", 0, 1);
        }
        
        public void Open(List<Tower> towers)
        {
            _animator.SetBool(OpenBool, true);
            foreach(var item in _items) {
                Destroy(item.gameObject);
            }
            _items.Clear();
            
            foreach(var tower in towers) {
                CreateItem(tower);
            }
        }

        public void Close()
        {
            _animator.SetBool(OpenBool, false);
        }
        
        public void CreateItem(Tower tower)
        {
            var item = Instantiate(_itemPrefab, _itemContainer);
            item.SetTower(tower);
            
            _items.Add(item);
        }
        
        public void ActivateTower(Tower tower)
        {
            var placer = FindFirstObjectByType<TowerPlacer>();
            if (placer != null) {
                var instance = Instantiate(tower, placer.transform);
                placer.SetTowerToPlace(instance);
            }

            Close();
        }
    }
}