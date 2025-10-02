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
    
        public TMP_InputField InputCount
        {
            get => inputCount;
            set
            {
                if (value is null)
                {
                    Debug.Log("InputCount can't be null");
                    return;
                }
                inputCount = value;
            }
        }
    
        public TMP_InputField InputWin
        {
            get => inputWin;
            set
            {
                if (value is null)
                {
                    Debug.Log("InputWin can't be null");
                    return;
                }
                inputWin = value;
            }
        }
    
        public TMP_InputField InputDraw
        {
            get => inputDraw;
            set
            {
                if (value is null)
                {
                    Debug.Log("InputDraw can't be null");
                    return;
                }
                inputDraw = value;
            }
        }
    
        public Button RollButton
        {
            get => rollButton;
            set
            {
                if (value is null)
                {
                    Debug.Log("RollButton can't be null");
                    return;
                }
                rollButton = value;
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
            if (!inputCount || !inputWin || !inputDraw ||
                !rollButton || !gameManager)
            {
                Debug.LogError($"{nameof(InputControler)}: Не все ссылки назначены в инспекторе!");
                return;
            }
        
            InputCount.onEndEdit.AddListener(OnInputCount);
            InputWin.onEndEdit.AddListener(OnWinInputChanged);
            InputDraw.onEndEdit.AddListener(OnDrawInputChanged);
            RollButton.onClick.AddListener(GameManager.ThrowDice);
        }
    
        private void OnInputCount(string text)
        {
            if (int.TryParse(text, out var value))
                GameManager.CountOfDice = Mathf.Clamp(value, 1, 10);
            else
                InputCount.text = string.Empty;
        }

        private void OnWinInputChanged(string text)
        {
            if (int.TryParse(text, out var value))
                GameManager.WinScore = value;
            else
                InputWin.text = string.Empty;
        }

        private void OnDrawInputChanged(string text)
        {
            if (int.TryParse(text, out var value))
                GameManager.DrawScore = value;
            else
                InputDraw.text = string.Empty;
        }
    
    }
}
