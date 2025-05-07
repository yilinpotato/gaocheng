using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public MapManager mapManager;

    // Start is called before the first frame update  
    void Start()
    {

            mapManager.GenerateMap();

    }

    // Update is called once per frame  
    void Update()
    {

    }
}
