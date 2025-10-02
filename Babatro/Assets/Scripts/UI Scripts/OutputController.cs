using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace UI_Scripts
{
    public class OutputController : MonoBehaviour
    {
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private TMP_Text winLoseText;
        [SerializeField] private Image resultImage;
        
        [SerializeField] private Sprite winSprite;
        [SerializeField] private Sprite drawSprite;
        [SerializeField] private Sprite loseSprite;
    
        [SerializeField] private GameManager gameManager;
        
        public TMP_Text ResultText
        {
            get => resultText;
            set
            {
                if (value is null)
                {
                    Debug.Log("ResultText can't be null");
                    return;
                }
                resultText = value;
            }
        }
    
        public TMP_Text WinLoseText
        {
            get => winLoseText;
            set
            {
                if (value is null)
                {
                    Debug.Log("WinLoseText can't be null");
                    return;
                }
                winLoseText = value;
            }
        }
    
        public GameManager GameManager
        {
            get => gameManager;
            set
            {
                if (value is null)
                {
                    Debug.Log("GameManager can't be null");
                    return;
                }
                gameManager = value;
            }
        }
        
        private void Awake()
        {
            if (!ResultText || !WinLoseText || !GameManager
                || !winSprite || !drawSprite || !loseSprite)
            {
                Debug.LogError($"{nameof(OutputController)}: Не все ссылки назначены в инспекторе!");
                return;
            }
            
            if (drawSprite)
                resultImage.sprite = drawSprite;
            
            GameManager.OnResultChanged += result => ResultText.text = result;
            GameManager.OnWinLoseResultChanged += (text) =>
            {
                WinLoseText.text = text;
                resultImage.sprite = text switch
                {
                    "Победа!" => winSprite,
                    "Ничья" => drawSprite,
                    "Поражение!" => loseSprite,
                    _ => resultImage.sprite
                };
            };

        }

    }
}
