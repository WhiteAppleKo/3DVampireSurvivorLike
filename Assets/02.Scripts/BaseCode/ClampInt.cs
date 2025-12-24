using UnityEngine;

public class ClampInt : ClampValue<int>
{
    public ClampInt(int min, int max, int initial) : base(min, max, initial)
    {
        
    }

    public override float Ratio => (float)(Current - Min) / (Max - Min);
    protected override int Add(int a, int b)
    {
        return a + b;
    }

    protected override int Subtract(int a, int b)
    {
        return a - b;
    }
    
    protected override int Clamp(int value, int min, int max)
    {
        return Mathf.Clamp(value, min, max);
    }
}
