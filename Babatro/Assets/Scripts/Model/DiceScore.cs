using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class DiceScore : MonoBehaviour
{
    
    [SerializeField] private float velocityThreshold = 0.1f;
    [SerializeField] private float angularVelocityThreshold = 0.1f;

    private Rigidbody diceRigidbody;
    private bool isTouchingTable;
    private bool hasMoved = false;
    public FaceTrigger[] faceTriggers;
    
    
    public float VelocityThreshold
    {
        get => velocityThreshold;
        set => velocityThreshold = value;
    }

    public float AngularVelocityThreshold
    {
        get => angularVelocityThreshold;
        set => angularVelocityThreshold = value;
    }

    public bool IsStopped
    {
        get
        {
            if (!diceRigidbody) return false;

            var linear = diceRigidbody.linearVelocity.magnitude;
            var angular = diceRigidbody.angularVelocity.magnitude;

            if (linear > VelocityThreshold || angular > AngularVelocityThreshold)
                hasMoved = true;

            return isTouchingTable && hasMoved && 
                   linear < VelocityThreshold && 
                   angular < AngularVelocityThreshold;
        }
    }
    
    public void DiceInitialisation(Rigidbody rb)
    {
        diceRigidbody = rb;
        faceTriggers = GetComponentsInChildren<FaceTrigger>();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Table"))
            isTouchingTable = true;
    }
    
    public void ResetForNewRoll()
    {
        hasMoved = false;
        isTouchingTable = false;
    }
    public int[] GetTouchedFaces()
        => faceTriggers
            .Where(ft => ft.isTouchingTable)
            .Select(ft => ft.FaceIndex)
            .ToArray();

    public void MarkAsMoved() => hasMoved = true;

}