using UnityEngine;

namespace DefaultNamespace
{
    public class ShadowSprite : MonoBehaviour
    {
        [SerializeField]
        private Color _shadowColor = new Color(0, 0, 0, 0.5f); // Semi-transparent black
        
        [SerializeField]
        private Vector2 _shadowOffset = new Vector2(2f, -2f); // Offset for the shadow effect
        
        private void Start()
        {
            if (transform.parent.GetComponent<ShadowSprite>()) {
                return;
            }
            
            var copy = Instantiate(gameObject, transform);
            var spriteRenderer = copy.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) {
                // Set the color to a semi-transparent black for shadow effect
                spriteRenderer.color = new Color(0, 0, 0, 0.5f);
                copy.transform.position = transform.position + (Vector3)_shadowOffset;
                
                // Optionally, you can adjust the sorting order to ensure it appears behind other sprites
                spriteRenderer.sortingOrder = -1;
            }
            else {
                Debug.LogWarning("No SpriteRenderer found on this GameObject.");
            }
        }
    }
}