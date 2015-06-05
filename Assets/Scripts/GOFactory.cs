using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GOF
{
    public class GOFactory : MonoBehaviour
    {
        private Dictionary<string, GOFactoryMachine> machines;

        [SerializeField]
        private GameObject bite;

        #region public_interface
        public GameObject spawn(string prefabName)
        {
            if (machines.ContainsKey(prefabName))
            {
                return machines[prefabName].createModel(generateId());
            }
            else
            {
                GameObject obj = Resources.Load(prefabName) as GameObject;
                if (obj != null)
                {
                    createMachine(prefabName);
                    return machines[prefabName].createModel(generateId());
                }
            }

            Debug.LogError("[GOF]Couldn't find any prefab named " + prefabName + ". Returning null.");
            return null;
        }

        public void createMachine(string machineName, params GOFactoryOption[] options)
        {
            if (!machines.ContainsKey(machineName))
            {
              var m = new GOFactoryMachine();
              m.ModelName = machineName;
              m.Factory = this;
              setMachineOptions(m, options);
              machines.Add(machineName, m);
            }
            else
            {
                Debug.LogError("[GOF]There is already a machine responsible for prefabs with this name : " + machineName + ". Please call 'configureMachine' if you want to change its configuration.");
            }
        }

        public void configureMachine(string machineName, params GOFactoryOption[] options)
        {
            if (machines.ContainsKey(machineName))
                setMachineOptions(machines[machineName], options);
            else
            {
                Debug.LogError("[GOF]You're trying to configure a machine that does not exists : " + machineName + ". Please call 'createMachine' first.");
            }
        }

        public static GOFactoryOption Position(Vector3 v3)
        {
            return new GOFactoryOption(GOFactoryOptionEnum.position, v3);
        }
        public static GOFactoryOption Prefab(GameObject obj)
        {
            return new GOFactoryOption(GOFactoryOptionEnum.prefab, obj);
        }
        public static GOFactoryOption InactiveLifeSpan(float f)
        {
            return new GOFactoryOption(GOFactoryOptionEnum.inactiveLifeSpan, f);
        }
        public static GOFactoryOption ActiveLifeSpan(float f)
        {
            return new GOFactoryOption(GOFactoryOptionEnum.activeLifeSpan, f);
        }

        #endregion

        private string generateId()
        {
            return Guid.NewGuid().ToString();
        }

        void Start()
        {
            machines = new Dictionary<string, GOFactoryMachine>();
            spawn("bite");
            createMachine("huk", GOFactory.Prefab(bite), GOFactory.Position(new Vector3(10,5,1)), GOFactory.InactiveLifeSpan(1));
            var obj = spawn("huk");
            obj.GetComponent<GOFactoryTracker>().Active = false;
        }

        public void startChildCoroutine(IEnumerator coroutineMethod)
        {
            StartCoroutine(coroutineMethod);
        }

        #region options

        private void setMachineOptions(GOFactoryMachine m, params GOFactoryOption[] options)
        {
            foreach (GOFactoryOption option in options)
            {
                switch(option.type)
                {
                    case GOFactoryOptionEnum.position:
                        m.DefaultPos = option.v3;
                        break;
                    case GOFactoryOptionEnum.prefab:
                        m.Prefab = option.obj;
                        break;
                    case GOFactoryOptionEnum.inactiveLifeSpan:
                        m.InactivityTimeBeforeDestroy = option.f;
                        break;
                    case GOFactoryOptionEnum.activeLifeSpan:
                        m.ActivityTimeBeforeDestroy = option.f;
                        break;
                    default:
                        break;
                }
            }
        }

        public class GOFactoryOption
        {
            public GOFactoryOptionEnum type;
            public Vector3 v3;
            public GameObject obj;
            public float f;

            public GOFactoryOption(GOFactoryOptionEnum t, Vector3 v) { type = t; v3 = v; }
            public GOFactoryOption(GOFactoryOptionEnum t, GameObject o) { type = t; obj = o; }
            public GOFactoryOption(GOFactoryOptionEnum t, float fl) { type = t; f = fl; }
        }

        public enum GOFactoryOptionEnum
        {
            position = 0,
            prefab = 1,
            inactiveLifeSpan = 2,
            activeLifeSpan = 3
        }
        #endregion
    }

}
