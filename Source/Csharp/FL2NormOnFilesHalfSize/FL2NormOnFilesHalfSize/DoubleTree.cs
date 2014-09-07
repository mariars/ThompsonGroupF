using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FL2NormOnFilesHalfSize {
	class DoubleTree {
		public TreeNode range;
		public TreeNode domain;

		public DoubleTree() { // CREATES THE IDENTITY DOUBLETREE		
			range = new TreeNode();
			domain = new TreeNode();

			range.sigma = domain;
			domain.sigma = range;
		}

		//WARNING sigma is left undefined.
		public DoubleTree(TreeNode rangeTree, TreeNode domainTree) { // CREATES A DOUBLE TREE WITHOUT THE SIGMAS DEFINED.
			this.range = rangeTree;
			this.domain = domainTree;


		}
		//x0 x1 are the generators of Thompson group F
		//C D are the generators of Thompson group T
		public DoubleTree(char c) {
			switch (c) {
				case '0'://x0
					range = new TreeNode(leftNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode()), rightNode: new TreeNode());
					domain = new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode()));
					range.left.left.sigma = domain.left;
					range.left.right.sigma = domain.right.left;
					range.right.sigma = domain.right.right;
					break;

				case '2'://x0^-1
					range = new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode()));
					domain = new TreeNode(leftNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode()), rightNode: new TreeNode());
					range.left.sigma = domain.left.left;
					range.right.left.sigma = domain.left.right;
					range.right.right.sigma = domain.right;
					break;

				case '1'://x1
					range = new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode(leftNode: new TreeNode(leftNode:new TreeNode(),rightNode: new TreeNode()), rightNode: new TreeNode()));
					domain = new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode(leftNode:new TreeNode(), rightNode:new TreeNode(leftNode: new TreeNode(),rightNode: new TreeNode())));
					range.left.sigma = domain.left;
					range.right.left.left.sigma = domain.right.left;
					range.right.left.right.sigma = domain.right.right.left;
					range.right.right.sigma = domain.right.right.right;
					break;

				case '3'://x1^-1
					range = new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode())));
					domain = new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode(leftNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode()), rightNode: new TreeNode()));
					range.left.sigma = domain.left;
					range.right.left.sigma = domain.right.left.left;
					range.right.right.left.sigma = domain.right.left.right;
					range.right.right.right.sigma = domain.right.right;
					break;


				case 'C':
					range = new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode()));
					domain = new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode()));

					range.left.sigma = domain.right.left;
					range.right.left.sigma = domain.right.right;
					range.right.right.sigma = domain.left;

					break;

				case 'I'://C^-1 i.e C inverse
					range = new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode()));
					domain = new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode()));

					range.left.sigma = domain.right.right;
					range.right.left.sigma = domain.left;
					range.right.right.sigma = domain.right.left;
					
					break;

				case 'D': 
					range = new TreeNode(leftNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode()), rightNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode()));
					domain = new TreeNode(leftNode: new TreeNode(leftNode: new TreeNode(), rightNode:new TreeNode()), rightNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode()));

					range.left.left.sigma = domain.left.right;
					range.left.right.sigma = domain.right.left;
					range.right.left.sigma = domain.right.right;
					range.right.right.sigma = domain.left.left;

					break;
				case 'J': //D^-1 i.e. D inverse
					range = new TreeNode(leftNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode()), rightNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode()));
					domain = new TreeNode(leftNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode()), rightNode: new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode()));

					range.left.left.sigma = domain.right.right;
					range.left.right.sigma = domain.left.left;
					range.right.left.sigma = domain.left.right;
					range.right.right.sigma = domain.right.left;
					break;

				case 'K'://D^2=D^-2
					range = new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode());
					domain = new TreeNode(leftNode: new TreeNode(), rightNode: new TreeNode());
					range.left.sigma = domain.right;
					range.right.sigma = domain.left;
					break;
			}



		}

	}





}

