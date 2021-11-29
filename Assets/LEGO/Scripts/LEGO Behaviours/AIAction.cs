using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAction : MonoBehaviour
{
    public int status;
    // Status: 
    // 0: stay
    // 1: follow 
    [SerializeField]
    public GameObject player;
    public GameObject current_object;


    private int min_distance;
    private int min_spped;
    private Vector3 last_position;

    // test
    // int test;

    // Start is called before the first frame update
    void Start()
    {
        this.status = 0;
        this.min_distance = 10;
        this.min_spped = 2;
        this.last_position = new Vector3();

        // test = 0;
        // PutFruit();
    }


    // Update is called once per frame
    void Update()
    {
        // if status is follow, follow the player
        // GameObject current_object = GameObject.Find("char_move");

        switch (this.status)
        {
            case 0:
                // do nothing (stop)
                break;
            case 1: 
                FollowMainCharacter();
                break;
            case 3:
                // if (test == 0) {
                //     PutFruit();
                //     test = 1;
                // }
                break;
            default:
                break;
        }
    }

    void Stop()
    {

    }

    void FollowMainCharacter()
    {
        // get players transform, if it's far away, move to player
        Transform m_PlayerTransform = player.transform;
        Debug.Log("player's location:" + m_PlayerTransform);
        Debug.Log("current location:" + this.last_position);
        float distance = (Vector3.Distance (current_object.transform.position, player.transform.position));

        Debug.Log("player's distance:" + distance);
        if (distance > this.min_distance) {
            current_object.transform.Translate(( player.transform.position - current_object.transform.position).normalized * Time.deltaTime * min_spped);
            if (Vector3.Distance(this.last_position, current_object.transform.position) < 0.8) {
                current_object.transform.Translate( Vector3.up * Time.deltaTime, Space.World);
            }
            
        }
        // float distance = Vector3.Distance (object1.transform.position, object2.transform.position);
        this.last_position = current_object.transform.position;
    }

    // private void PutFruit() 
    // {
    //     Vector3 offset = new Vector3(0, 10, 2);
    //     // instance_object.name = "Fruit";
    //     Vector3 position = player.transform.position + offset;

    //     Instantiate(fruit, position, Quaternion.identity);
    // }  
}
