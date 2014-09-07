using System;


namespace FL2NormOnFilesHalfSize
{
	class MyNode
	{
		public MyNode  Previous;
		public MyNode Next;
		public TreeNode Value;

		public MyNode() { 			
			Previous = null;
			Next = null;
			Value = null;
		}
		public MyNode(MyNode Previous, MyNode Next, TreeNode Value) {
			this.Previous = Previous;
			this.Next = Next;
			this.Value = Value;
		}


	}
}
