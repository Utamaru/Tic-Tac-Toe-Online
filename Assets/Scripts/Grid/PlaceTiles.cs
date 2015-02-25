using UnityEngine;
using System.Collections;

public class PlaceTiles : MonoBehaviour
{
    public GameObject TilePrefab;
    public Transform Parent;

    private float unit = 2.56f;

    void Start()
    {
        float startX = -unit;
        float posX = startX;
        float posY = -unit;
        float deltaPos = unit;

        for (int i = 0; i < Helpers.GridSize.y; ++i)
        {
            for (int j = 0; j < Helpers.GridSize.x; ++j)
            {
                GameObject tile = GameObject.Instantiate(TilePrefab) as GameObject;
                Transform tileTransform = tile.transform;
                Tile tileScript = tile.AddComponent<Tile>();
                tileTransform.parent = Parent;
                tileTransform.localPosition = new Vector3(posX, posY);
                tileTransform.name = j + ":" + i;
                tileScript.Coordinates = new IntVector2(j, i);

                Grid.get.SetTile(tileScript, new IntVector2(j, i));

                posX += deltaPos;
            }

            posX = startX;
            posY += deltaPos;
        }
    }
}
