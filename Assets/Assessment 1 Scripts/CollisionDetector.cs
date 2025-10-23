using System;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public static event Action<bool> CollisionEntered;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionEntered?.Invoke(true);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        CollisionEntered?.Invoke(false);
    }
}
