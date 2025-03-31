using UnityEngine;
using UnityEngine.UI;

public class Hearts : MonoBehaviour
{
    public Sprite fullHeart, emptyHeart;
    Image HeartImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    public void setFull(bool isFull)
    {
        if (HeartImage == null)
        {
            HeartImage = GetComponent<Image>();
        }

        if (fullHeart == null || emptyHeart == null)
        {
            Debug.LogError("FullHeart or EmptyHeart sprite not set!");
            return;
        }

        HeartImage.sprite=isFull ? fullHeart : emptyHeart;
        Debug.Log($"Heart ({gameObject.name}) set to {(isFull ? "FULL" : "EMPTY")}");
    }


}
