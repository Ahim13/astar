using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "HigherCost", menuName = "CustomTiles/HigherCost", order = 0)]
public class HigherCost : TileWithCost
{
	public override int Cost => 5;
}