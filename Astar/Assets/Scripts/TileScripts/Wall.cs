using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Wall", menuName = "CustomTiles/Wall", order = 0)]
public class Wall : TileWithCost
{
	public override int Cost
	{
		get { return 100; }
	}
}