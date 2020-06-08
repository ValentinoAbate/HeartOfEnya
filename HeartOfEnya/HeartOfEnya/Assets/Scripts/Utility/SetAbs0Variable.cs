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
        storage.SetNumber("$abs0Defeated", pData.absoluteZeroPhase2Defeated ? 1 : 0);
    }
}
