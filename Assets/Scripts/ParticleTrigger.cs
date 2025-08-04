using UnityEngine;

namespace DefaultNamespace
{
    public class ParticleTrigger : MonoBehaviour
    {
        [System.Serializable]
        private struct ParticleData
        {
            public string Name;
            public ParticleSystem System;
            public Vector2 Offset;
        }
        
        [SerializeField]
        private ParticleData[] _systems;

        public void PlaySystem(string systemName)
        {
            foreach (var system in _systems) {
                if (system.Name == systemName) {
                    if (system.System.gameObject.scene.rootCount == 0) {
                        Instantiate(system.System, transform.position + (Vector3)system.Offset, Quaternion.identity);
                    }
                    else {
                        system.System.Play();
                    }
                    break;
                }
            }
        }
    }
}