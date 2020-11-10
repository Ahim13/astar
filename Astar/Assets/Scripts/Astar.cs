using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Astar : MonoBehaviour
{
	[SerializeField] private Tilemap _tilemap;
	[SerializeField] private Tilemap _secondLayerTilemap;
	[SerializeField] private TileBase[] _tilebase; // 0 - unchecked 1 - checked 2 - wall 3 - checking 4 - target

	[SerializeField] private Vector3Int StartLocation = Vector3Int.zero;
	[SerializeField] private Vector3Int GoalLocation = Vector3Int.zero;
	[SerializeField] private GameObject TextPrefab;


	private void Start()
	{
		// _tilemap.SetTile(new Vector3Int(0, 0, 0), _tilebase[0]);
		// Unchecked a = _tilemap.GetTile<Unchecked>(new Vector3Int(0, 0, 0));

		StartLocation = FindTile<Player>(_secondLayerTilemap);
		GoalLocation = FindTile<Target>(_secondLayerTilemap);

		StartCoroutine(BFSWithCost());
	}

	private Vector3Int FindTile<T>(Tilemap tilemap)
	{
		foreach (var position in tilemap.cellBounds.allPositionsWithin)
		{
			if (tilemap.GetTile(position) is T)
				return position;
		}

		return Vector3Int.zero;
	}

	private IEnumerator BreadthFirstSearch()
	{
		var frontier = new Queue<Vector3Int>();
		frontier.Enqueue(StartLocation);
		var reached = new HashSet<Vector3Int>();
		reached.Add(StartLocation);

		while (frontier.Count > 0)
		{
			var current = frontier.Dequeue();
			_tilemap.SetTile(current, _tilebase[1]);
			foreach (var neighbour in GetNeighbours(current))
			{
				if (!reached.Contains(neighbour))
				{
					frontier.Enqueue(neighbour);
					reached.Add(neighbour);
					_tilemap.SetTile(neighbour, _tilebase[3]);
				}
			}

			yield return new WaitForSeconds(0.05f);
		}
	}

	private IEnumerator BreadthFirstSearch2()
	{
		var frontier = new Queue<Vector3Int>();
		frontier.Enqueue(StartLocation);
		var cameFrom = new Dictionary<Vector3Int, Vector3Int?>();
		cameFrom[StartLocation] = null;

		while (frontier.Count > 0)
		{
			var current = frontier.Dequeue();
			_tilemap.SetTile(current, _tilebase[1]);
			foreach (var next in GetNeighbours(current))
			{
				if (!cameFrom.ContainsKey(next))
				{
					frontier.Enqueue(next);
					cameFrom[next] = current;
					_tilemap.SetTile(next, _tilebase[3]);
					CreateArrowPointingPrevious(next, cameFrom);
				}
			}

			yield return new WaitForSeconds(0.02f);
		}

		var curr = GoalLocation;
		List<Vector3Int> path = new List<Vector3Int>();
		while (curr != StartLocation)
		{
			path.Add(curr);
			_tilemap.SetTile(curr, _tilebase[6]);
			curr = cameFrom[curr].Value;
			yield return new WaitForSeconds(0.05f);
		}

		_tilemap.SetTile(curr, _tilebase[6]);
	}

	private IEnumerator BFSEarlyExit()
	{
		var frontier = new Queue<Vector3Int>();
		frontier.Enqueue(StartLocation);
		var cameFrom = new Dictionary<Vector3Int, Vector3Int?>();
		cameFrom[StartLocation] = null;
		var goal = GoalLocation;
		List<Vector3Int> path = new List<Vector3Int>();

		while (frontier.Count > 0)
		{
			var current = frontier.Dequeue();
			_tilemap.SetTile(current, _tilebase[1]);
			if (current == goal)
				break;
			foreach (var next in GetNeighbours(current))
			{
				if (!cameFrom.ContainsKey(next))
				{
					frontier.Enqueue(next);
					cameFrom[next] = current;
					_tilemap.SetTile(next, _tilebase[3]);
					CreateArrowPointingPrevious(next, cameFrom);
				}
			}

			yield return new WaitForSeconds(0.02f);
		}

		while (goal != StartLocation)
		{
			path.Add(goal);
			_tilemap.SetTile(goal, _tilebase[6]);
			goal = cameFrom[goal].Value;
			yield return new WaitForSeconds(0.05f);
		}

		_tilemap.SetTile(goal, _tilebase[6]);
	}

	private IEnumerator BFSWithCost()
	{
		var frontier = new SimplePriorityQueue<Vector3Int>();
		var cameFrom = new Dictionary<Vector3Int, Vector3Int?>();
		var costSoFar = new Dictionary<Vector3Int, int>();
		var CostTexts = new Dictionary<Vector3Int, TextMeshPro>();
		var path = new List<Vector3Int>();
		var goal = GoalLocation;

		frontier.Enqueue(StartLocation, 0);
		cameFrom[StartLocation] = null;
		costSoFar[StartLocation] = 0;

		while (frontier.Count > 0)
		{
			var current = frontier.Dequeue();
			if (current != StartLocation && current != GoalLocation)
				_secondLayerTilemap.SetTile(current, _tilebase[1]);
			if (current == goal)
				break;
			foreach (var next in GetNeighbours(current))
			{
				var newCost = costSoFar[current] + ((TileWithCost) _tilemap.GetTile(next)).Cost;
				if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
				{
					costSoFar[next] = newCost;
					var priority = newCost;
					frontier.Enqueue(next, priority);
					cameFrom[next] = current;
					if (!CostTexts.ContainsKey(next))
					{
						CostTexts[next] = Instantiate(TextPrefab, _tilemap.CellToWorld(next), Quaternion.identity, GameObject.Find("Texts").transform).GetComponent<TextMeshPro>();

						CostTexts[next].text = newCost.ToString();
					}
					else
					{
						CostTexts[next].text = newCost.ToString();
					}

					if (next != StartLocation && next != GoalLocation)
						_secondLayerTilemap.SetTile(next, _tilebase[3]);
				}
			}

			yield return new WaitForSeconds(0.02f);
		}


		while (goal != StartLocation)
		{
			path.Add(goal);
			if (goal != StartLocation && goal != GoalLocation)
				_secondLayerTilemap.SetTile(goal, _tilebase[6]);
			goal = cameFrom[goal].Value;
			yield return new WaitForSeconds(0.05f);
		}

		//_secondLayerTilemap.SetTile(goal, _tilebase[6]);
	}

	private void CreateArrowPointingPrevious(Vector3Int next, Dictionary<Vector3Int, Vector3Int?> cameFrom)
	{
		var targetVector = cameFrom[next] - next;
		if (targetVector != null)
		{
			_secondLayerTilemap.SetTile(next, _tilebase[5]);
			Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(Vector3.forward, targetVector.Value), Vector3.one);
			_secondLayerTilemap.SetTransformMatrix(next, matrix);
		}
	}

	private IEnumerable<Vector3Int> GetNeighbours(Vector3Int center)
	{
		List<Vector3Int> neighbours = new List<Vector3Int>();

		var neighbour = center + new Vector3Int(1, 0, 0);
		if (_tilemap.GetTile(neighbour) && !(_tilemap.GetTile(neighbour) is Wall))
			neighbours.Add(neighbour);
		neighbour = center + new Vector3Int(-1, 0, 0);
		if (_tilemap.GetTile(neighbour) && !(_tilemap.GetTile(neighbour) is Wall))
			neighbours.Add(neighbour);
		neighbour = center + new Vector3Int(0, 1, 0);
		if (_tilemap.GetTile(neighbour) && !(_tilemap.GetTile(neighbour) is Wall))
			neighbours.Add(neighbour);
		neighbour = center + new Vector3Int(0, -1, 0);
		if (_tilemap.GetTile(neighbour) && !(_tilemap.GetTile(neighbour) is Wall))
			neighbours.Add(neighbour);

		return neighbours;
	}
}