using UnityEngine;

namespace DefaultNamespace
{
    public class AudioTrigger : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _source;
        
        public void PlaySound(AudioClip clip)
        {
            _source.PlayOneShot(clip);
        }
    }
}