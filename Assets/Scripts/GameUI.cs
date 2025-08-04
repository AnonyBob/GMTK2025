using UnityEngine;

namespace DefaultNamespace
{
    public class GameUI : MonoBehaviour
    {
        public GameObject Orientation;

        public GameObject Win;

        public GameObject Lose;
        
        public void CloseOrientation()
        {
            Orientation.SetActive(false);
        }

        public void WinGame()
        {
            StopGame();
            Win.SetActive(true);
        }

        public void LoseGame()
        {
            StopGame();   
            Lose.SetActive(true);
        }

        public void Restart()
        {
            
        }
        
        public void Quit()
        {
            Application.Quit();
        }

        private void StopGame()
        {
            var manager = FindFirstObjectByType<WalkerManager>();
            manager.enabled = false;
        }
    }
}