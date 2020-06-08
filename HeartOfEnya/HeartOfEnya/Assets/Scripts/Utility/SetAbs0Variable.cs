using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAbs0Variable : MonoBehaviour
{
    public ExampleVariableStorage storage;
    // Start is called before the first frame update
    void Awake()
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        var abs0Val = new Yarn.Value(pData.absoluteZeroPhase2Defeated ? 1 : 0);
        storage.SetValue("$abs0Defeated", abs0Val);
    }
}
