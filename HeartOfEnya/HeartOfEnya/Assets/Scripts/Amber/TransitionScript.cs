using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionScript : MonoBehaviour
{
    public static TransitionScript main;
    // Start is called before the first frame update
    void Awake()
    {
            if (main == null)
            {
                main = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

        }

    // Update is called once per frame
    void Update()
    {
        
    }
}
