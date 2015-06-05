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
                activate();
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

        protected virtual void activate()
        {
          gameObject.SetActive(isActive);
          if(!isActive)
            machine.putAway(this);
        }
    }
}