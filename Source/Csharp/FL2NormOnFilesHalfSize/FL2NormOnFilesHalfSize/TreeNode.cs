using System;


namespace FL2NormOnFilesHalfSize
{
	class TreeNode
	{
		public TreeNode left;
		public TreeNode right;
		public TreeNode parent;
		public int leaves; // total number of leaves below this node
		public bool currentNodeIsTheLeftChild;
		public TreeNode sigma;//if it is a leaf it will point to the node in myList;

		public TreeNode(TreeNode leftNode = null, TreeNode rightNode = null, TreeNode parentNode = null, bool currentNodeIsTheLeftChild=true,TreeNode sigmaNode=null) { // CALLED WHEN Node Tree = new Node()
			
			left = leftNode;
			right = rightNode;
			parent = parentNode;
			if (left != null) { left.currentNodeIsTheLeftChild = true; left.parent = this; }
			if (right != null) { right.currentNodeIsTheLeftChild = false; right.parent = this; }
			sigma = sigmaNode;
			this.currentNodeIsTheLeftChild = currentNodeIsTheLeftChild;


			// update parent links in children nodes, and calculate how many total leaves below this node
			leaves = 0;
			
			if (left != null && right != null) { 
				left.parent = this; 
				right.parent = this; 
				leaves =left.leaves+ right.leaves;
			}// if leftchild of currentNode  is not null then the parent of the left child is the current node. The leaves of the current child are increased by the leaves of the left child
			if (left == null && right == null) { leaves = 1; }
			
			
			
			//else {
			//	left.parent = this;
			//	right.parent = this;
			//	leaves = left.leaves + right.leaves;
			//}
		
		}
	}
}
