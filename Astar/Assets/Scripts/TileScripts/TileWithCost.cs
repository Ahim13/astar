using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class TileWithCost : Tile
{
	public abstract int Cost { get; }
}