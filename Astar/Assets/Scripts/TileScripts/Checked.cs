using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "Checked", menuName = "CustomTiles/Checked", order = 0)]
public class Checked : TileWithCost
{
	public override int Cost
	{
		get { return 1; }
	}
}
