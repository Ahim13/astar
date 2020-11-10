using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "Unchecked", menuName = "CustomTiles/Unchecked", order = 0)]
public class Unchecked : TileWithCost
{
	public override int Cost
	{
		get { return 1; }
	}
}