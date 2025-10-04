using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Scripts
{
    public class InputControler : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputCount;
        [SerializeField] private TMP_InputField inputWin;
        [SerializeField] private TMP_InputField inputDraw;
    
        [SerializeField] private Button rollButton;
    
        [SerializeField] private GameManager gameManager;
    

        private void Awake()
        {
            if (!inputCount || !inputWin || !inputDraw ||
                !rollButton || !gameManager)
            {
                Debug.LogError($"{nameof(InputControler)}: Не все ссылки назначены в инспекторе!");
                return;
            }
        
            inputCount.onEndEdit.AddListener(OnInputCount);
            inputWin.onEndEdit.AddListener(OnWinInputChanged);
            inputDraw.onEndEdit.AddListener(OnDrawInputChanged);
            rollButton.onClick.AddListener(gameManager.ThrowDice);
        }
    
        private void OnInputCount(string text)
        {
            if (int.TryParse(text, out var value))
                gameManager.CountOfDice = Mathf.Clamp(value, 1, 10);
            else
                inputCount.text = string.Empty;
        }

        private void OnWinInputChanged(string text)
        {
            if (int.TryParse(text, out var value))
                gameManager.WinScore = value;
            else
                inputWin.text = string.Empty;
        }

        private void OnDrawInputChanged(string text)
        {
            if (int.TryParse(text, out var value))
                gameManager.DrawScore = value;
            else
                inputDraw.text = string.Empty;
        }
    
    }
}
