using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace UI_Scripts
{
    public class OutputController : MonoBehaviour
    {
        [SerializeField] private TMP_Text countResultTMP;
        [SerializeField] private TMP_Text gameResultTMP;
        [SerializeField] private Image resultImage;
        
        [SerializeField] private Sprite winSprite;
        [SerializeField] private Sprite drawSprite;
        [SerializeField] private Sprite loseSprite;
        
        [SerializeField] private string winText;
        [SerializeField] private string loseText;
        [SerializeField] private string drawText;
        
    
        [SerializeField] private GameManager gameManager;

        
        private void Awake()
        {
            if (!countResultTMP || !gameResultTMP || !gameManager
                || !winSprite || !drawSprite || !loseSprite)
            {
                Debug.LogError($"{nameof(OutputController)}: Не все ссылки назначены в инспекторе!");
                return;
            }
            
            if (drawSprite)
                resultImage.sprite = drawSprite;
            
            GameManager.OnResultChanged += resultText => countResultTMP.text = resultText;
            GameManager.OnWinLoseResultChanged += UpdateUI;

        }
        
        private void UpdateUI(ResultOfTheGame result)
        {
            switch (result)
            {
                case ResultOfTheGame.Win:
                    gameResultTMP.text = winText;
                    resultImage.sprite = winSprite;
                    break;
                case ResultOfTheGame.Draw:
                    gameResultTMP.text = drawText;
                    resultImage.sprite = drawSprite;
                    break;
                case ResultOfTheGame.Lose:
                    gameResultTMP.text = loseText;
                    resultImage.sprite = loseSprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

    }
}
