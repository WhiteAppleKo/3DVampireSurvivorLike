using UnityEngine;

namespace Features.ExpCristal
{
    public class ExpCristalVisualizer : MonoBehaviour, IExpCristalVisualizer
    {
        public Vector3 Position 
        { 
            get => transform.position; 
            set => transform.position = value; 
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
