using System;


namespace FL2NormOnFilesHalfSize {
	class MyList {
		public MyNode First;
		public MyNode Last;
		public MyNode Current;
		public int Length;

		public MyList() {
			First = null;
			Last = null;
			Current = null;
			Length = 0;
		}
		public TreeNode ReadNode() {
			if (Current == null) Current=First;
			if (Current == null) return null;
			TreeNode node=Current.Value;
			Current = Current.Next;
			return node;
		}

		public void AddLast(TreeNode treeNode){
			if (Last == null){// list is empty
				Last = new MyNode(null,null,treeNode);
				First = Last;
				++Length;
			}else{// list has at least one element
				MyNode oldLast = Last;
				Last = new MyNode(oldLast, null, treeNode);
				oldLast.Next = Last;
				++Length;
			}
		}
		public void AddFirst(TreeNode treeNode) {
			if (First == null) {// list is empty
				First = new MyNode(null, null, treeNode);
				Last = First;
				++Length;				
			}
			else {// list has at least one element
				MyNode oldFirst = First;
				First = new MyNode(null, oldFirst, treeNode);
				oldFirst.Previous = First;
				++Length;
			}
		}
		public void AddAfter(MyNode node, TreeNode treeNode) {
			if (node.Next == null) { AddLast(treeNode); return; }
			MyNode nodeToTheRight = node.Next;
			MyNode newNode = new MyNode(node, node.Next, treeNode);
			node.Next = newNode;
			nodeToTheRight.Previous = newNode;			
		}
		public void AddBefore(MyNode node, TreeNode treeNode) {
			if (node.Previous == null) {AddFirst(treeNode); return;}
			MyNode nodeToTheLeft = node.Previous;
			MyNode newNode = new MyNode(node.Previous, node, treeNode);
			node.Previous = newNode;
			nodeToTheLeft.Next = newNode;
		}

		public void Remove(MyNode node) {
			MyNode nodeToTheLeft = node.Previous;
			MyNode nodeToTheRight = node.Next;
			if (nodeToTheLeft == null) First = node.Next;
			else
				nodeToTheLeft.Next = nodeToTheRight;
			if (nodeToTheRight == null) Last = node.Previous;
			else
				nodeToTheRight.Previous = nodeToTheLeft;
			--Length;
		}

		//First and Last of MyList are joined, node will be First
		public void NewBeginning(MyNode node) {
			First.Previous = Last;
			Last.Next = First;
			MyNode nodeToTheLeft = node.Previous;
			nodeToTheLeft.Next = null;
			node.Previous = null;
			First = node;
			Last = nodeToTheLeft;
			Current = node;
		}

		public void NewBeginning(TreeNode treeNode) {
			MyNode cur = First;
			while (cur != null) {
				if (cur.Value == treeNode) break;
				cur = cur.Next;
			}
			if (cur.Value == treeNode) NewBeginning(cur);
			else Console.WriteLine("Error in NewBeginning: treeNode not found");
		}


	}
}
