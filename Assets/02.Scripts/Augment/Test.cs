using UnityEngine;

[CreateAssetMenu(fileName = "New Stat", menuName = "Augments/test")]
public class Test : ScriptableObject, IStatAugment
{
    [Tooltip("증가시킬 체력 양")] 
    public int hp = 20;
    

    public void ModifyStats(BaseStats stats)
    {
        stats.hp.IncreaseMaxValue(hp);
        stats.hp.Increase(hp);
    }
}