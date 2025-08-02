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
        private Tower _towerToPlaceInstance;

        private void LateUpdate()
        {
            if (_towerToPlaceInstance != null) {
                var position = Mouse.current.position.ReadValue();
                position = _mainCamera.ScreenToWorldPoint(position);
                _towerToPlaceInstance.transform.position = position;

                if (_towerToPlaceInstance.CheckCanPlace(position)) {
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
            if (_towerToPlaceInstance != null) {
                CancelTower();
            }
            
            Machine.AddMoney(-tower.Stats.Cost);
            _towerToPlace = tower;
            _towerToPlaceInstance = Instantiate(tower, transform);
            _towerToPlaceInstance.PrepareForPlacing();
        }

        public void PlaceTower(Vector3 position)
        {
            if (_towerToPlace == null)
                return;
            
            _towerToPlaceInstance.Place(position);
            _towerToPlaceInstance = null;
            
            if (Machine.CanAfford(_towerToPlace)) {
                SetTowerToPlace(_towerToPlace);
            }
            else {
                _towerToPlace = null;
            }
        }

        public void CancelTower()
        {
            if(_towerToPlace != null) {
                Machine.AddMoney(_towerToPlace.Stats.Cost);
                Destroy(_towerToPlaceInstance.gameObject);
                _towerToPlace = null;
                _towerToPlaceInstance = null;
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