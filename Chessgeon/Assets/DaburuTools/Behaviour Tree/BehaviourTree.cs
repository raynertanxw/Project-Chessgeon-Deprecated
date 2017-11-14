﻿using UnityEngine;
using System.Collections;

namespace DaburuTools
{
	public class BehaviourTree
	{
		private IBTNode mRootNode = null;
		private IBTNode mCurNode = null;
		public void SetCurNode(IBTNode _newCurNode) { mCurNode = _newCurNode; }

		public BehaviourTree(IBTNode _rootNode)
		{
			mRootNode = _rootNode;
			mRootNode.SetTree(this);
			ResetTree();
		}

		// The update function.
		public void Tick()
		{
			if (mCurNode == null)
			{
				ResetTree();
			}
			mCurNode.Execute();
			// Once a node returns running, OR the curNode is set to null, end tick.
			while (mCurNode != null && mCurNode.Status != BTStatus.Running)
			{
				mCurNode.Execute();
			}
		}

		private void ResetTree()
		{
			mCurNode = mRootNode;
			mCurNode.ResetNode();
		}
	}
}