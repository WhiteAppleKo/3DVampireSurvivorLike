using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClampValue<T> where T : struct, IComparable<T>
{
    public class Callback
    {
        public Action<T, T> onValueChanged;
        public Action<T, T> onIncreased;
        public Action<T, T> onDecreased;
        public Action<T, T> onMinReached;
        public Action<T, T> onMaxReached;
    }

    public Callback Events { get; protected set; } = new Callback();
    
    public T Min { get; protected set; }
    public T Max { get; protected set; }
    public T Current { get; protected set; }
    
    public abstract float Ratio { get; }
    
    public void ResetToMin() => Set(Min);
    public void ResetToMax() => Set(Max);
    
    public void Increase(T amount)
    {
        T prev = Current;
        Set(Add(prev, amount));
        Events.onIncreased?.Invoke(prev, amount);
    }
    
    public void Decrease(T amount)
    {
        T prev = Current;
        Set(Subtract(prev, amount));
        Events.onDecreased?.Invoke(prev, amount);
    }
    
    protected ClampValue(T min, T max, T initial)
    {
        Min = min;
        Max = max;
        Current = Clamp(initial, min, max);
    }
    
    protected void Set(T value)
    {
        value = Clamp(value, Min, Max);
        if (Current.CompareTo(value) == 0) return;

        T prev = Current;
        Current = value;

        Events.onValueChanged?.Invoke(prev, Current);
        if (Current.CompareTo(Min) == 0) Events.onMinReached?.Invoke(prev, Current);
        if (Current.CompareTo(Max) == 0) Events.onMaxReached?.Invoke(prev, Current);
    }
    protected abstract T Add(T a, T b);
    protected abstract T Subtract(T a, T b);
    protected abstract T Clamp(T value, T min, T max);
}