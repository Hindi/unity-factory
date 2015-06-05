using UnityEngine;
using System;
using System.Collections;

[Serializable()] 
public class GOFactoryMachineTemplate
{
    [Tooltip("The name of the machine or the prefab.")]
    public string MachineName = "";

    [Tooltip("The number of instances spawned on start.")]
    public int PreInstantiateCount;

    [Tooltip("If no prefab is set, the factory will instantiate objects from the Resources folders using the machine name.")]
    public GameObject Prefab;

    [Tooltip("Time before the object is destroyed after it is spawned. 0 = infinite")]
    public int LifeSpan;

    [Tooltip("Time before the object is destroyed when recycled. 0 = infinite")]
    public int InactiveLifeSpan;

    [Tooltip("The position where the objects are spawned.")]
    public Vector3 DefaultPosition;

    public bool folded = true;
}
