using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceGraphicsChanger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer rend;
    [SerializeField] private Transform toFind;
    [SerializeField] private DistancedSpriteContainer sprites;
    [SerializeField] private float intervalBetweenDistanceCheck;

    private float startDistance;
    private float timer;

    private void Start()
    {
        startDistance = Vector2.Distance(toFind.position, transform.position);
        sprites.InitiliazeMinMaxValues();
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
            var newSprite = CheckDistanceAndReturnNewSpriteOrCurrent();
            if (newSprite != rend.sprite)
            {
                rend.sprite = newSprite;
            }
            timer = intervalBetweenDistanceCheck;
        }
    }

    public Sprite CheckDistanceAndReturnNewSpriteOrCurrent()
    {
       if (sprites == null) return rend.sprite;
       if (sprites.Values.Length == 0) return rend.sprite;

       var currentDistance = Vector2.Distance(toFind.position, transform.position);
       var percentageDistanceSinceStart = currentDistance / startDistance * 100;
       
       foreach(var dSprite in sprites.Values)
       {
            if (percentageDistanceSinceStart.IsInBetween(dSprite.minDistance,dSprite.maxDistance))
            {
                if(dSprite.sprite != null)
                    return dSprite.sprite;
            }
       }

       return rend.sprite;
    }
}
