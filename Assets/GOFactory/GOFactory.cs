using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GOFactoryMachine")]
namespace GOF
{
    public class GOFactory : MonoBehaviour
    {
        private Dictionary<string, GOFactoryMachine> machines;

        [SerializeField]
        private GOFCatalog catalog;

        #region public_interface
        public GameObject spawn(string machineName)
        {
            if (machines.ContainsKey(machineName))
            {
                return machines[machineName].createModel(generateId());
            }
            else
            {
                GameObject obj = Resources.Load(machineName) as GameObject;
                if (obj != null)
                {
                    createMachine(machineName);
                    return machines[machineName].createModel(generateId());
                }
            }

            Debug.LogError("[GOF]Couldn't find any prefab named " + machineName + ". Returning null.");
            return null;
        }

        public void createMachine(string machineName, params GOFactoryOption[] options)
        {
            if(machineName != "")
            {
                if (!machines.ContainsKey(machineName))
                {
                    var m = new GOFactoryMachine();
                    m.MachineName = machineName;
                    m.Factory = this;
                    machines.Add(machineName, m);
                    setMachineOptions(m, options);
                }
                else
                {
                    Debug.LogError("[GOF]There is already a machine responsible for prefabs with this name : " + machineName + ". Please call 'configureMachine' if you want to change its configuration.");
                }
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

        public void clearAllInstance(string name)
        {
            if(machines.ContainsKey(name))
                machines[name].clear();
        }

        public void clearAllInstance()
        {
            foreach(KeyValuePair<string, GOFactoryMachine> p in  machines)
                p.Value.clear();
        }

        public static void Recycle(GameObject obj)
        {
            obj.GetComponent<GOFactoryTracker>().recycle();
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
        public static GOFactoryOption LifeSpan(float f)
        {
            return new GOFactoryOption(GOFactoryOptionEnum.lifeSpan, f);
        }
        public static GOFactoryOption PreInstantiate(int i)
        {
            return new GOFactoryOption(GOFactoryOptionEnum.preInstantiate, i);
        }

        #endregion

        #region private
        void Start()
        {
            machines = new Dictionary<string, GOFactoryMachine>();
            loadCatalog();
        }

        private string generateId()
        {
            return Guid.NewGuid().ToString();
        }

        internal void startChildCoroutine(IEnumerator coroutineMethod)
        {
            StartCoroutine(coroutineMethod);
        }

        private void preInstantiate(string machine, int count)
        {
            for (int i = 0; i < count; ++i)
                spawn(machine).GetComponent<GOFactoryTracker>().recycle();
        }

        private void loadCatalog()
        {
            if(catalog != null)
            {
                foreach (GOFactoryMachineTemplate t in catalog.MachinesList)
                {
                    createMachine(t.MachineName, GOFactory.Prefab(t.Prefab), GOFactory.LifeSpan(t.LifeSpan), GOFactory.InactiveLifeSpan(t.InactiveLifeSpan), GOFactory.Position(t.DefaultPosition), GOFactory.PreInstantiate(t.PreInstantiateCount));
                }
            }
        }

        #endregion

        #region options

        private void setMachineOptions(GOFactoryMachine m, params GOFactoryOption[] options)
        {
            int preInstant = 0;
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
                        m.InactiveLifeSpan = option.f;
                        break;
                    case GOFactoryOptionEnum.lifeSpan:
                        m.LifeSpan = option.f;
                        break;
                    case GOFactoryOptionEnum.preInstantiate:
                        preInstant = option.i;
                        break;
                    default:
                        break;
                }
            }
            if (preInstant > 0)
                preInstantiate(m.MachineName, preInstant);
        }

        public class GOFactoryOption
        {
            public GOFactoryOptionEnum type;
            public Vector3 v3;
            public GameObject obj;
            public float f;
            public int i;

            public GOFactoryOption(GOFactoryOptionEnum t, Vector3 v) { type = t; v3 = v; }
            public GOFactoryOption(GOFactoryOptionEnum t, GameObject o) { type = t; obj = o; }
            public GOFactoryOption(GOFactoryOptionEnum t, float fl) { type = t; f = fl; }
            public GOFactoryOption(GOFactoryOptionEnum t, int inte) { type = t; i = inte; }
        }

        public enum GOFactoryOptionEnum
        {
            position = 0,
            prefab = 1,
            inactiveLifeSpan = 2,
            lifeSpan = 3,
            preInstantiate = 4
        }
        #endregion
    }

}
