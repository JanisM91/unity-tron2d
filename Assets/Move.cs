using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

public class MovementPlayer1 : MonoBehaviour {

    private MqttClient client;

    // Movement keys (customizable in Inspector)
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode rightKey;
    public KeyCode leftKey;

    // Movement Speed
    public float speed = 16;

    // Wall Prefab
    public GameObject wallPrefab;

    public String direction = "";

    // Current Wall
    Collider2D wall;

    // Last Wall's End
    Vector2 lastWallEnd;

    void spawnWall()
    {
        // Save last wall's position
        lastWallEnd = transform.position;

        // Spawn a new Lightwall
        GameObject g = (GameObject)Instantiate(wallPrefab, transform.position, Quaternion.identity);
        wall = g.GetComponent<Collider2D>();
    }

    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        Debug.Log("Received: " + System.Text.Encoding.UTF8.GetString(e.Message));

        if (System.Text.Encoding.UTF8.GetString(e.Message).Equals("rechts"))
        {
            direction = "rechts";
        }else if (System.Text.Encoding.UTF8.GetString(e.Message).Equals("links"))
        {
            direction = "links";
        }
    }

    void OnTriggerEnter2D(Collider2D co)
    {
        // Not the current wall?
        if (co != wall)
        {
            print("Player lost:" + name);
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        // Initial Velocity
        GetComponent<Rigidbody2D>().velocity = Vector2.up * speed;
        
        //spawn wall on start
        spawnWall();

        //create Client and establish connection
        client = new MqttClient(IPAddress.Parse("193.175.85.50"), 1883, false, null);
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        string clientId = Guid.NewGuid().ToString();
        client.Connect(clientId);
        client.Subscribe(new string[] { "tron2d/player1" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        Debug.Log("Connected...");
    }

    void fitColliderBetween(Collider2D co, Vector2 a, Vector2 b)
    {
        // Calculate the Center Position
        co.transform.position = a + (b - a) * 0.5f;

        // Scale it (horizontally or vertically)
        float dist = Vector2.Distance(a, b);
        if (a.x != b.x)
            co.transform.localScale = new Vector2(dist+1, 1);
        else
            co.transform.localScale = new Vector2(1, dist+1);
    }

    // Update is called once per frame
    void Update () {

        if(direction.Equals("rechts") )
        {
            if (GetComponent<Rigidbody2D>().velocity == Vector2.right * speed)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.down * speed;
                spawnWall();
            }
            else if (GetComponent<Rigidbody2D>().velocity == Vector2.left * speed)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.up * speed;
                spawnWall();
            }
            else if (GetComponent<Rigidbody2D>().velocity == Vector2.up * speed)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
                spawnWall();
            }
            else if (GetComponent<Rigidbody2D>().velocity == Vector2.down * speed)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
                spawnWall();
            }

        }else if (direction.Equals("links"))
        {
            if (GetComponent<Rigidbody2D>().velocity == Vector2.right * speed)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.up * speed;
                spawnWall();
            }
            else if (GetComponent<Rigidbody2D>().velocity == Vector2.left * speed)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.down * speed;
                spawnWall();
            }
            else if (GetComponent<Rigidbody2D>().velocity == Vector2.up * speed)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
                spawnWall();
            }
            else if (GetComponent<Rigidbody2D>().velocity == Vector2.down * speed)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
                spawnWall();
            }
        }
        direction = "";

        fitColliderBetween(wall, lastWallEnd, transform.position);
    }
}
