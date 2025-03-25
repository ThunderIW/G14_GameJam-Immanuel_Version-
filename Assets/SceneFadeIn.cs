using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SceneFadeIn : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 1);
            fadeImage.raycastTarget = true;

            fadeImage.DOFade(0, fadeDuration).OnComplete(() =>
            {
                fadeImage.raycastTarget = false;
            });
        }
    }
}
