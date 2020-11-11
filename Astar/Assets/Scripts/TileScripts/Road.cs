using UnityEngine;


[CreateAssetMenu(fileName = "Road", menuName = "CustomTiles/Road", order = 0)]
public class Road : TileWithCost
{
	public override int Cost => 1;
}