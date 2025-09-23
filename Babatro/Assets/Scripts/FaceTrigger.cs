using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FaceTrigger : MonoBehaviour
{
    #region Fields
    
    [SerializeField] private int faceIndex;
    public DiceScore owner;
    public bool isTouchingTable;
    
    #endregion

    #region Methods
    public int FaceIndex
    {
        get => faceIndex;
        set => faceIndex = value;
    }
    
    #endregion

    #region UnityEvents
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
    
    #endregion
}