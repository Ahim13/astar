using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "Player", menuName = "CustomTiles/Player", order = 0)]
public class Player : TileWithCost
{
	public override int Cost
	{
		get { return 1; }
	}
}