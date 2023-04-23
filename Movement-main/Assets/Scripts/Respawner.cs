using UnityEngine;

public class Respawner : MonoBehaviour {
    [SerializeField] GameObject player;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float spawnValue;

    void Update() {
        if (player.transform.position.y < spawnValue) {
            RespawnPoint();
        }
    }

    void RespawnPoint() {
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = spawnPoint.transform.position;
        player.GetComponent<CharacterController>().enabled = true;
    }
}