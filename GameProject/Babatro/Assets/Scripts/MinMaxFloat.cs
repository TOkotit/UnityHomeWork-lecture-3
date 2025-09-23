using UnityEngine;

[System.Serializable]
public struct MinMaxFloat
{
    #region Fields
    
    [SerializeField] private float _min;
    [SerializeField] private float _max;

    #endregion
    
    #region Methods
    public float Min
    {
        get => _min;
        set { _min = value; if (_max < _min) _max = _min; }
    }

    public float Max
    {
        get => _max;
        set { _max = value; if (_min > _max) _min = _max; }
    }
    
    #endregion

    #region MinMax
    public MinMaxFloat(float min, float max)
    {
        _min = 0; _max = 0;
        Min = min;
        Max = max;
    }

    public float GetRandom() => Random.Range(Min, Max);
    
    #endregion
}