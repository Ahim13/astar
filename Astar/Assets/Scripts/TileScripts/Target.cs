using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Target", menuName = "CustomTiles/Target", order = 0)]
public class Target : TileWithCost
{
	public override int Cost
	{
		get { return 1; }
	}
}