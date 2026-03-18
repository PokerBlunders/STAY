using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [Header("Target")]
    public Trainer trainer;
    public Transform newWaypoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (trainer != null && newWaypoint != null)
            {
                trainer.SetWaypoint(newWaypoint);
                trainer.StartWalking();
            }
        }
    }
}