using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    #region Fields

    [Header("References")]
    [SerializeField] private GameObject dicePrefab;

    [Header("Settings")]
    [SerializeField] private int countOfDice = 1;
    [SerializeField] private Vector3 start = Vector3.zero;
    [SerializeField] private int spacing = 5;
    [SerializeField] private MinMaxFloat speedRange = new(5f, 10f);
    [SerializeField] private MinMaxFloat randomOffsetX = new(-0.3f, 0.3f);
    [SerializeField] private MinMaxFloat randomOffsetY = new(0f, 0.2f);
    [SerializeField] private MinMaxFloat randomOffsetZ = new(-0.1f, 0.1f);
    [SerializeField] private MinMaxFloat torqueRangeX = new(-10f, 10f);
    [SerializeField] private MinMaxFloat torqueRangeY = new(-10f, 10f);
    [SerializeField] private MinMaxFloat torqueRangeZ = new(-10f, 10f);

    private readonly List<GameObject> dicesList = new();
    private readonly List<Rigidbody> rigitbodyList = new();
    private readonly List<DiceScore> scoresList = new();
    
    private bool resultsProcessed;
    
    private static readonly Dictionary<int, int> BottomToTop = new()
    {
        {1, 6},
        {2, 5},
        {3, 4},
        {4, 3},
        {5, 2},
        {6, 1}
    };

    #endregion

    #region Methods

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
    
    // MinMax методы прописаны в классе
    #endregion
    
    #region Initsialisation

    private void Initialisation()
    {
        for (var i = 0; i < CountOfDice; i++)
        {
            var pos = new Vector3(Start.x + i * Spacing, Start.y, Start.z);
            GameObject dice;
            Rigidbody diceRigidbody;
            DiceScore diceScore;
            
            if (i < dicesList.Count)
            {
                dice = dicesList[i];
                diceRigidbody = rigitbodyList[i];
                diceScore = scoresList[i];

                dice.transform.position = pos;
                dice.transform.rotation = Quaternion.identity;
                diceRigidbody.linearVelocity = Vector3.zero;
                diceRigidbody.angularVelocity = Vector3.zero;
                diceScore.DiceInitialisation(diceRigidbody);
            }
            else
            {
                dice = Instantiate(DicePrefab, pos, Quaternion.identity);
                dicesList.Add(dice);

                diceRigidbody = dice.GetComponent<Rigidbody>();
                rigitbodyList.Add(diceRigidbody);

                diceScore = dice.GetComponent<DiceScore>();
                diceScore.DiceInitialisation(diceRigidbody);
                scoresList.Add(diceScore);
            }
        }
    }
    
    #endregion

    #region  Update

    private void Update()
    {
        
        if (resultsProcessed) return;
        if (!scoresList.All(ds => ds.IsStopped)) return;

        var total = 0;
        foreach (var dice in scoresList)
            total +=  BottomToTop[dice.GetTouchedFaces().FirstOrDefault() + 1];
        
        Debug.Log($"Результаты кубиков Сумма: {total}");

        resultsProcessed = true;
    }

    #endregion

    #region Events
    private void ThrowDice()
    {
        foreach (var (rig, score) in rigitbodyList.Zip(scoresList, (r, s) => (r, s)))
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
    }

    public void OnRoll(InputAction.CallbackContext ctx)
    {
        if (!ctx.started ) return;
        
        foreach (var s in scoresList)
            s.ResetForNewRoll();
        resultsProcessed = false;
        Debug.Log("Бросаем кубики!");
        Initialisation();
        ThrowDice();
    }
    
    
    // Я это сделал и оно работает, остальное не волнует
    public void StartRebindRoll(InputAction.CallbackContext ctx)
    {
        
        if (!ctx.performed) return;
        
        var rollAction = ctx.action?.actionMap?.FindAction("Roll");
        
        rollAction.Disable();
        Debug.Log("Нажмите любую кнопку для перебинда");
        
        rollAction.PerformInteractiveRebinding()
            .WithControlsExcluding("<Keyboard>/tilde")
            .OnComplete(callback =>
            {
                rollAction.Enable();
                callback.Dispose();
                Debug.Log("Roll успешно перебинжен!");
            })
            .Start();
    }
    
    #endregion
}
