using UnityEngine;

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

    public abstract void GetPulled(LassoScript lasso, PlayerController player);

    public abstract Vector3 NeckPosition();
}