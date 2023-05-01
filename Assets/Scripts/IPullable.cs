using UnityEngine;

public enum TryPullResult 
{
    StartPulling,
    Fail,
}

abstract public class IPullable : MonoBehaviour
{
    public virtual float SpeedReduction()
    {
        return 1;
    }

    public virtual bool CanBePulled()
    {
        return true;
    }

    public abstract TryPullResult GetPulled(LassoScript lasso, PlayerController player);

    public abstract Vector3 NeckPosition();
}