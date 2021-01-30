using UnityEngine;

[CreateAssetMenu(fileName = "DistancedSpriteContainer")]
public class DistancedSpriteContainer : ScriptableObject
{
    public DistancedSprite[] Values;
    public void InitiliazeMinMaxValues()
    {
        var percentageFragment = 100 / Values.Length;
        var reverseIterator = Values.Length -1;

        for(int i = 0; i < Values.Length; i++)
        {
            Values[reverseIterator].minDistance = i * percentageFragment;
            Values[reverseIterator].maxDistance = (i + 1) * percentageFragment;
            reverseIterator--;
        }

        Values[0].maxDistance = 101;
    }
}
