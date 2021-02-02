using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.UI;

public class DistanceGraphicsChanger : MonoBehaviour, IPunObservable , IInRoomCallbacks
{
    [SerializeField] private SpriteRenderer rend;
    [SerializeField] private Transform toFind;
    [SerializeField] private DistancedSpriteContainer sprites;
    [SerializeField] private float intervalBetweenDistanceCheck;
    [SerializeField] private Light2D myLight;
    [SerializeField] private AudioSource audio;

    private float startDistance;
    private float timer;
    private int currentIndex;
    private bool initialized = false;

    private void Start()
    {
        rend.sprite = sprites.Values[0].sprite;
    }

    public void Initiliaze()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 1) return;

        foreach(var player in players)
        {
            Debug.Log("There's players i'm looking for");
            if (player != this.gameObject)
            {
                toFind = player.transform;
            }
        }
        startDistance = Vector2.Distance(toFind.position, transform.position);
        sprites.InitiliazeMinMaxValues();
        initialized = true;
        Debug.Log(initialized);
        currentIndex = GetValidIndexFromDistanceOrFirst(sprites.Values);
        rend.sprite = sprites.Values[currentIndex].sprite;
    }

    private void Update()
    {
        UpdateSpriteBasedOnDistanceOnIntervals();
    }

    public void UpdateSpriteBasedOnDistanceOnIntervals()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            if (!initialized)
            {
                Initiliaze();
                return;
            }
            
            var newSpriteIndex = GetValidIndexFromDistanceOrFirst(sprites.Values);
            if (newSpriteIndex != currentIndex)
            {
                currentIndex = newSpriteIndex;
                rend.sprite = sprites.Values[currentIndex].sprite;
            }
            timer = intervalBetweenDistanceCheck;
        }
    }

    public int GetValidIndexFromDistanceOrFirst(DistancedSprite[] sprites)
    {
       var firstIndex = 0;

       if (sprites == null) return firstIndex;
       if (sprites.Length == 0) return firstIndex;
       if (toFind == null) return firstIndex; 

       var currentDistance = Vector2.Distance(toFind.position, transform.position);
       var percentageDistanceSinceStart = currentDistance / startDistance * 100;

       if (Application.platform == RuntimePlatform.WindowsEditor|| Application.platform == RuntimePlatform.WindowsPlayer)
       {
           myLight.transform.localScale = new Vector2(Mathf.Clamp((100 - percentageDistanceSinceStart) / 10,1,10), Mathf.Clamp((100 - percentageDistanceSinceStart) / 10, 1, 10));
       }
       

        for (int i = 0; i < sprites.Length; i++)
        {
            if (percentageDistanceSinceStart.IsInBetween(sprites[i].minDistance, sprites[i].maxDistance))
            {
                if (sprites[i].sprite != null)
                    return i;
            }

        }
        return firstIndex;
      }
      

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentIndex);
        }
        else
        {
            this.currentIndex = (int)stream.ReceiveNext();
            this.rend.sprite = sprites.Values[currentIndex].sprite;
        }
       
    }

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Initiliaze();
    }

    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        throw new System.NotImplementedException();
    }

    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        throw new System.NotImplementedException();
    }
}
