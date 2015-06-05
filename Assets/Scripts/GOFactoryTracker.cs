using UnityEngine;
using System.Collections;

namespace GOF
{
    public class GOFactoryTracker : MonoBehaviour
    {
        public string Id;

        private float inactivityTimeBeforeDestroy;
        public float InactivityTimeBeforeDestroy
        {
            set { inactivityTimeBeforeDestroy = value; }
        }

        protected bool isActive;
        public bool Active
        {
            get { return isActive; }
            set
            {
                isActive = value;
                activate(isActive);
            }
        }

        void Start()
        {

        }

        protected GOFactoryMachine machine;
        public GOFactoryMachine Machine
        {
            set { machine = value; }
        }

        protected virtual void activate(bool b)
        {
            hide(b);
        }

        protected void hide(bool b)
        {
            gameObject.SetActive(b);
            if (!b)
                transform.position = new Vector3(0, 1000, 0);
        }
    }
}