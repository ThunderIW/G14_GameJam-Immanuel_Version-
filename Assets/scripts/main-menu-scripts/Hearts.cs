using DG.Tweening;
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
        AnimateHeart(isFull);
        Debug.Log($"Heart ({gameObject.name}) set to {(isFull ? "FULL" : "EMPTY")}");
    }
    private void AnimateHeart(bool isFull)
    {
        if (isFull)
        {
            transform.localScale = Vector3.one;
            transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 8, 0.5f);

        }else
        {
            transform.DOScale(0.7f, 0.2f).SetLoops(2, LoopType.Yoyo);
            HeartImage.DOFade(0.4f, 0.2f).SetLoops(2, LoopType.Yoyo);

        }
       
    }
    



}
