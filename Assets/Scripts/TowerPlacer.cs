using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class TowerPlacer : MonoBehaviour
    {
        [SerializeField]
        private Camera _mainCamera;

        [SerializeField]
        private Tower _testTower;
        
        private Tower _towerToPlace;
        private Tower _selectedTower;

        private void LateUpdate()
        {
            if (_towerToPlace != null) {
                var position = Mouse.current.position.ReadValue();
                position = _mainCamera.ScreenToWorldPoint(position);
                _towerToPlace.transform.position = position;

                if (_towerToPlace.CheckCanPlace(position)) {
                    if(Mouse.current.leftButton.wasPressedThisFrame) {
                        PlaceTower(position);
                    }
                }
                
                if (Mouse.current.rightButton.wasPressedThisFrame) {
                    CancelTower();
                }
            }
        }
        
        public void SetTowerToPlace(Tower tower)
        {
            if (_towerToPlace != null) {
                CancelTower();
            }
            
            Machine.AddMoney(-tower.Stats.Cost);
            _towerToPlace = tower;
            _towerToPlace.PrepareForPlacing();
        }

        public void PlaceTower(Vector3 position)
        {
            if (_towerToPlace == null)
                return;
            
            _towerToPlace.Place(position);
            _towerToPlace = null;
        }

        public void CancelTower()
        {
            if(_towerToPlace != null) {
                Machine.AddMoney(_towerToPlace.Stats.Cost);
                Destroy(_towerToPlace.gameObject);
                _towerToPlace = null;
            }
        }

        public void SetSelectedTower(Tower tower)
        {
            if (_selectedTower != null) {
                _selectedTower.Place(_selectedTower.transform.position);
            }
            
            _selectedTower = tower;
        }
    }
}