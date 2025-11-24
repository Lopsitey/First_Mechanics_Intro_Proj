#region

using Unity.Properties;
using UnityEngine;

#endregion

namespace Assessment_2_Scripts.Player
{
    public class TransformWrapper : MonoBehaviour
    {
        //allows the data source path and data source to access the transform position x and y
        [CreateProperty] public float xPos => transform.position.x;

        [CreateProperty] public float yPos => transform.position.y;
    }
}