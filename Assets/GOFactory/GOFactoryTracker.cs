using UnityEngine;
using System;
using System.Collections;

namespace GOF
{
    public delegate void Reset();
    public class GOFactoryTracker : MonoBehaviour
    {
        [HideInInspector]
        public string Id;

        public void recycle()
        {
            gameObject.SetActive(false);
            machine.putAway(this);
        }

        protected GOFactoryMachine machine;
        public GOFactoryMachine Machine
        {
            set { machine = value; }
        }

        private Action<GameObject> resetCb;
        public Action<GameObject> Reset
        {
            set { resetCb = value; }
            get { return resetCb; }
        }

        public void resetObject()
        {
            gameObject.SetActive(true);
            if (resetCb != null)
                resetCb(gameObject);
        }
    }
}