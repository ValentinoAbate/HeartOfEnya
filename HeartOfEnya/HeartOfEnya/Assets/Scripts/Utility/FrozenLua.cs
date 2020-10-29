using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenLua : MonoBehaviour
{
    public void Defrost()
    {
        var anim = GetComponent<Animator>();
        anim.Play("FadeOut");
    }
    // Start is called before the first frame update
    void Start()
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        gameObject.SetActive(pData.luaBossPhase2Defeated && !pData.LuaUnfrozen);
    }
}
