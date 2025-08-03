using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class InfoScreen : MonoBehaviour
    {
        [SerializeField]   
        private Camera _mainCamera;

        [SerializeField]
        private RectTransform _container;

        [SerializeField]
        private RectTransform _panel;

        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _blurb;
        [SerializeField] private GameObject _runCostDisplay;
        [SerializeField] private TextMeshProUGUI _runCost;
        [SerializeField] private GameObject _lifeDisplay;
        [SerializeField] private TextMeshProUGUI _lifeRemaining;
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show(Tower tower, Transform anchor)
        {
            _title.text = tower.Stats.Name;
            _blurb.text = tower.Stats.Blurb;
            
            _runCostDisplay.SetActive(tower.Stats.RunCost > 0);
            _runCost.text = $"{tower.Stats.RunCost}/tick";
            
            _lifeDisplay.SetActive(tower.Stats.Life > 0);
            _lifeRemaining.text = $"{tower.LifeRemaining}/{tower.Stats.Life}";
            
            var screenPoint = _mainCamera.WorldToScreenPoint(anchor.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_container, screenPoint, _mainCamera,
                out var localPoint);

            var pivotX = 0f;
            if (localPoint.x > 100) {
                pivotX = 1f;
            }
            
            var pivotY = 0f;
            if (localPoint.y > 100) {
                pivotY = 1f;
            }
            
            _panel.pivot = new Vector2(pivotX, pivotY);
            _panel.anchoredPosition = localPoint;
            
            gameObject.SetActive(true);
        }
    }
}