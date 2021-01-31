using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using System;

public class Player : MonoBehaviourPun 
{
    [SerializeField] float speed;
    [SerializeField] public TMP_InputField field;
    [SerializeField] GameObject speechBubble;
    [SerializeField] AudioSource screamSource; 

    
    public TextMeshProUGUI speechBubbleText; 
    public static Player LocalPlayer;

    private Rigidbody2D body;
    private Camera mainCam;
    public string myMessage;


    private void OnEnable()
    {
         field.onValueChanged.AddListener(UpdateMessages);
    }

    private void OnDisable()
    {
        field.onValueChanged.RemoveAllListeners();
    }
    private void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayer = gameObject.GetComponent<Player>();
            LocalPlayer.gameObject.name = "LocalPlayer";
            var cameraWork = GetComponent<CameraWork>();
            cameraWork.OnStartFollowing();
        }
        else
        {
            Main.otherPlayer = gameObject.GetComponent<Player>();
            Main.otherPlayer.gameObject.name = "OtherPlayer";
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (speechBubble.activeSelf) return; 
        if(other.gameObject.tag == "Player")
        {
            LocalPlayer.speechBubbleText.text = LocalPlayer.myMessage;
            LocalPlayer.EmitMyMessage();
            photonView.RPC("UpdateMessage", RpcTarget.All, field.text);
            Debug.Log("bumpedintoplayer");
        }
    }

    public void EmitMyMessage()
    {
        if (speechBubble.activeSelf) return; 
        StartCoroutine(EmitMessage());
    }

    private IEnumerator EmitMessage()
    {
        speechBubble.SetActive(true);
        yield return new WaitForSeconds(5f);
        speechBubble.SetActive(false);
    }

    private void Update()
    {
        
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

        
        if (Input.GetMouseButton(0))
        {
            if (mainCam == null) mainCam = Camera.main;
            var mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            var direction = mouseWorldPos - transform.position;
            if (Vector2.Distance(mouseWorldPos,transform.position) > 0.2f)
            {
                Move(direction, body, speed);
            }
        }
        
    }

    public void UpdateMessages(string inputValue)
    {
        LocalPlayer.myMessage = inputValue;
    }

    [PunRPC]
    public void UpdateMessage(string message)
    {
        Debug.Log("Got this message: " + message);
        Main.AddToDebugDataContent(message);

        Main.otherPlayer.myMessage = message;
        Main.otherPlayer.speechBubbleText.text = message;
        Main.otherPlayer.EmitMyMessage();
        
    }

    public void Move(Vector2 direction, Rigidbody2D body, float speed)
    {
        body.AddForce(direction.normalized * speed, ForceMode2D.Impulse);
    }
}
