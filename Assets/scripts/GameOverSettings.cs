using TMPro;
using UnityEngine;

public class GameOverSettings : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI highScoreText;
   
    [Header("For testing the level reached text and highScore achieved")]
    [SerializeField]  private int level=5;
    [SerializeField]  private int highScore = 5;

   

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        if (PlayerPrefs.HasKey("levelReached") && PlayerPrefs.HasKey("HighScore")){
            level = PlayerPrefs.GetInt("levelReached", 0);
            highScore = PlayerPrefs.GetInt("HighScore", 0);

        }
        levelText.text = $"You reached:  <b><color=#6EC4E8>level {level}</color></b>";
        highScoreText.text= $"{highScore}";




    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
