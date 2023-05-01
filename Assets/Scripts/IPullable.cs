using UnityEngine;

public class IPullable : MonoBehaviour
{
    public virtual float SpeedReduction()
    {
        return 1;
    }

    public virtual bool CanBePulled()
    {
        return true;
    }

    public virtual void GetPulled(LassoScript lasso, PlayerController player) { }

    public virtual Vector3 NeckPosition()
    {
        return Vector3.zero;
    }
}