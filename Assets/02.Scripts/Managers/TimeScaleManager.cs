using UnityEngine;

public class TimeScaleManager : SingletoneBase<TimeScaleManager>
{
    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }
}
