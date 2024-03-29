﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarManager
{
	private static NodeBinaryHeap openBHList = new NodeBinaryHeap(HeapType.MinHeap);
	private static HashSet<Node> closedList = new HashSet<Node>();

	private static LinkedList<Node> ConvertToPath(Node _node)
	{
		LinkedList<Node> path = new LinkedList<Node>();
		while (_node != null)
		{
			path.AddFirst(_node);
			_node = _node.parent;
		}
		return path;
	}

	public static LinkedList<Node> FindPath(Node _startNode, Node _goalNode, Floor inFloor, eMoveType inMoveType)
	{
		if (_startNode.State != Floor.eTileState.Enemy ||
			_goalNode.State != Floor.eTileState.Morphy)
			return null;

		GridStratergy gridStratergy = inFloor.GridStratergyForMoveType(inMoveType);

		// RESET ALL NODES.
		IEnumerator gridEnumurator = inFloor.Nodes.GetEnumerator();
		while (gridEnumurator.MoveNext())
		{
			(gridEnumurator.Current as Node).Reset();
		}

		openBHList.Clear();
		openBHList.Insert(_startNode);
		_startNode.nodePathCost = 0.0f;
		_startNode.totalCost = gridStratergy.HeuristicEstimatedCost(_startNode, _goalNode, _startNode);// + _startNode.nodePathCost;

		closedList.Clear();
		Node curNode = null;

		while (openBHList.Count > 0)
		{
			// Check if the closed List contains the _goalNode.
			if (closedList.Contains(_goalNode))
			{
				return ConvertToPath(curNode);
			}

			curNode = openBHList.PopRoot();

			for (LinkedListNode<Node> curLinkedNode = curNode.neighbours[(int)inMoveType].First; curLinkedNode != null; curLinkedNode = curLinkedNode.Next)
			{
				Node curNeighbourNode = (Node)curLinkedNode.Value;
				if (curNeighbourNode.State != Floor.eTileState.Empty &&
					curNeighbourNode.State != Floor.eTileState.Morphy)
					continue;

				if (!closedList.Contains(curNeighbourNode))
				{
					//Cost from current node to this neighbour node
					float cost = gridStratergy.NeighbourPathCost(curNode, curNeighbourNode);

					//Total Cost So Far from start to this neighbour node
					float totalPathCost = curNode.nodePathCost + cost;

					//Estimated cost for neighbour node to the goal
					float neighbourNodeEstCost = gridStratergy.HeuristicEstimatedCost(curNeighbourNode, _goalNode, _startNode);

					if (openBHList.Contains(curNeighbourNode)) // Calculated before?
					{
						if (totalPathCost < curNeighbourNode.nodePathCost)
						{
							curNeighbourNode.nodePathCost = totalPathCost;
							curNeighbourNode.parent = curNode;
							curNeighbourNode.totalCost = totalPathCost + neighbourNodeEstCost;
						}
					}
					else
					{
						curNeighbourNode.nodePathCost = totalPathCost;
						curNeighbourNode.parent = curNode;
						curNeighbourNode.totalCost = totalPathCost + neighbourNodeEstCost;

						//Add the neighbour node to the list if not already existed in the list
						openBHList.Insert(curNeighbourNode);
					}
				}
			}

			closedList.Add(curNode);
		}

		if (closedList.Contains(_goalNode))
			return ConvertToPath(curNode);

		return null;
	}
}
