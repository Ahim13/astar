using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private float _movementSpeed = 1;
	[SerializeField] private Rigidbody2D _rb;
	[SerializeField] private Camera _mainCamera;
	[SerializeField] private Tilemap _tilemap;
	[SerializeField] private Animator _animator;

	private IEnumerator _moveCoroutine;

	private static readonly int Horizontal = Animator.StringToHash("Horizontal");
	private static readonly int Vertical = Animator.StringToHash("Vertical");
	private static readonly int Speed = Animator.StringToHash("Speed");

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (!(_tilemap.GetTile(_tilemap.WorldToCell(_mainCamera.ScreenToWorldPoint(Input.mousePosition))) is Wall))
			{
				if (_moveCoroutine != null)
					StopCoroutine(_moveCoroutine);
				_moveCoroutine = MoveAlongPath(_tilemap.WorldToCell(_mainCamera.ScreenToWorldPoint(Input.mousePosition)));
				StartCoroutine(_moveCoroutine);
			}
		}
	}

	private IEnumerator MoveAlongPath(Vector3Int target)
	{
		var path = GetPathToLocation(target);
		while (path.Count > 0)
		{
			Vector2 newLoc = _tilemap.GetCellCenterWorld(path.Pop());

			while (_rb.position != newLoc)
			{
				_rb.position = Vector2.MoveTowards(_rb.position, newLoc, _movementSpeed * Time.deltaTime);
				var direction = (newLoc - _rb.position).normalized;
				_animator.SetFloat(Horizontal, direction.x);
				_animator.SetFloat(Vertical, direction.y);
				_animator.SetFloat(Speed, 1);

				yield return null;
			}

			_animator.SetFloat(Speed, 0);
		}
	}

	private Stack<Vector3Int> GetPathToLocation(Vector3Int target)
	{
		var frontier = new SimplePriorityQueue<Vector3Int>();
		var cameFrom = new Dictionary<Vector3Int, Vector3Int?>();
		var costSofar = new Dictionary<Vector3Int, int>();
		var path = new Stack<Vector3Int>();
		var goal = target;
		var startLoc = _tilemap.WorldToCell(_rb.position);

		frontier.Enqueue(startLoc, 0);
		cameFrom[startLoc] = null;
		costSofar[startLoc] = 0;

		while (frontier.Count > 0)
		{
			var current = frontier.Dequeue();
			if (current == target)
				break;
			foreach (var next in GetNeighbours(current, _tilemap))
			{
				var newCost = costSofar[current];
				if (_tilemap.GetTile(next) is TileWithCost)
					newCost += ((TileWithCost) _tilemap.GetTile(next)).Cost;
				else
					newCost += 100;
				if (!costSofar.ContainsKey(next) || newCost < costSofar[next])
				{
					costSofar[next] = newCost;
					var priority = newCost + Heurustic(target, next);
					frontier.Enqueue(next, priority);
					cameFrom[next] = current;
				}
			}
		}

		while (goal != startLoc)
		{
			path.Push(goal);
			if (cameFrom.ContainsKey(goal))
				goal = cameFrom[goal].Value;
		}

		return path;
	}

	private int Heurustic(Vector3Int target, Vector3Int next)
	{
		return Mathf.Abs(target.x - next.x) + Mathf.Abs(target.y - next.y);
	}

	private IEnumerable<Vector3Int> GetNeighbours(Vector3Int center, Tilemap tilemap)
	{
		List<Vector3Int> neighbours = new List<Vector3Int>();

		var neighbour = center + new Vector3Int(1, 0, 0);
		if (tilemap.GetTile(neighbour) && !(tilemap.GetTile(neighbour) is Wall))
			neighbours.Add(neighbour);
		neighbour = center + new Vector3Int(-1, 0, 0);
		if (tilemap.GetTile(neighbour) && !(tilemap.GetTile(neighbour) is Wall))
			neighbours.Add(neighbour);
		neighbour = center + new Vector3Int(0, 1, 0);
		if (tilemap.GetTile(neighbour) && !(tilemap.GetTile(neighbour) is Wall))
			neighbours.Add(neighbour);
		neighbour = center + new Vector3Int(0, -1, 0);
		if (tilemap.GetTile(neighbour) && !(tilemap.GetTile(neighbour) is Wall))
			neighbours.Add(neighbour);

		return neighbours;
	}
}