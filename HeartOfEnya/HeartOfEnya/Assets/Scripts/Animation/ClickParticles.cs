using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickParticles : MonoBehaviour
{
    [SerializeField]
    GameObject psObject;

    [SerializeField]
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) PlayClickParticles();
    }

    public void PlayClickParticles()
    {
       
       Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
       pos = new Vector3(pos.x, pos.y, 0);
       Instantiate(psObject, pos, new Quaternion());
    }
}
