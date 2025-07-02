using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemyPrefab;

    private void Update()
    {
        SpawnEnemyPrefab();
    }
    public void SpawnEnemyPrefab()
    {
        if(Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Joystick1Button13))
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.transform.position = transform.position + Vector3.forward * 10f;
        }
    }
}
