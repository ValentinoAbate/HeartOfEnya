using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FMODUnity.StudioGlobalParameterTrigger))]
public class UISliderSetGlobalFMODParam : MonoBehaviour
{
    private FMODUnity.StudioGlobalParameterTrigger param;

    private void Awake()
    {
        param = GetComponent<FMODUnity.StudioGlobalParameterTrigger>();
    }
    public void OnValueChange(float val)
    {
        param.value = val;
        param.TriggerParameters();
    }
}
