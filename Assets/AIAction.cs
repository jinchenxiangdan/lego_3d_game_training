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
    private Vector3 last_position;

    // Start is called before the first frame update
    void Start()
    {
        this.status = 0;
        this.min_distance = 10;
        this.last_position = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        // if status is follow, follow the player
        if (this.status == 1) {
            // get players transform, if it's far away, move to player
            Transform m_PlayerTransform = player.transform;
            Debug.Log("player's location:" + m_PlayerTransform);
            Debug.Log("current location:" + this.last_position);
            float distance = (Vector3.Distance (current_object.transform.position, player.transform.position));

            Debug.Log("player's distance:" + distance);
            if (distance > this.min_distance) {
                current_object.transform.Translate(( player.transform.position - current_object.transform.position).normalized * Time.deltaTime);
                if (Vector3.Distance(this.last_position, current_object.transform.position) < 0.8) {
                    current_object.transform.Translate( Vector3.up * Time.deltaTime, Space.World);
                }
                
            }
            // float distance = Vector3.Distance (object1.transform.position, object2.transform.position);
            this.last_position = current_object.transform.position;

        }
        
    }
    

    void Stop()
    {

    }

    void FollowMainCharacter()
    {
        
    }
}
