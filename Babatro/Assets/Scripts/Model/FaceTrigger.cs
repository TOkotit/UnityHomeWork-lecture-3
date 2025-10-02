using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FaceTrigger : MonoBehaviour
{
    
    [SerializeField] private int faceIndex;
    public DiceScore owner;
    public bool isTouchingTable;
    
    public int FaceIndex
    {
        get => faceIndex;
        set => faceIndex = value;
    }
    
    private void Awake()
    {
        var myCollider = GetComponent<Collider>();
        myCollider.isTrigger = true;
        if (!owner) owner = GetComponentInParent<DiceScore>();
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Table"))
            isTouchingTable = true;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Table"))
            isTouchingTable = false;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Table"))
            isTouchingTable = true;
    }

}