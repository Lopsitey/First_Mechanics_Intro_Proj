using UnityEngine;

public class CameraInitialisation : MonoBehaviour
{
   public Vector3 offset;
   private Transform playerTransform;
   
   public void init(GameObject player)
   {
      playerTransform = player.transform;
   }
   void LateUpdate()
   {
      if (playerTransform)
      {
         transform.position = playerTransform.position + offset;
      }
   }
}
