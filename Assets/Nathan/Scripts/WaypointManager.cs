using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [Header("Target")]
    public Trainer trainer;               // The trainer whose waypoint will be updated
    public Transform newWaypoint;          // The new waypoint to assign

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (trainer != null && newWaypoint != null)
            {
                trainer.SetWaypoint(newWaypoint); // We'll add this method to Trainer
                trainer.StartWalking();
            }
        }
    }
}