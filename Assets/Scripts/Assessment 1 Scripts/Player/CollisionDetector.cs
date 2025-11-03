#region

using UnityEngine;

#endregion

namespace Assessment_1_Scripts.Player
{
    public class CollisionDetector : MonoBehaviour
    {
        public delegate void
            CollisionEventHandler(); //the delegate type for the collision state event - use void return type and no parameters

        public event CollisionEventHandler
            CollisionOverlap; //the collision state event - using the delegate type and it's signature

        //alternatively, you can use Action from System namespace
        //public event Action CollisionEntered;//the collision state event

        private void OnCollisionEnter2D(Collision2D other) //when a collision starts
        {
            CollisionOverlap?.Invoke(); //if the event isn't null, collision entered
        }

        //potential for a separate event for exit
        private void OnCollisionExit2D(Collision2D other) //when a collision ends
        {
            CollisionOverlap?.Invoke();
        }
    }
}