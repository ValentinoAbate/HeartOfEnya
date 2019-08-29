using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUtils.Shuffle;

public class RandomTest : MonoBehaviour
{
    public int[] arr = {0,1,2,3,4,5,6,7,8,9};
    public List<int> list = new List<int>{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    // Start is called before the first frame update
    void Start()
    {
        arr.Shuffle();
        list.Shuffle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
