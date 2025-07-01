using UnityEngine;

public class Ladder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerLadderMovement ladderMovement = other.GetComponent<PlayerLadderMovement>();
            if (ladderMovement != null)
                ladderMovement.isOnLadder = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerLadderMovement ladderMovement = other.GetComponent<PlayerLadderMovement>();
            if (ladderMovement != null)
                ladderMovement.isOnLadder = false;
        }
    }
}
