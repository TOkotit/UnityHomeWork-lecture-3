using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject dicePrefab;

    [Header("Settings")]
    [SerializeField, Range(1, 10)] private int countOfDice = 1;
    [SerializeField] private Vector3 start = Vector3.zero;
    [SerializeField] private int spacing = 5;
    
    [Header("Physics")]
    [SerializeField] private MinMaxFloat speedRange = new(5f, 10f);
    [SerializeField] private MinMaxFloat randomOffsetX = new(-0.3f, 0.3f);
    [SerializeField] private MinMaxFloat randomOffsetY = new(0f, 0.2f);
    [SerializeField] private MinMaxFloat randomOffsetZ = new(-0.1f, 0.1f);
    [SerializeField] private MinMaxFloat torqueRangeX = new(-10f, 10f);
    [SerializeField] private MinMaxFloat torqueRangeY = new(-10f, 10f);
    [SerializeField] private MinMaxFloat torqueRangeZ = new(-10f, 10f);
    
    [SerializeField] private int winScore = 10;
    [SerializeField] private int drawScore = 5;


    private readonly List<GameObject> dicesList = new();
    private readonly List<Rigidbody> rigidbodyList = new();
    private readonly List<DiceScore> scoresList = new();
    
    private bool resultsProcessed;
    private int resultCount;
    private int stoppedCount;
    private string resultLine;
    private string winLoseResultLine;

    public static event Action<string> OnResultChanged;
    public static event Action<ResultOfTheGame> OnWinLoseResultChanged;
    

    
    public string ResultLine
    {
        get => resultLine;
        private set
        {
            resultLine = value;
            OnResultChanged?.Invoke(resultLine);
        }
    }
    
    public GameObject DicePrefab
    {
        get => dicePrefab;
        set
        {
            if (value is null)
            {
                Debug.LogWarning("DicePrefab is null");
                return;
            }
            dicePrefab = value;
        }
    }

    public int CountOfDice
    {
        get => countOfDice;
        set
        {
            if (value < 0)
            {
                Debug.LogWarning("CountOfDice cannot be less than 0");
                return;
            }
            countOfDice = value;
        }
    }

    public Vector3 Start
    {
        get => start;
        set => start = value;
    }

    public int Spacing
    {
        get => spacing;
        set => spacing = value;
    }

    public static ResultOfTheGame ResultOfTheGame
    { 
        set => OnWinLoseResultChanged?.Invoke(value);
    }
    
    public int WinScore
    {
        get => winScore;
        set
        {
            if (value < 0)
            {
                Debug.LogWarning("WinScore cannot be negative");
                return;
            }
            winScore = value;
            ReevaluateLastResult();
        }
    }
    

    public int DrawScore
    {
        get => drawScore;
        set
        {
            if (value < 0)
            {
                Debug.LogWarning("DrawScore cannot be negative");
                return;
            }
            drawScore = value;
            ReevaluateLastResult();
        }
    }

    // MinMax методы прописаны в классе
    
    private void Initialize()
    {
        if (CountOfDice < dicesList.Count)
        {
            for (var i = dicesList.Count - 1; i >= CountOfDice; i--)
            {
                scoresList[i].OnStopped -= HandleDiceStopped;
                Destroy(dicesList[i]);        
                dicesList.RemoveAt(i);
                rigidbodyList.RemoveAt(i);
                scoresList.RemoveAt(i);
            }
        }
        
        
        for (var i = 0; i < CountOfDice; i++)
        {
            var pos = new Vector3(Start.x + i * Spacing, Start.y, Start.z);
            GameObject dice;
            Rigidbody diceRigidbody;
            DiceScore diceScore;
            
            if (i < dicesList.Count)
            {
                dice = dicesList[i];
                diceRigidbody = rigidbodyList[i];
                diceScore = scoresList[i];

                dice.transform.position = pos;
                dice.transform.rotation = Quaternion.identity;
                diceRigidbody.linearVelocity = Vector3.zero;
                diceRigidbody.angularVelocity = Vector3.zero;
                diceScore.DiceInitialisation(diceRigidbody);
                diceScore.OnStopped += HandleDiceStopped;
            }
            else
            {
                dice = Instantiate(DicePrefab, pos, Quaternion.identity);
                dicesList.Add(dice);

                diceRigidbody = dice.GetComponent<Rigidbody>();
                rigidbodyList.Add(diceRigidbody);

                diceScore = dice.GetComponent<DiceScore>();
                diceScore.DiceInitialisation(diceRigidbody);
                diceScore.OnStopped += HandleDiceStopped;
                scoresList.Add(diceScore);
            }
        }
    }
    
    private void ReevaluateLastResult()
    {
        if (!resultsProcessed) return;
        
        if (resultCount >= WinScore)
            ResultOfTheGame = ResultOfTheGame.Win;
        else if (resultCount >= DrawScore)
            ResultOfTheGame = ResultOfTheGame.Draw;
        else
            ResultOfTheGame = ResultOfTheGame.Lose;
    }
    
    private void HandleDiceStopped(DiceScore dice)
    {
        stoppedCount++;
        if (stoppedCount != scoresList.Count) return;
        CalculateResults();
        resultsProcessed = true;
        stoppedCount = 0;
    }
    
    private IEnumerator WaitForAllStopped()
    {
        while (!scoresList.All(ds => ds.IsStopped))
            yield return new WaitForSeconds(0.05f);

        CalculateResults();
        resultsProcessed = true;
    }
    

    private void CalculateResults()
    {
        resultCount = scoresList.Select(dice => 7 - (dice.GetTouchedFaces().FirstOrDefault() + 1)).Sum();
        
        ResultLine = $"Сумма: {resultCount}";

        if (resultCount >= WinScore)
            ResultOfTheGame =  ResultOfTheGame.Win;
        else if (resultCount >= DrawScore)
            ResultOfTheGame = ResultOfTheGame.Draw;
        else
            ResultOfTheGame = ResultOfTheGame.Lose;
    }

    public void OnRoll(InputAction.CallbackContext ctx)
    {
        if (!ctx.started ) return;
        ThrowDice();
    }
    
    public void ThrowDice()
    {
    
        foreach (var s in scoresList)
            s.ResetForNewRoll();
        resultsProcessed = false;
        Initialize();
    
        foreach (var (rig, score) in rigidbodyList.Zip(scoresList, (r, s) => (r, s)))
        {
            var forceDir = (Vector3.forward + new Vector3(
                Random.Range(randomOffsetX.Min, randomOffsetX.Max),
                Random.Range(randomOffsetY.Min, randomOffsetY.Max),
                Random.Range(randomOffsetZ.Min, randomOffsetZ.Max)
            )).normalized;

            rig.AddForce(forceDir * speedRange.GetRandom(), ForceMode.Impulse);

            var torque = new Vector3(
                Random.Range(torqueRangeX.Min, torqueRangeX.Max),
                Random.Range(torqueRangeY.Min, torqueRangeY.Max),
                Random.Range(torqueRangeZ.Min, torqueRangeZ.Max)
            );
            rig.AddTorque(torque, ForceMode.Impulse);

            score.MarkAsMoved();
        }
        
        StartCoroutine(WaitForAllStopped());
    }
    
    // Я это сделал и оно работает, остальное не волнует
    public void StartRebindRoll(InputAction.CallbackContext ctx)
    {
        
        if (!ctx.performed) return;
        
        var rollAction = ctx.action?.actionMap?.FindAction("Roll");

        if (rollAction == null) return;
        rollAction.Disable();
        Debug.Log("Нажмите любую кнопку для перебинда");

        rollAction.PerformInteractiveRebinding()
            .WithControlsExcluding("<Keyboard>/tilde")
            .OnComplete(callback =>
            {
                rollAction.Enable();
                callback.Dispose();
            })
            .Start();
    }
}
