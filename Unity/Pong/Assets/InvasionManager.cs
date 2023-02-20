using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvasionManager : MonoBehaviour
{
    public GameObject blockSpawn;
    public GameObject block;
    public GameObject parent;
    private float offset = 0.45f;
    private Camera cam;
    public block b;
    public float distance = 0;
    private bool init = false;
    private Vector3 spawnPos;
    public bool paused = false;
    private float _Spawndistance;
    // Start is called before the first frame update
    void Start()
    {
        paused = false;
        _Spawndistance = Screen.width / 6;
        cam = Camera.main;
        Vector3 off = cam.ScreenToWorldPoint(block.GetComponent<SpriteRenderer>().bounds.size);
        offset =  - off.x/2;
        Debug.Log("Offset: " + offset);
        spawnPos = blockSpawn.transform.position;
    } 

    // Update is called once per frame
    void Update() 
    {
        Debug.Log(distance);

        if (init)
        {
            distance += b.moveSpeed * Time.deltaTime;
            if (distance >= offset && paused == false)
            {
                spawnRow();
                distance = 0;
            }
        }

        //else distance = 0;
    }

    public void spawnRow()
    {
        Debug.Log(("Spawning Row"));
        var blockWidth = block.GetComponent<SpriteRenderer>().bounds.size.x;
        for (int i = 0; i < 6; i++)
        {
            Instantiate(block, cam.ScreenToWorldPoint(new Vector3( _Spawndistance * i + blockWidth/2 + (_Spawndistance - blockWidth)/2,  Screen.height -300, spawnPos.z)), new Quaternion(0, 0, 0, 0),parent.transform);
        }
        b = GameObject.FindWithTag("Block").GetComponent<block>();
        paused = false;
        init = true;
    }
}
