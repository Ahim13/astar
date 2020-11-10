using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Checking", menuName = "CustomTiles/Checking", order = 0)]
public class Checking : TileWithCost
{
	public override int Cost
	{
		get { return 1; }
	}
}