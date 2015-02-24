using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour
{
    public TileState State
    {
        get
        {
            return _state;
        }
    }

    public GameObject SymbolObject;
    public IntVector2 Coordinates;

    private TileState _state;

    private void Awake()
    {
        _state = TileState.Free;
    }

    public bool Occupy(TileState state)
    {
        if (state == TileState.Free | State != TileState.Free) return false;

        _state = state;
        SymbolObject = GameObject.Instantiate(Resources.Load(state.ToString())) as GameObject;
        SymbolObject.transform.parent = transform;
        SymbolObject.transform.localPosition = Vector3.zero;
        return true;
    }

    public void Clear()
    {
        _state = TileState.Free;
        if(SymbolObject != null)
        {
            Destroy(SymbolObject);
            SymbolObject = null;
        }
    }

    private void OnMouseDown()
    {
        GameplayController.get.OnClickTile(PhotonNetwork.player.GetTeam(), Coordinates);
    }
}
