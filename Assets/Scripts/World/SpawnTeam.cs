using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnType
{
    antiterroristSpawn,
    terroristSpawn
}

public class SpawnTeam : MonoBehaviour
{
    public float squareWidthSpawn;
    public float squareHeightSpawn;

    public SpawnType spawnType;

   public Vector3 SpawnPosition()
    {
        float newX = Random.Range(transform.position.x - squareWidthSpawn, transform.position.x + squareWidthSpawn);
        float newY = Random.Range(transform.position.y - squareHeightSpawn, transform.position.y + squareHeightSpawn);

        return new Vector3(0, 0, 0);
    }

    private void OnDrawGizmosSelected()
    {

        if(spawnType == SpawnType.antiterroristSpawn)
        {
            Gizmos.color = Color.blue;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireCube(transform.position, new Vector2(squareWidthSpawn, squareHeightSpawn));
        
    }
}
