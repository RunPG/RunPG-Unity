using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivityScript : MonoBehaviour
{
    public abstract int range { get; }
    public abstract int cooldown { get; }

    public abstract void Enter();
    public abstract void ShowInfo();
    public abstract bool IsInRange();
}
