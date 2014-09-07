using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FL2NormOnFilesHalfSize
{
	static class DoubleTreeFunctions
    {
		//This program was originally written for composing elements of Thompson group T, using the generators C, D which are not the Cannon-Floyd-Perry generators.
		//The actions of C and D on elements of T (represented as reduced double trees with a rotation) will appear in future work by U. Haagerup K. K. Olesen and M. Ramirez-Solano.
		//This program is adapted to elements of F with standard generators x0 x1 and is inspired by the paper "Forest diagrams for elements of Thompson’s group F" by Belk & Brown.
		//The algorithm for the action of x0 x1 C or D can be generalized to the action of any element from T.

		/*
		//WARNING This function is a debugging function and it sabotages treenode.leaves which is obsolete for doubletrees but not for forests.
		static public void drawDoubleTreeToConsole(DoubleTree doubleTree) {
			Console.WriteLine("DRAWING:");
			Visual.drawTreeToConsole(doubleTree.range);
			Visual.drawTreeToConsoleFlipped(doubleTree.domain);
		}
		*/
		//Information about the doubleTree
		static public void Info(DoubleTree doubleTree) {
			Console.WriteLine("INFO:");
			//drawDoubleTreeToConsole(doubleTree);
			
			Console.WriteLine("LeafLabels= "+DoubleTreeLeafLabels(doubleTree)+" "+DoubleTreeLength(doubleTree));
		
			int[] exponentsOfLeaves = DoubleTreeNormalFormExp(doubleTree);
			int subindex = 0;
			int cnt=0;
			string normalform = "";
			foreach (int exp in exponentsOfLeaves) {
				if (exp != 0) 
					normalform += "x_" + (subindex + 1) + "^" + (exp);
					++cnt;
					if (2 * cnt < exponentsOfLeaves.Length) subindex = cnt; else subindex = exponentsOfLeaves.Length - cnt-1;
				
			}
			Console.WriteLine("normalform:" + normalform);


			int counter = 0;
			int leftcounter = 0;
			int rightcounter = 0;
			int depth = 0;
			string strdepthrange = "Leaf depth R: ";// depth of leaves.
			MyList myListR = new MyList();
			string strRLeaves = readTreePreorder(doubleTree.range, ref counter, ref leftcounter, ref rightcounter, true, ref myListR,depth,ref strdepthrange);

			counter = 0;
			leftcounter = 0;
			rightcounter = 0;
			depth = 0;
			string strdepthdomain = "Leaf depth D: ";//depth of leaves.
			MyList myListD = new MyList();
			string strDleaves = readTreePreorder(doubleTree.domain, ref counter, ref leftcounter, ref rightcounter, true, ref myListD,depth,ref strdepthdomain);

			MyNode cur = myListD.First;
			string strDomain = "Domain: ";
			while (cur != null) {
				strDomain = strDomain + cur.Value.leaves + " ";
				cur = cur.Next;
			}


			cur = myListR.First;
			string strRange = " Range: ";
			string strSigma = "";
			string str123 = ""; int ct = 1;

			while (cur != null) {
				strRange +=  cur.Value.leaves + " ";
				strSigma +=  cur.Value.sigma.leaves + " ";
				str123 +=   ct + " "; ++ct;
				cur = cur.Next;
			}

			//strSigma maps range to domain. we want domain to range
			string cat = str123 + str123;
			string strSigmaInverse = cat.Substring(strSigma.IndexOf('1'), strSigma.Length);
			

			Console.WriteLine(strRLeaves);


			Console.WriteLine(strRange + " Length= " + myListR.Length);
			Console.WriteLine(" sigma: "+strSigma + " Length= " + myListR.Length + " (Sigma:Range->Domain)");
			Console.WriteLine("   inv: "+strSigmaInverse + " Length= " + myListD.Length + " (SigmaInverse:Domain->Range )");
			Console.WriteLine(strDomain + " Length= " + myListD.Length);
			Console.WriteLine(strdepthrange + " Length= " + myListD.Length);
			Console.WriteLine(strdepthdomain + " Length= " + myListD.Length);

			Console.WriteLine(strDleaves);

			Console.WriteLine("doubleTree=" + DoubleTreeFunctions.DoubleTreeToString(doubleTree));
			Console.WriteLine("doubleTreeInverse=" + DoubleTreeFunctions.DoubleTreeToInverseToString(doubleTree));
			Console.WriteLine("doubleTreeToForest=" + DoubleTreeFunctions.DoubleTreeToForestToString(doubleTree));
			Console.WriteLine("doubleTreeInverseToForest=" + DoubleTreeFunctions.DoubleTreeToInverseToForestToString(doubleTree));


		}

		#region functions Ai Aiinv C D xi xiinv
		static public void functionA1inv(ref DoubleTree doubleTree) {
			functionCinv(ref doubleTree);
			functionDinv(ref doubleTree);
		}

		static public void functionA2inv(ref DoubleTree doubleTree) {
			functionCinv(ref doubleTree);
			functionDsqr(ref doubleTree);
		}

		static public void functionA3inv(ref DoubleTree doubleTree) {
			functionCinv(ref doubleTree);
			functionD(ref doubleTree);
		}

		static public void functionA4inv(ref DoubleTree doubleTree) {
			functionC(ref doubleTree);
			functionDinv(ref doubleTree);
		}

		static public void functionA5inv(ref DoubleTree doubleTree) {
			functionC(ref doubleTree);
			functionDsqr(ref doubleTree);
		}

		static public void functionA6inv(ref DoubleTree doubleTree) {
			functionC(ref doubleTree);
			functionD(ref doubleTree);
		}





		static public void functionA1(ref DoubleTree doubleTree) {
			functionD(ref doubleTree);
			functionC(ref doubleTree);
		}

		static public void functionA2(ref DoubleTree doubleTree) {
			functionDsqr(ref doubleTree);
			functionC(ref doubleTree);
		}

		static public void functionA3(ref DoubleTree doubleTree) {
			functionDinv(ref doubleTree);
			functionC(ref doubleTree);
		}

		static public void functionA4(ref DoubleTree doubleTree) {
			functionD(ref doubleTree);
			functionCinv(ref doubleTree);
		}

		static public void functionA5(ref DoubleTree doubleTree) {
			functionDsqr(ref doubleTree);
			functionCinv(ref doubleTree);
		}

		static public void functionA6(ref DoubleTree doubleTree) {
			functionDinv(ref doubleTree);
			functionCinv(ref doubleTree);
		}



		static public void functionX0(ref DoubleTree doubleTree) {
			//The action of X0 on a double tree with range:
			//     .
			//    / \ Ip
			//   /  /\
			//  I  II III
			// is the following:
			//     .
			//  n1/ \
			//   /\  \
			//  I II III

			//ALGORITHM:
			// (1) If (I,Ip) does not exist, then we have the identity and so we return just X0;
			// (2) If (II, III) dooes not exist then a caret is created on Ip.sigma which is the element in the domain mapping to Ip, which in this case is a leaf.
			//     If (I, II) and (I.sigma,II.sigma) form carets (hence they match; note: carets do not commute) then
			// (3)         create plain new node n1 with n1.sigma=I.sigma.parent; and remove caret (I.sigma,II.sigma)
			// (4) else create new node n1=(I,II);
			//     If (I,II) was reduced and III is a leaf (i.e. (n1,III) is a caret) and (n1.sigma,III.sigma) form a caret (hence they match) then
			// (5)             return the identity;
			// (6) else redefine range=(n1,III); 


			//(1) doubleTree is the identity
			if (doubleTree.range.left == null && doubleTree.domain.left == null) {
				doubleTree = new DoubleTree('0');//X0
				return;
			}

			TreeNode I = doubleTree.range.left;
			TreeNode Ip = doubleTree.range.right;

			//(2)
			if (Ip.left == null) {//add caret on leaf Ip and on leaf sigmainverse(Ip)
				TreeNode sigmainverseOfIp = Ip.sigma;
				sigmainverseOfIp.left = new TreeNode(parentNode: sigmainverseOfIp, currentNodeIsTheLeftChild: true);
				sigmainverseOfIp.right = new TreeNode(parentNode: sigmainverseOfIp, currentNodeIsTheLeftChild: false);
				Ip.left = new TreeNode(parentNode: Ip, currentNodeIsTheLeftChild: true, sigmaNode: sigmainverseOfIp.left);
				Ip.right = new TreeNode(parentNode: Ip, currentNodeIsTheLeftChild: false, sigmaNode: sigmainverseOfIp.right);
				Ip.sigma = null;
			}

			TreeNode II = doubleTree.range.right.left;
			TreeNode III = doubleTree.range.right.right;
			TreeNode n1 = null;
			bool pairIandIIwasReduced = false;
			if (I.left == null && II.right == null // (I,II) form a caret
				&& I.sigma.currentNodeIsTheLeftChild == true
				&& II.sigma.currentNodeIsTheLeftChild == false
				&& I.sigma.parent == II.sigma.parent // (I.sigma,II.sigma) for a caret
				) {//(3)
				n1 = new TreeNode(sigmaNode: I.sigma.parent);
				//removing bottom caret
				n1.sigma.left = null;
				n1.sigma.right = null;
				pairIandIIwasReduced = true;
			}
			else//(4)
				n1 = new TreeNode(leftNode: I, rightNode: II);

			if (pairIandIIwasReduced == true && III.left == null//(n1,III) form a caret
				&& n1.sigma.currentNodeIsTheLeftChild == true
				&& III.sigma.currentNodeIsTheLeftChild == false
				&& n1.sigma.parent == III.sigma.parent//(n1.sigma,III.sigma) form a caret
				) {//(5)
				doubleTree = new DoubleTree(); // The element is the identity
				return;
			}//6
			doubleTree.range = new TreeNode(leftNode: n1, rightNode: III);
			return;
		}


		static public void functionX0inv(ref DoubleTree doubleTree) {
			//The action of X0^-1 on a double tree with range:
			//     .
			//  Ip/ \
			//   /\  \
			//  I  II III
			// is the following:
			//     .
			//    / \n1
			//   /  /\
			//  I II III

			//ALGORITHM:
			// (1) If (Ip,III) does not exist, then we have the identity and so we return just X0^-1;
			// (2) If (I, II) dooes not exist then a caret is created on Ip.sigma which is the element in the domain mapping to Ip, which in this case is a leaf.
			//     If (II, III) and (II.sigma,III.sigma) form carets (hence they match; note: carets do not commute) then
			// (3)         create plain new node n1 with n1.sigma=II.sigma.parent; and remove caret (II.sigma,III.sigma)
			// (4) else create new node n1=(II,III);
			//     If (II,III) was reduced and I is a leaf (i.e. (I,n1) is a caret) and (I.sigma,n1.sigma) form a caret (hence they match) then
			// (5)             return the identity;
			// (6) else redefine range=(I,n1); 


			//(1) doubleTree is the identity
			if (doubleTree.range.left == null && doubleTree.domain.left == null) {
				doubleTree = new DoubleTree('2');//X0inv
				return;
			}

			TreeNode Ip = doubleTree.range.left;
			TreeNode III = doubleTree.range.right;

			//(2)
			if (Ip.left == null) {//add caret on leaf Ip and on leaf sigmainverse(Ip)
				TreeNode sigmainverseOfIp = Ip.sigma;
				sigmainverseOfIp.left = new TreeNode(parentNode: sigmainverseOfIp, currentNodeIsTheLeftChild: true);
				sigmainverseOfIp.right = new TreeNode(parentNode: sigmainverseOfIp, currentNodeIsTheLeftChild: false);
				Ip.left = new TreeNode(parentNode: Ip, currentNodeIsTheLeftChild: true, sigmaNode: sigmainverseOfIp.left);
				Ip.right = new TreeNode(parentNode: Ip, currentNodeIsTheLeftChild: false, sigmaNode: sigmainverseOfIp.right);
				Ip.sigma = null;
			}

			TreeNode I = doubleTree.range.left.left;
			TreeNode II = doubleTree.range.left.right;
			TreeNode n1 = null;
			bool pairIIandIIIwasReduced = false;
			if (II.left == null && III.right == null // (II,III) form a caret
				&& II.sigma.currentNodeIsTheLeftChild == true
				&& III.sigma.currentNodeIsTheLeftChild == false
				&& II.sigma.parent == III.sigma.parent // (II.sigma,III.sigma) for a caret
				) {//(3)
				n1 = new TreeNode(sigmaNode: II.sigma.parent);
				//removing bottom caret
				n1.sigma.left = null;
				n1.sigma.right = null;
				pairIIandIIIwasReduced = true;
			}
			else//(4)
				n1 = new TreeNode(leftNode: II, rightNode: III);

			if (pairIIandIIIwasReduced == true && I.left == null//(I,n1) form a caret
				&& I.sigma.currentNodeIsTheLeftChild == true
				&& n1.sigma.currentNodeIsTheLeftChild == false
				&& I.sigma.parent == n1.sigma.parent//(I.sigma,n1.sigma) form a caret
				) {//(5)
				doubleTree = new DoubleTree(); // The element is the identity
				return;
			}//6
			doubleTree.range = new TreeNode(leftNode: I, rightNode: n1);
			return;
		}


		static public void functionX1(ref DoubleTree doubleTree) {
			//The action of x1 on a double tree with range:
			//      /\
			//     /  \Ip
			//    /  / \Ipp
			//   /  /  /\
			//  I II III IV
			// is the following:
			//      /\
			//     /  \n2
			//    /n1/ \
			//   /  /\  \
			//  I II III IV

			//ALGORITHM:
			// (1) If (I,Ip) does not exist, then we have the identity and so we return just x1;
			// (2) If (II, Ipp) does not exist then create a caret on Ip.sigma which is the element in the domain that maps to Ip, which in this case is a leaf.
			// (3) If (III, IV) does not exist then create a caret on Ipp.sigma which is the element in the domain that maps to Ipp, which in this case is a leaf.
			//     If (II, III) and (II.sigma,III.sigma) form carets (hence they match; Note: carets do not commute ) then 
			// (4)      create a plain new node n1 with n1.sigma=II.sigma.parent and remove caret (II.sigma,III.sigma).
			// (5) else create node n1=(II,III)
			//     If (II,III) was reduced and IV is a leaf (i.e. (n1,IV) is a caret) and (n1.sigma,IV.sigma) is a caret 
			// (6)      create a plain new node n2 with n2.sigma=IV.sigma.parent and remove caret (n1.sigma,IV.sigma).
			// (7) else create node n2=(n1,IV)     
   			//     if (n1,IV) was reduced and I is a leaf (i.e. (I,n2) is a caret) and  (I.sigma,n2.sigma) is a caret then 
			// (8)     return the identity    
			// (9) else redefine range=(I,n2) and return


			//(1) doubleTree is the identity
			if (doubleTree.range.left == null && doubleTree.domain.left == null) {
				doubleTree = new DoubleTree('1');
				return;
			}

			TreeNode I = doubleTree.range.left;
			TreeNode Ip = doubleTree.range.right;

			//(2)
			if (Ip.left == null) {//add caret on leaf Ip and on leaf sigmainverse(Ip)
				TreeNode sigmainverseOfIp = Ip.sigma;
				sigmainverseOfIp.left = new TreeNode(parentNode: sigmainverseOfIp, currentNodeIsTheLeftChild: true);
				sigmainverseOfIp.right = new TreeNode(parentNode: sigmainverseOfIp, currentNodeIsTheLeftChild: false);
				Ip.left = new TreeNode(parentNode: Ip, currentNodeIsTheLeftChild: true, sigmaNode: sigmainverseOfIp.left);
				Ip.right = new TreeNode(parentNode: Ip, currentNodeIsTheLeftChild: false, sigmaNode: sigmainverseOfIp.right);
				Ip.sigma = null;
			}

			TreeNode II = Ip.left;
			TreeNode Ipp = Ip.right;
			//(3)
			if (Ipp.left == null) {//add caret on leaf Ipp and on leaf sigmainverse(Ipp)
				TreeNode sigmainverseOfIpp = Ipp.sigma;
				sigmainverseOfIpp.left = new TreeNode(parentNode: sigmainverseOfIpp, currentNodeIsTheLeftChild: true);
				sigmainverseOfIpp.right = new TreeNode(parentNode: sigmainverseOfIpp, currentNodeIsTheLeftChild: false);
				Ipp.left = new TreeNode(parentNode: Ipp, currentNodeIsTheLeftChild: true, sigmaNode: sigmainverseOfIpp.left);
				Ipp.right = new TreeNode(parentNode: Ipp, currentNodeIsTheLeftChild: false, sigmaNode: sigmainverseOfIpp.right);
				Ipp.sigma = null;
			}

			TreeNode III = Ipp.left;
			TreeNode IV = Ipp.right;
			TreeNode n1 = null;
			TreeNode n2 = null;
			bool pairIIandIIIwasReduced = false;
			bool pairn2andIVwasReduced = false;

			if (II.left == null && III.right == null // (II,III) form a caret
				&& II.sigma.currentNodeIsTheLeftChild == true
				&& III.sigma.currentNodeIsTheLeftChild == false
				&& II.sigma.parent == III.sigma.parent // (II.sigma,III.sigma) form a caret
				) {//(4)
				n1 = new TreeNode(sigmaNode: II.sigma.parent);
				//removing bottom caret
				n1.sigma.left = null;
				n1.sigma.right = null;
				pairIIandIIIwasReduced = true;
			}
			else//(5)
				n1 = new TreeNode(leftNode: II, rightNode: III);


			if (pairIIandIIIwasReduced == true && IV.left==null//(n1,IV) form a caret
				&& n1.sigma.currentNodeIsTheLeftChild == true
				&& IV.sigma.currentNodeIsTheLeftChild == false
				&& n1.sigma.parent == IV.sigma.parent//(n1.sigma,IV.sigma) form a caret
				) {//(6)
				n2 = new TreeNode(sigmaNode: IV.sigma.parent);
				//removing bottom caret
				n2.sigma.left = null;
				n2.sigma.right = null;
				pairn2andIVwasReduced = true;			
			}
			else//(7)
				n2 = new TreeNode(leftNode: n1, rightNode: IV);

			

			if (I.left == null && pairn2andIVwasReduced == true//(I,n2) form a caret
				&& I.sigma.currentNodeIsTheLeftChild == true
				&& n2.sigma.currentNodeIsTheLeftChild == false
				&& I.sigma.parent == n2.sigma.parent//(I.sigma,n2.sigma) form a caret
				) {//(8)
				doubleTree = new DoubleTree(); // The element is the identity
				return;
			}
			//(9)
			doubleTree.range = new TreeNode(leftNode: I, rightNode: n2);
			return;
		}

		static public void functionX1inv(ref DoubleTree doubleTree) {
			//The action of x1^-1 on a double tree with range:
			//       .
			//      / \
			//     /   \Ip
			//    /Ipp/ \
			//   /   /\  \
			//  I  II III IV
			// is the following:
			//      /\
			//     /  \n2
			//    /  / \n1
			//   /  /  /\
			//  I II III IV

			//ALGORITHM:
			// (1) If (I,Ip) does not exist, then we have the identity and so we return just x1;
			// (2) If (Ipp, IV) does not exist then create a caret on Ip.sigma which is the element in the domain that maps to Ip, which in this case is a leaf.
			// (3) If (II, III) does not exist then create a caret on Ipp.sigma which is the element in the domain that maps to Ipp, which in this case is a leaf.
			//     If (III, IV) and (III.sigma,IV.sigma) form carets (hence they match; Note: carets do not commute ) then 
			// (4)      create a plain new node n1 with n1.sigma=III.sigma.parent and remove caret (III.sigma,IV.sigma).
			// (5) else create node n1=(III,IV)
			//     If (III,IV) was reduced and II is a leaf (i.e. (II,n1) is a caret) and (II.sigma,n1.sigma) is a caret 
			// (6)      create a plain new node n2 with n2.sigma=II.sigma.parent and remove caret (II.sigma,n1.sigma).
			// (7) else create node n2=(II,n1)     
			//     if (II,n1) was reduced and I is a leaf (i.e. (I,n2) is a caret) and  (I.sigma,n2.sigma) is a caret then 
			// (8)     return the identity    
			// (9) else redefine range=(I,n2) and return


			//(1) doubleTree is the identity
			if (doubleTree.range.left == null && doubleTree.domain.left == null) {
				doubleTree = new DoubleTree('3');
				return;
			}

			TreeNode I = doubleTree.range.left;
			TreeNode Ip = doubleTree.range.right;

			//(2)
			if (Ip.left == null) {//add caret on leaf Ip and on leaf sigmainverse(Ip)
				TreeNode sigmainverseOfIp = Ip.sigma;
				sigmainverseOfIp.left = new TreeNode(parentNode: sigmainverseOfIp, currentNodeIsTheLeftChild: true);
				sigmainverseOfIp.right = new TreeNode(parentNode: sigmainverseOfIp, currentNodeIsTheLeftChild: false);
				Ip.left = new TreeNode(parentNode: Ip, currentNodeIsTheLeftChild: true, sigmaNode: sigmainverseOfIp.left);
				Ip.right = new TreeNode(parentNode: Ip, currentNodeIsTheLeftChild: false, sigmaNode: sigmainverseOfIp.right);
				Ip.sigma = null;
			}

			TreeNode Ipp = Ip.left;
			TreeNode IV = Ip.right;
			//(3)
			if (Ipp.left == null) {//add caret on leaf Ipp and on leaf sigmainverse(Ipp)
				TreeNode sigmainverseOfIpp = Ipp.sigma;
				sigmainverseOfIpp.left = new TreeNode(parentNode: sigmainverseOfIpp, currentNodeIsTheLeftChild: true);
				sigmainverseOfIpp.right = new TreeNode(parentNode: sigmainverseOfIpp, currentNodeIsTheLeftChild: false);
				Ipp.left = new TreeNode(parentNode: Ipp, currentNodeIsTheLeftChild: true, sigmaNode: sigmainverseOfIpp.left);
				Ipp.right = new TreeNode(parentNode: Ipp, currentNodeIsTheLeftChild: false, sigmaNode: sigmainverseOfIpp.right);
				Ipp.sigma = null;
			}

			TreeNode II = Ipp.left;
			TreeNode III = Ipp.right;
			TreeNode n1 = null;
			TreeNode n2 = null;
			bool pairIIIandIVwasReduced = false;
			bool pairIIandn1wasReduced = false;

			if (III.left == null && IV.right == null // (III,IV) form a caret
				&& III.sigma.currentNodeIsTheLeftChild == true
				&& IV.sigma.currentNodeIsTheLeftChild == false
				&& III.sigma.parent == IV.sigma.parent // (III.sigma,IV.sigma) form a caret
				) {//(4)
				n1 = new TreeNode(sigmaNode: III.sigma.parent);
				//removing bottom caret
				n1.sigma.left = null;
				n1.sigma.right = null;
				pairIIIandIVwasReduced = true;
			}
			else//(5)
				n1 = new TreeNode(leftNode: III, rightNode: IV);


			if (pairIIIandIVwasReduced == true && II.left == null//(II,n1) form a caret
				&& II.sigma.currentNodeIsTheLeftChild == true
				&& n1.sigma.currentNodeIsTheLeftChild == false
				&& II.sigma.parent == n1.sigma.parent//(II.sigma,n1.sigma) form a caret
				) {//(6)
				n2 = new TreeNode(sigmaNode: II.sigma.parent);
				//removing bottom caret
				n2.sigma.left = null;
				n2.sigma.right = null;
				pairIIandn1wasReduced = true;
			}
			else//(7)
				n2 = new TreeNode(leftNode: II, rightNode: n1);



			if (I.left == null && pairIIandn1wasReduced == true//(I,n2) form a caret
				&& I.sigma.currentNodeIsTheLeftChild == true
				&& n2.sigma.currentNodeIsTheLeftChild == false
				&& I.sigma.parent == n2.sigma.parent//(I.sigma,n2.sigma) form a caret
				) {//(8)
				doubleTree = new DoubleTree(); // The element is the identity
				return;
			}
			//(9)
			doubleTree.range = new TreeNode(leftNode: I, rightNode: n2);
			return;
		}


		static public void functionC(ref DoubleTree doubleTree) {
			//The action of C on a double tree with range:
			//     .
			//    / \Ip
			//   /  /\
			//  I  II III
			// is the following:
			//     .
			//    / \n1
			//   /  /\
			// II III I

			//ALGORITHM:
			// (1) If (I,Ip) does not exist, then we have the identity and so we return just C;
			// (2) If (II, III) dooes not exist then a caret is created on Ip.sigma which is the element in the domain mapping to Ip, which in this case is a leaf.
			//     If (III, I) and (III.sigma,I.sigma) form carets (hence they match; note: carets do not commute) then
			// (3)         create plain new node n1 with n1.sigma=III.sigma.parent; and remove caret (III.sigma,I.sigma)
			// (4) else create new node n1=(III,I);
			//     if (III,I) was reduced and II is a leaf (i.e. (II,n1) is a caret) and (II.sigma,n1.sigma) form a caret (hence they match) then
			// (5)             return the identity;
			// (6) else redefine range=(II,n1); 


			//(1) doubleTree is the identity
			if (doubleTree.range.left == null && doubleTree.domain.left == null) {
				doubleTree = new DoubleTree('C');
				return;
			}

			TreeNode I = doubleTree.range.left;
			TreeNode Ip = doubleTree.range.right;

			//(2)
			if (Ip.left == null) {//add caret on leaf Ip and on leaf sigmainverse(Ip)
				TreeNode sigmainverseOfIp = Ip.sigma;
				sigmainverseOfIp.left = new TreeNode(parentNode: sigmainverseOfIp, currentNodeIsTheLeftChild: true);
				sigmainverseOfIp.right = new TreeNode(parentNode: sigmainverseOfIp, currentNodeIsTheLeftChild: false);
				Ip.left = new TreeNode(parentNode: Ip, currentNodeIsTheLeftChild: true, sigmaNode: sigmainverseOfIp.left);
				Ip.right = new TreeNode(parentNode: Ip, currentNodeIsTheLeftChild: false, sigmaNode: sigmainverseOfIp.right);
				Ip.sigma = null;
			}

			TreeNode II = doubleTree.range.right.left;
			TreeNode III = doubleTree.range.right.right;
			TreeNode n1 = null;
			bool pairIIIandIwasReduced = false;
			if (III.left == null && I.right == null // (III,I) form a caret
				&& III.sigma.currentNodeIsTheLeftChild == true
				&& I.sigma.currentNodeIsTheLeftChild == false
				&& III.sigma.parent == I.sigma.parent // (III.sigma,I.sigma) for a caret
				) {//(3)
				n1 = new TreeNode(sigmaNode: III.sigma.parent);
				//removing bottom caret
				n1.sigma.left = null;
				n1.sigma.right = null;
				pairIIIandIwasReduced = true;
			}
			else//(4)
				n1 = new TreeNode(leftNode: III, rightNode: I);

			if (pairIIIandIwasReduced == true && II.left == null//(II,n1) form a caret
				&& II.sigma.currentNodeIsTheLeftChild == true
				&& n1.sigma.currentNodeIsTheLeftChild == false
				&& II.sigma.parent == n1.sigma.parent//(II.sigma,n1.sigma) form a caret
				) {//(5)
				doubleTree = new DoubleTree(); // The element is the identity
				return;
			}//6
			doubleTree.range = new TreeNode(leftNode: II, rightNode: n1);
			return;
		}


		static public void functionCinv(ref DoubleTree doubleTree) {
			//The action of C^-1 on a double tree with range:
			//     .
			//    / \Ip
			//   /  /\
			//  I  II III
			// is the following:
			//     .
			//    / \n1
			//   /  /\
			// III I II

			//ALGORITHM:
			// (1) If (I,Ip) does not exist, then we have the identity and so we return just C^-1;
			// (2) If (II, III) dooes not exist then a caret is created on Ip.sigma which is the element in the domain mapping to Ip, which in this case is a leaf.
			//     If (I, II) and (I.sigma,II.sigma) form carets (hence they match; note: carets do not commute) then
			// (3)         create plain new node n1 with n1.sigma=I.sigma.parent; and remove caret (I.sigma,II.sigma)
			// (4) else create new node n1=(I,II);
			//     if (I,II) was reduced and III is a leaf (i.e. (III,n1) is a caret) and (III.sigma,n1.sigma) form a caret (hence they match) then
			// (5)             return the identity;
			// (6) else redefine range=(III,n1); 


			//(1) doubleTree is the identity
			if (doubleTree.range.left == null && doubleTree.domain.left == null) {
				doubleTree = new DoubleTree('I');
				return;
			}

			TreeNode I = doubleTree.range.left;
			TreeNode Ip = doubleTree.range.right;

			//(2)
			if (Ip.left == null) {//add caret on leaf Ip and on leaf sigmainverse(Ip)
				TreeNode sigmainverseOfIp = Ip.sigma;
				sigmainverseOfIp.left = new TreeNode(parentNode: sigmainverseOfIp, currentNodeIsTheLeftChild: true);
				sigmainverseOfIp.right = new TreeNode(parentNode: sigmainverseOfIp, currentNodeIsTheLeftChild: false);
				Ip.left = new TreeNode(parentNode: Ip, currentNodeIsTheLeftChild: true, sigmaNode: sigmainverseOfIp.left);
				Ip.right = new TreeNode(parentNode: Ip, currentNodeIsTheLeftChild: false, sigmaNode: sigmainverseOfIp.right);
				Ip.sigma = null;
			}

			TreeNode II = doubleTree.range.right.left;
			TreeNode III = doubleTree.range.right.right;
			TreeNode n1 = null;
			bool pairIandIIwasReduced = false;
			if (I.left == null && II.right == null // (I,II) form a caret
				&& I.sigma.currentNodeIsTheLeftChild == true
				&& II.sigma.currentNodeIsTheLeftChild == false
				&& I.sigma.parent == II.sigma.parent // (I.sigma,II.sigma) for a caret
				) {//(3)
				n1 = new TreeNode(sigmaNode: I.sigma.parent);
				//removing bottom caret
				n1.sigma.left = null;
				n1.sigma.right = null;
				pairIandIIwasReduced = true;
			}
			else//(4)
				n1 = new TreeNode(leftNode: I, rightNode: II);

			if (pairIandIIwasReduced == true && III.left == null//(III,n1) form a caret
				&& III.sigma.currentNodeIsTheLeftChild == true
				&& n1.sigma.currentNodeIsTheLeftChild == false
				&& III.sigma.parent == n1.sigma.parent//(III.sigma,n1.sigma) form a caret
				) {//(5)
				doubleTree = new DoubleTree(); // The element is the identity
				return;
			}//(6)
			doubleTree.range = new TreeNode(leftNode: III, rightNode: n1);
			return;
		}

		static public void functionD(ref DoubleTree doubleTree) {
			//The action of D on a double tree with range:
			//      /\
			//     /  \
			// Ip /    \Ipp
			//   /\    /\
			//  I II III IV
			// is the following:
			//      /\
			//     /  \
			//  n1/    \n2
			//   /\    /\
			// II III IV I

			//ALGORITHM:
			// (1) If (Ip,Ipp) does not exist, then we have the identity and so we return just D;
			// (2) If (I, II) dooes not exist then create a caret on Ip.sigma which is the element in the domain that maps to Ip, which in this case is a leaf.
			// (3) If (III, IV) dooes not exist then create a caret on Ip.sigma which is the element in the domain that maps to Ipp, which in this case is a leaf.
			//     If (II, III) and (II.sigma,III.sigma) form carets (hence they match; Note: carets do not commute ) then 
			// (4)      create a plain new node n1 with n1.sigma=II.sigma.parent and remove caret (II.sigma,III.sigma).
			// (5) else create node n1=(II,III)
			//     If (IV,I) and (IV.sigma,I.sigma) form carets (hence they match ) then
			// (6)      create new node n2 with n2.sigma=IV.sigma.parent and remove caret: (IV.sigma, I.sigma), 
			// (7)    else create node n2=(IV,I)
			//     If both (II,III) and (IV,I) were reduced and (n1.sigma,n2.sigma) is a caret 
			// (8)                  return identity;
			// (9) redefine range=(n1,n2)  and return


			//(1) doubleTree is the identity
			if (doubleTree.range.left == null && doubleTree.domain.left == null) {
				doubleTree = new DoubleTree('D');
				return;
			}

			TreeNode Ip = doubleTree.range.left;
			TreeNode Ipp = doubleTree.range.right;

			//(2)
			if (Ip.left == null) {//add caret on leaf Ip and on leaf sigmainverse(Ip)
				TreeNode sigmainverseOfIp = Ip.sigma;
				sigmainverseOfIp.left = new TreeNode(parentNode: sigmainverseOfIp, currentNodeIsTheLeftChild: true);
				sigmainverseOfIp.right = new TreeNode(parentNode: sigmainverseOfIp, currentNodeIsTheLeftChild: false);
				Ip.left = new TreeNode(parentNode: Ip, currentNodeIsTheLeftChild: true, sigmaNode: sigmainverseOfIp.left);
				Ip.right = new TreeNode(parentNode: Ip, currentNodeIsTheLeftChild: false, sigmaNode: sigmainverseOfIp.right);
				Ip.sigma = null;
			}
			//(3)
			if (Ipp.left == null) {//add caret on leaf Ipp and on leaf sigmainverse(Ipp)
				TreeNode sigmainverseOfIpp = Ipp.sigma;
				sigmainverseOfIpp.left = new TreeNode(parentNode: sigmainverseOfIpp, currentNodeIsTheLeftChild: true);
				sigmainverseOfIpp.right = new TreeNode(parentNode: sigmainverseOfIpp, currentNodeIsTheLeftChild: false);
				Ipp.left = new TreeNode(parentNode: Ipp, currentNodeIsTheLeftChild: true, sigmaNode: sigmainverseOfIpp.left);
				Ipp.right = new TreeNode(parentNode: Ipp, currentNodeIsTheLeftChild: false, sigmaNode: sigmainverseOfIpp.right);
				Ipp.sigma = null;
			}

			TreeNode I = doubleTree.range.left.left;
			TreeNode II = doubleTree.range.left.right;
			TreeNode III = doubleTree.range.right.left;
			TreeNode IV = doubleTree.range.right.right;
			TreeNode n1 = null;
			TreeNode n2 = null;
			bool pairIIandIIIwasReduced = false;
			bool pairIVandIwasReduced = false;

			if (II.left == null && III.right == null // (II,III) form a caret
				&& II.sigma.currentNodeIsTheLeftChild == true
				&& III.sigma.currentNodeIsTheLeftChild == false
				&& II.sigma.parent == III.sigma.parent // (III.sigma,I.sigma) form a caret
				) {//(4)
				n1 = new TreeNode(sigmaNode: II.sigma.parent);
				//removing bottom caret
				n1.sigma.left = null;
				n1.sigma.right = null;
				pairIIandIIIwasReduced = true;
			}
			else//(5)
				n1 = new TreeNode(leftNode: II, rightNode: III);

			if (IV.left == null && I.right == null // (IV,I) form a caret
				&& IV.sigma.currentNodeIsTheLeftChild == true
				&& I.sigma.currentNodeIsTheLeftChild == false
				&& IV.sigma.parent == I.sigma.parent // (IV.sigma,I.sigma) form a caret
				) {//(6)
				n2 = new TreeNode(sigmaNode: IV.sigma.parent);
				//removing bottom caret
				n2.sigma.left = null;
				n2.sigma.right = null;
				pairIVandIwasReduced = true;
			}
			else//(7)
				n2 = new TreeNode(leftNode: IV, rightNode: I);

			if (pairIIandIIIwasReduced == true && pairIVandIwasReduced == true//(n1,n2) form a caret
				&& n1.sigma.currentNodeIsTheLeftChild == true
				&& n2.sigma.currentNodeIsTheLeftChild == false
				&& n1.sigma.parent == n2.sigma.parent//(n1.sigma,n2.sigma) form a caret
				) {//(8)
				doubleTree = new DoubleTree(); // The element is the identity
				return;
			}
			//(9)
			doubleTree.range = new TreeNode(leftNode: n1, rightNode: n2);
			return;
		}


		static public void functionDinv(ref DoubleTree doubleTree) {
			//The action of D^-1 on a double tree with range:
			//      /\
			//     /  \
			// Ip /    \Ipp
			//   /\    /\
			//  I II III IV
			// is the following:
			//      /\
			//     /  \
			//  n1/    \n2
			//   /\    /\
			//  IV I II III 

			//ALGORITHM:
			// (1) If (Ip,Ipp) does not exist, then we have the identity and so we return just D^-1;
			// (2) If (I, II) dooes not exist then create a caret on Ip.sigma which is the element in the domain that maps to Ip, which in this case is a leaf.
			// (3) If (III, IV) dooes not exist then create a caret on Ip.sigma which is the element in the domain that maps to Ipp, which in this case is a leaf.
			//     If (IV, I) and (IV.sigma,I.sigma) form carets (hence they match; Note: carets do not commute ) then 
			// (4)      create a plain new node n1 with n1.sigma=IV.sigma.parent and remove caret (IV.sigma,I.sigma).
			// (5) else create node n1=(IV,I)
			//     If (II,III) and (II.sigma,III.sigma) form carets (hence they match ) then
			// (6)      create new node n2 with n2.sigma=II.sigma.parent and remove caret: (II.sigma, III.sigma), 
			// (7)    else create node n2=(II,III)
			//     If both (IV,I) and (II,III) were reduced and (n1.sigma,n2.sigma) is a caret 
			// (8)                  return identity;
			// (9) redefine range=(n1,n2)  and return


			//(1) doubleTree is the identity
			if (doubleTree.range.left == null && doubleTree.domain.left == null) {
				doubleTree = new DoubleTree('J');
				return;
			}

			TreeNode Ip = doubleTree.range.left;
			TreeNode Ipp = doubleTree.range.right;

			//(2)
			if (Ip.left == null) {//add caret on leaf Ip and on leaf sigmainverse(Ip)
				TreeNode sigmainverseOfIp = Ip.sigma;
				sigmainverseOfIp.left = new TreeNode(parentNode: sigmainverseOfIp, currentNodeIsTheLeftChild: true);
				sigmainverseOfIp.right = new TreeNode(parentNode: sigmainverseOfIp, currentNodeIsTheLeftChild: false);
				Ip.left = new TreeNode(parentNode: Ip, currentNodeIsTheLeftChild: true, sigmaNode: sigmainverseOfIp.left);
				Ip.right = new TreeNode(parentNode: Ip, currentNodeIsTheLeftChild: false, sigmaNode: sigmainverseOfIp.right);
				Ip.sigma = null;
			}
			//(3)
			if (Ipp.left == null) {//add caret on leaf Ipp and on leaf sigmainverse(Ipp)
				TreeNode sigmainverseOfIpp = Ipp.sigma;
				sigmainverseOfIpp.left = new TreeNode(parentNode: sigmainverseOfIpp, currentNodeIsTheLeftChild: true);
				sigmainverseOfIpp.right = new TreeNode(parentNode: sigmainverseOfIpp, currentNodeIsTheLeftChild: false);
				Ipp.left = new TreeNode(parentNode: Ipp, currentNodeIsTheLeftChild: true, sigmaNode: sigmainverseOfIpp.left);
				Ipp.right = new TreeNode(parentNode: Ipp, currentNodeIsTheLeftChild: false, sigmaNode: sigmainverseOfIpp.right);
				Ipp.sigma = null;
			}

			TreeNode I = doubleTree.range.left.left;
			TreeNode II = doubleTree.range.left.right;
			TreeNode III = doubleTree.range.right.left;
			TreeNode IV = doubleTree.range.right.right;
			TreeNode n1 = null;
			TreeNode n2 = null;
			bool pairIVandIwasReduced = false;
			bool pairIIandIIIwasReduced = false;


			if (IV.left == null && I.right == null // (IV,I) form a caret
				&& IV.sigma.currentNodeIsTheLeftChild == true
				&& I.sigma.currentNodeIsTheLeftChild == false
				&& IV.sigma.parent == I.sigma.parent // (IV.sigma,I.sigma) form a caret
				) {//(4)
				n1 = new TreeNode(sigmaNode: IV.sigma.parent);
				//removing bottom caret
				n1.sigma.left = null;
				n1.sigma.right = null;
				pairIVandIwasReduced = true;
			}
			else//(5)
				n1 = new TreeNode(leftNode: IV, rightNode: I);

			if (II.left == null && III.right == null // (II,III) form a caret
				&& II.sigma.currentNodeIsTheLeftChild == true
				&& III.sigma.currentNodeIsTheLeftChild == false
				&& II.sigma.parent == III.sigma.parent // (III.sigma,I.sigma) form a caret
				) {//(6)
				n2 = new TreeNode(sigmaNode: II.sigma.parent);
				//removing bottom caret
				n2.sigma.left = null;
				n2.sigma.right = null;
				pairIIandIIIwasReduced = true;
			}
			else//(7)
				n2 = new TreeNode(leftNode: II, rightNode: III);

			if (pairIVandIwasReduced == true && pairIIandIIIwasReduced == true//(n1,n2) form a caret
				&& n1.sigma.currentNodeIsTheLeftChild == true
				&& n2.sigma.currentNodeIsTheLeftChild == false
				&& n1.sigma.parent == n2.sigma.parent//(n1.sigma,n2.sigma) form a caret
				) {//(8)
				doubleTree = new DoubleTree(); // The element is the identity
				return;
			}
			//(9)
			doubleTree.range = new TreeNode(leftNode: n1, rightNode: n2);
			return;
		}


		static public void functionDsqr(ref DoubleTree doubleTree) {
			//The action of D^2 on a double tree with range:
			//     .
			//    / \
			//   I  II 
			// is the following:
			//     .
			//    / \
			//   II  I

			//ALGORITHM:
			// (1) If (I,II) does not exist, then we have the identity and so we return just D^2;
			//     If (II, I) and (II.sigma,I.sigma) form carets (hence they match; note: carets do not commute) then
			// (2)     return identity;
			// (3) else redefine range=(II,n1); 

			//(1) doubleTree is the identity
			if (doubleTree.range.left == null && doubleTree.domain.left == null) {
				doubleTree = new DoubleTree('K');
				return;
			}

			TreeNode I = doubleTree.range.left;
			TreeNode II = doubleTree.range.right;


			if (II.left == null && I.right == null // (II,I) form a caret
				&& II.sigma.currentNodeIsTheLeftChild == true
				&& I.sigma.currentNodeIsTheLeftChild == false
				&& II.sigma.parent == I.sigma.parent // (II.sigma,I.sigma) form a caret
				) {//(2)
				doubleTree = new DoubleTree(); // doubleTree is assigned the identity
				return;
			}
			//(3)

			doubleTree.range = new TreeNode(leftNode: II, rightNode: I);

			return;
		}

		#endregion

		//*** Ai,xi,C,D-functions on tuple of integers 0 to 5 **
		#region TupleOnForest read in traditioanal way
		//applies tuple to doubleTree. i.e. if tuple=100 then doubleTree is changed to x1x0x0(f).
		//NOTICE THAT the Ai-functions ARE APPLIED TO doubleTree FROM RIGHT TO LEFT i.e in traditional way.
		static public void functionAonTuple(ref byte[] tuple, ref DoubleTree doubleTree) {
			for (int k = tuple.Length - 1; k >= 0; k--) {
				if (tuple[k] == 0) functionA1(ref doubleTree); // functions: 0=A1 1=A2 2=A3 3=A4 4=A5  5=A6
				if (tuple[k] == 1) functionA2(ref doubleTree);
				if (tuple[k] == 2) functionA3(ref doubleTree);
				if (tuple[k] == 3) functionA4(ref doubleTree);
				if (tuple[k] == 4) functionA5(ref doubleTree);
				if (tuple[k] == 5) functionA6(ref doubleTree);
			}

		}
		//overload of previous function TupleOnForest receiving string instead of array of bytes.
		static public void functionAonTuple(string tuplestr, ref DoubleTree doubleTree) {
			char[] tuplechar = tuplestr.ToCharArray();
			byte[] tuple = new byte[tuplechar.Length];
			for (int i = 0; i < tuplechar.Length; i++) {
				tuple[i] = (byte)char.GetNumericValue(tuplechar[i]);
			}
			// Console.WriteLine("tuplestr to byte[] is {"+string.Join(",",tuple)+"}");
			functionAonTuple(ref tuple, ref doubleTree);
		}

		//applies tuple to doubleTree. i.e. if tuple=100 then doubleTree is changed to A1inv A0inv A1inv(doubletree).
		//NOTICE THAT the Ainv functions ARE APPLIED TO doubleTree FROM LEFT TO Right i.e 
		//in traditional way (a1a2)^-1=a2^-1a1^-1 thus you apply first a1inv then a2inv etc
		static public void functionAinvOnTuple(ref byte[] tuple, ref DoubleTree doubleTree) {
			for (int k = 0; k < tuple.Length; k++) {
				if (tuple[k] == 0) functionA1inv(ref doubleTree); // functions: 0=A1inv 1=A2inv 2=A3inv 3=A4inv 4=A5inv  5=A6inv
				if (tuple[k] == 1) functionA2inv(ref doubleTree);
				if (tuple[k] == 2) functionA3inv(ref doubleTree);
				if (tuple[k] == 3) functionA4inv(ref doubleTree);
				if (tuple[k] == 4) functionA5inv(ref doubleTree);
				if (tuple[k] == 5) functionA6inv(ref doubleTree);
			}

		}
		//overload of previous function TupleOnForest receiving string instead of array of bytes.
		static public void functionAinvOnTuple(string tuplestr, ref DoubleTree doubleTree) {
			char[] tuplechar = tuplestr.ToCharArray();
			byte[] tuple = new byte[tuplechar.Length];
			for (int i = 0; i < tuplechar.Length; i++) {
				tuple[i] = (byte)char.GetNumericValue(tuplechar[i]);
			}
			// Console.WriteLine("tuplestr to byte[] is {"+string.Join(",",tuple)+"}");
			functionAinvOnTuple(ref tuple, ref doubleTree);
		}



		//***CD-functions to tuple of integers 0 to 5  where 0=C 1=D 2=C^-1 3=D^2  4=D^-1**
		//applies tuple to doubleTree. i.e. if tuple=100 then doubleTree is changed to x1x0x0(f).
		static public void functionCDonTuple(ref byte[] tuple, ref DoubleTree doubleTree) {
			for (int k = tuple.Length - 1; k >= 0; k--) {
				if (tuple[k] == 0) functionC(ref doubleTree); // functions:  0=C 1=D 2=C^-1 3=D^2  4=D^-1
				if (tuple[k] == 1) functionD(ref doubleTree);
				if (tuple[k] == 2) functionCinv(ref doubleTree);
				if (tuple[k] == 3) functionDsqr(ref doubleTree);
				if (tuple[k] == 4) functionDinv(ref doubleTree);
			}

		}
		//overload of previous function TupleOnForest receiving string instead of array of bytes.
		static public void functionCDonTuple(string tuplestr, ref DoubleTree doubleTree) {
			char[] tuplechar = tuplestr.ToCharArray();
			byte[] tuple = new byte[tuplechar.Length];
			for (int i = 0; i < tuplechar.Length; i++) {
				tuple[i] = (byte)char.GetNumericValue(tuplechar[i]);
			}
			// Console.WriteLine("tuplestr to byte[] is {"+string.Join(",",tuple)+"}");
			functionCDonTuple(ref tuple, ref doubleTree);
		}



		//***AB-functions to tuple of integers 0 to 5  where 0=x0 1=x1 2=x0^-1 3=x1^-1 **
		//applies tuple to doubleTree. i.e. if tuple=100 then doubleTree is changed to x1x0x0(f).
		static public void functionX0X1onTuple(ref byte[] tuple, ref DoubleTree doubleTree) {
			for (int k = tuple.Length - 1; k >= 0; k--) {
				if (tuple[k] == 0) functionX0(ref doubleTree); // functions:  0=x0 1=x1 2=x0^-1 3=x1^-1
				if (tuple[k] == 1) functionX1(ref doubleTree);
				if (tuple[k] == 2) functionX0inv(ref doubleTree);
				if (tuple[k] == 3) functionX1inv(ref doubleTree);
			}

		}
		//overload of previous function TupleOnForest receiving string instead of array of bytes.
		static public void functionX0X1onTuple(string tuplestr, ref DoubleTree doubleTree) {
			char[] tuplechar = tuplestr.ToCharArray();
			byte[] tuple = new byte[tuplechar.Length];
			for (int i = 0; i < tuplechar.Length; i++) {
				tuple[i] = (byte)char.GetNumericValue(tuplechar[i]);
			}
			// Console.WriteLine("tuplestr to byte[] is {"+string.Join(",",tuple)+"}");
			functionX0X1onTuple(ref tuple, ref doubleTree);
		}

		#endregion



		//Reads a tree in preorder in recursive mode: See http://en.wikipedia.org/wiki/Tree_traversal
		//stores the leafs in myList
		//Warning: treeNode.leaves are sabotaged. treeNode.leaves are never used in doubleTrees but they are still used in forests.
		static public string readTreePreorder(TreeNode node, ref int counter, ref int leftcounter, ref int rightcounter, bool currentNodeIsTheLeftChild, ref MyList myList, int depth, ref string strdepth) {
			if (node == null) return ".";
			string str = "";
			str += visit(node, str, ref counter, ref leftcounter, ref rightcounter, node.currentNodeIsTheLeftChild, ref myList, depth, ref strdepth);
			str += readTreePreorder(node.left, ref counter, ref leftcounter, ref rightcounter, currentNodeIsTheLeftChild, ref myList,  depth+1, ref strdepth);
			str += readTreePreorder(node.right, ref  counter, ref leftcounter, ref rightcounter, currentNodeIsTheLeftChild, ref myList, depth+1, ref strdepth);
			return str + "COMPLETED(c,l,r)=(" + counter + "," + leftcounter + "," + rightcounter + ")" + Environment.NewLine;//Completion arrives when it reaches a leaf
		}

		static private string visit(TreeNode node, string str, ref int counter, ref int leftcounter, ref int rightcounter, bool currentNodeIsTheLeftChild,ref MyList myList, int depth,ref string strdepth) {
			if (node.left == null && node.right == null) {
				strdepth += depth+",";
				str = "";
				if (currentNodeIsTheLeftChild == true) {
					str = "=LeftLeaf "; ++leftcounter; str += leftcounter;
					str += "`" + (leftcounter + rightcounter) + "'";
				}
				else {
					str = "=RightLeaf ";
					++rightcounter;
					str += rightcounter;
					str += "`"+(leftcounter+rightcounter) + "'";
				}
				node.leaves = leftcounter + rightcounter;
				myList.AddLast(node);
				return "[Node" + (++counter) +  str + "]";
			}
			return "{Node" + (++counter) + "}";
		}

		//leaf 1 2 3 ... if myNode!=null
		/*static public int leafNumber(MyNode myNode) {
			MyNode cur = myNode;
			int count = 0;
			while (cur != null) {
				++count;
				cur = cur.Previous;
			}
			return count;
		}
		  */


		#region serialization

		// Serialize doubleTree to string
		static public string DoubleTreeToString(DoubleTree doubleTree) {
			string r = "", d = "";
			r = TreeToStringNew(doubleTree.range);

			TreeNode cur = doubleTree.range;
			TreeNode domainSigma = null;
			while (cur != null) {// First leaf in range maps to domainSigma 
				domainSigma = cur.sigma;
				cur = cur.left;
			}
			d = TreeToStringNewWithPointer(doubleTree.domain, domainSigma);
			//Console.WriteLine(r.TrimEnd('0') + "3" + d.TrimEnd('0'));
//			Console.WriteLine(r + "3" + d);
			//3 = |
			return base4toBase64(r.TrimEnd('0') + "3" + d.TrimEnd('0'));
			//return r + "|3|" + d;
		}

		static public string DoubleTreeToStringBin(DoubleTree doubleTree) {
			string r = "", d = "";
			r = TreeToStringNew(doubleTree.range);

			TreeNode cur = doubleTree.range;
			TreeNode domainSigma = null;
			while (cur != null) {// First leaf in range maps to domainSigma 
				domainSigma = cur.sigma;
				cur = cur.left;
			}

			string p="";
			while (domainSigma != null) {
				if (domainSigma.currentNodeIsTheLeftChild == true)
					p = "0" + p;
				else
					p = "1" + p;
				domainSigma = domainSigma.parent;
			}
			p = p.TrimEnd('0');
			if (p != "") p = "z" + base4toBase64(base2toBase4(p));

			d = TreeToStringNew(doubleTree.domain);
			//Console.WriteLine(r.TrimEnd('0') + "3" + d.TrimEnd('0'));
			//			Console.WriteLine(r + "3" + d);
			//3 = |
			return base4toBase64(base2toBase4(r.TrimEnd('0'))) + "z" + base4toBase64(base2toBase4(d.TrimEnd('0')))+p;
			//return r + "|3|" + d;
		}


		static public void DoubleTreeToDoubleStringBin(DoubleTree doubleTree, ref string s, ref string sInverse) {


			TreeNode cur = doubleTree.range;
			TreeNode domainSigma = null;
			while (cur != null) {// First leaf in range maps to domainSigma 
				domainSigma = cur.sigma;
				cur = cur.left;
			}

			string p = "";
			while (domainSigma.parent != null) {//we do not count the root.
				if (domainSigma.currentNodeIsTheLeftChild == true)
					p = "0" + p;
				else
					p = "1" + p;
				domainSigma = domainSigma.parent;
			}
			p = p.TrimEnd('0');
			if (p != "") p = "z" + base4toBase64(base2toBase4(p));//if p="" then the element is in F and z is not recorded


			TreeNode domainFirstLeaf = null;
			cur = doubleTree.domain;
			while (cur != null) {// First leaf in domain is found
				domainFirstLeaf = cur;
				cur = cur.left;
			}


			TreeNode rangeSigma = null;
			string r = TreeToStringNewWithInversePointer(doubleTree.range, domainFirstLeaf,ref rangeSigma); //domainSigma stores first leaf of domain


			string q = "";
			while (rangeSigma.parent != null) {//we do not count the root.
				if (rangeSigma.currentNodeIsTheLeftChild == true)
					q = "0" + q;
				else
					q = "1" + q;
				rangeSigma = rangeSigma.parent;
			}
			q = q.TrimEnd('0');
			if (q != "") q = "z" + base4toBase64(base2toBase4(q));


			string d = TreeToStringNew(doubleTree.domain);

			s = base4toBase64(base2toBase4(r.TrimEnd('0'))) + "z" + base4toBase64(base2toBase4(d.TrimEnd('0'))) + p;
			sInverse = base4toBase64(base2toBase4(d.TrimEnd('0'))) + "z" + base4toBase64(base2toBase4(r.TrimEnd('0'))) + q;//domain and range are swapped with range carrying the pointer

		}

		static public string DoubleTreeToDoubleStringBinOnF(DoubleTree doubleTree) {
			string r = TreeToStringNew(doubleTree.range);
			string d = TreeToStringNew(doubleTree.domain);
			return base4toBase64(base2toBase4(r.TrimEnd('0'))) + "z" + base4toBase64(base2toBase4(d.TrimEnd('0')));
			

		}

		static public string DoubleTreeToDoubleStringBinOnFfirstHalf(DoubleTree doubleTree) {
			string r = TreeToStringNew(doubleTree.range);
			string d = TreeToStringNew(doubleTree.domain);

			if (string.CompareOrdinal(r, d) > 0) {//r>d
				return "";// base4toBase64(base2toBase4(d.TrimEnd('0'))) + "z" + base4toBase64(base2toBase4(r.TrimEnd('0')));
			}
			//r<=d
			return base4toBase64(base2toBase4(r.TrimEnd('0'))) + "z" + base4toBase64(base2toBase4(d.TrimEnd('0')));
		}

		static public string DoubleTreeToDoubleStringBinOnFsecondHalf(DoubleTree doubleTree) {
			string r = TreeToStringNew(doubleTree.range);
			string d = TreeToStringNew(doubleTree.domain);

			if (string.CompareOrdinal(r, d) > 0) {//r>d
				return base4toBase64(base2toBase4(r.TrimEnd('0'))) + "z" + base4toBase64(base2toBase4(d.TrimEnd('0')));
			}
			return "";


		}

		static public string DoubleTreeToDoubleStringBinOnFPartition(DoubleTree doubleTree,ref int partition) {
			string r = TreeToStringNew(doubleTree.range);
			string d = TreeToStringNew(doubleTree.domain);

			partition=string.CompareOrdinal(r, d);
			return base4toBase64(base2toBase4(r.TrimEnd('0'))) + "z" + base4toBase64(base2toBase4(d.TrimEnd('0')));
		}


		static public string DoubleTreeToDoubleStringBinOnFPartitionInverse(DoubleTree doubleTree, ref int partition) {
			string r = TreeToStringNew(doubleTree.range);
			string d = TreeToStringNew(doubleTree.domain);

			partition = string.CompareOrdinal(r, d);
			return base4toBase64(base2toBase4(d.TrimEnd('0'))) + "z" + base4toBase64(base2toBase4(r.TrimEnd('0')));
		}


		static public string DoubleTreeToDoubleStringBinOnFsecondHalfInverse(DoubleTree doubleTree) {
			string r = TreeToStringNew(doubleTree.range);
			string d = TreeToStringNew(doubleTree.domain);

			if (string.CompareOrdinal(r, d) > 0) {//r>d
				return base4toBase64(base2toBase4(d.TrimEnd('0'))) + "z" + base4toBase64(base2toBase4(r.TrimEnd('0')));
			}
			return "";


		}




		// Serialize doubleTree to string
		static public string DoubleTreeToForestToString(DoubleTree doubleTree) {
			TreeNode leftmostLeaf = null;
			string r = TreeToForestToString(doubleTree.range, ref leftmostLeaf);
			string d = TreeToForestToStringWithPointer(doubleTree.domain, leftmostLeaf.sigma);
			//3 = |    
			//return base4toBase64(r+ "3" + d);//first 3 means pointer of range, second 3 means starting range, if third 3 is single (i.e. no 33) then it means the range.sigma is on the leftside of the domain of the pointer. 33 means right side of the domain of the pointer.
			return r + "|" + d;
		}

		static public string DoubleTreeToInverseToForestToString(DoubleTree doubleTree) {
			TreeNode leftmostLeaf = null;
			string d = TreeToForestToString(doubleTree.domain, ref leftmostLeaf);
			string r=TreeToForestToStringWithInversePointer(doubleTree.range,leftmostLeaf);
			//3 = |
			return base4toBase64(d + "3" + r);//domain and range are swapped with range carrying the pointer
			//return d + "|" + r;
		}


		static public string TreeToForestToString(TreeNode tree, ref TreeNode leftmostLeaf) {
			string r = "";			
			//a tree is seen as a sequence of subtrees along the main arc /\ of the tree. A trivial subtree is a leaf.
			//reading left side of the range.
			//READING THE LEFT SIDE OF THE TREE
			TreeNode cur = tree.left;//tree cannot be null. it has to be at least the idenity.
			leftmostLeaf = tree;
			while (cur != null) {//if range is a leaf the loop does nothing. if range is a caret the loop gives nothing. first leaf is not recorded
				if (cur.left != null) { //cur is not the leftmost leaf.
					if (cur.right.left == null) //tree is of depth 0  so a 2=, is written
						r = "2" + r;
					else
						r = TreeToStringNew(cur.right).TrimEnd('0') + "2" + r;
				}
				leftmostLeaf = leftmostLeaf.left;
				cur = cur.left;
			}
			//READING THE RIGHT SIDE OF THE TREE
			cur = tree.right;
			r = r + "3";//adding the pointer at 1/2
			while (cur != null) {//if range is a leaf the loop does nothing. if range is a caret the loop gives nothing. last leaf is not recorded
				if (cur.right != null) {// cur is not the rightmost leaf
					if (cur.left.left == null) //tree is of depth 0  so a 2=, is written
						r = r + "2";
					else
						r = r + TreeToStringNew(cur.left).TrimEnd('0') + "2";
				}
				cur = cur.right;
			}//format for range: tree1, tree2, tree3, | tree 4, tree5  where ,=2 and |=3.
			//r = r.Trim('2');// remove trivial trees from left and right. CANNOT DO THIS! Then A2 and A4 have same forest representation!
			//if range=id: r=|
			//if range=caret: r=|
			//if range=vine: r=|
			return r;
		}

		static public string TreeToForestToStringWithPointer(TreeNode tree, TreeNode sigma) {
			string d = "";
			//a tree is seen as a sequence of subtrees along the main arc /\ of the tree. A trivial subtree is a leaf.
			//reading left side of the range.
			//READING THE LEFT SIDE OF THE TREE
			TreeNode cur = tree.left;//tree cannot be null. it has to be at least the idenity.
			while (cur != null) {//if range is a leaf the loop does nothing. if range is a caret the loop gives nothing. first leaf is not recorded
				if (cur.left != null) { //cur is not the leftmost leaf.
					if (cur.right.left == null) { //tree is of depth 0  so a 2=, is written
						if (cur.right == sigma) d = "32" + d;
						else
							d = "2" + d;
					}
					else
						d = TreeToStringNewWithPointer(cur.right, sigma).TrimEnd('0') + "2" + d;
				}
				else {
					if (cur == sigma) d = "32" + d;
				}
				cur = cur.left;
			}//if the pointer was found, then it will be before a 2. 
			//READING THE RIGHT SIDE OF THE RANGE
			cur = tree.right;
			d = d + "33";//adding the pointer at 1/2 
			while (cur != null) {//if range is a leaf the loop does nothing. if range is a caret the loop gives nothing. last leaf is not recorded
				if (cur.right != null) {// cur is not the rightmost leaf
					if (cur.left.left == null) { //tree is of depth 0  so a 2=, is written
						if (cur.left == sigma) d = d + "32";
						else
							d = d + "2";
					}
					else
						d = d + TreeToStringNewWithPointer(cur.left, sigma).TrimEnd('0') + "2";
				}
				else {
					if (cur == sigma) d = d + "32";
				}
				cur = cur.right;
			}//if a pointer is found on the righthandside then if a 333 occur then the first 33 mean the pointer at 1/2 and the other tree means sigma
			//format for range: tree1, tree2, tree3, | tree 4, tree6  where ,=2 and |=3.
			
			//if range=id: r=|
			//if range=caret: r=|
			//if range=vine: r=|
			return d;
		}

		static public string TreeToForestToStringWithInversePointer(TreeNode tree, TreeNode domainSigma) {
			string r = "";
			//a tree is seen as a sequence of subtrees along the main arc /\ of the tree. A trivial subtree is a leaf.
			//READING THE LEFT SIDE OF THE TREE
			
			TreeNode cur = tree.left;//tree cannot be null. it has to be at least the idenity.
			while (cur != null) {//if range is a leaf the loop does nothing. if range is a caret the loop gives nothing. first leaf is not recorded
				if (cur.left != null) { //cur is not the leftmost leaf.
					if (cur.right.left == null) {//tree is of depth 0  so a 2=, is written
						if (cur.right.sigma == domainSigma) r = "32" + r;
						else
							r = "2" + r;
					}
					else
						r = TreeToStringNewWithInversePointer(cur.right, domainSigma).TrimEnd('0') + "2" + r;
				}
				else {//cur is the leftmost leaf;
					if (cur.sigma == domainSigma) r = "32" + r;
				}
				cur = cur.left;
			}//if the pointer was found, then it will be before a 2. 
			//READING THE RIGHT SIDE OF THE RANGE
			cur = tree.right;
			r = r + "33";//adding the pointer at 1/2 
			while (cur != null) {//if range is a leaf the loop does nothing. if range is a caret the loop gives nothing. last leaf is not recorded
				if (cur.right != null) {// cur is not the rightmost leaf
					if (cur.left.left == null) { //tree is of depth 0  so a 2=, is written
						if (cur.left.sigma == domainSigma) r = r + "32";
						else
							r = r + "2";
					}
					else
						r = r + TreeToStringNewWithInversePointer(cur.left, domainSigma).TrimEnd('0') + "2";
				}
				else {
					if (cur.sigma == domainSigma) r = r + "32";
				}
				cur = cur.right;
			}//if a pointer is found on the righthandside then if a 333 occur then the first 33 mean the pointer at 1/2 and the other tree means sigma
			//format for range: tree1, tree2, tree3, || tree 4, tree5  where ,=2 and |=3.

			//if range=id: r=|
			//if range=caret: r=|
			//if range=vine: r=|
			return r;
		}
		 

		// Serialize the inverse of the given doubleTree to string
		static public string DoubleTreeToInverseToString(DoubleTree doubleTree) {

			TreeNode domainFirstLeaf = null;
			TreeNode cur = doubleTree.domain;
			while (cur != null) {// First leaf in domain is domainSigma
				domainFirstLeaf = cur;
				cur = cur.left;
			}

			string r = "", d = "";
			r = TreeToStringNewWithInversePointer(doubleTree.range, domainFirstLeaf); //domainSigma stores first leaf of domain
		
			d = TreeToStringNew(doubleTree.domain);
			//Console.WriteLine(d.TrimEnd('0') + "3" + r.TrimEnd('0'));
			//Console.WriteLine(d + "3" + r);
			//3 = |
			return base4toBase64(d.TrimEnd('0') + "3" + r.TrimEnd('0'));//domain and range are swapped with range carrying the pointer
			//return d + "|3|" + r;
		}


		static public string TreeToStringNew(TreeNode node) {
			if (node.left == null) return "0";//We have reached a leaf.
			return "1" + TreeToStringNew(node.left) + TreeToStringNew(node.right);
		}


		static public DoubleTree StringToDoubleTree(string str) {
			string[] words=str.Split('z');
			string r = base4toBase2(base64toBase4(words[0]));//need to convert to base 4 and then to base 2;
			string d = base4toBase2(base64toBase4(words[1]));
			string p = "";
			
			if (words.Length == 3) { p = base4toBase2(base64toBase4(words[2])); }
			//Console.WriteLine("r=" + words[0] + " d=" + words[1] + " p=" + p);
			//Console.WriteLine("r=" + r + " d=" + d + " p=" + p);
			MyList leaves=new MyList();		
			TreeNode domain = null;
			StringToTreeReadLeaf(node: ref domain, str: ref d, myList: ref leaves);
			TreeNode pointer = null, cur = domain; 
			int pos = 0;
			while (cur != null) {
				pointer = cur;
				char ch = '0';
				if (pos < p.Length) ch = p[pos];
				pos++;
				if (ch == '0') cur = cur.left;
				else cur = cur.right;
				
			}
			//Console.WriteLine("nUMBER OF LEAVES " + leaves.Length);
			//MyNode myNode = leaves.First;
			//for (int i = 0; i < leaves.Length;i++ ) {
			//	Console.WriteLine(""+i+" "+myNode.Value.currentNodeIsTheLeftChild);
			//	myNode=myNode.Next;
			//}
			
			leaves.NewBeginning(pointer);
			TreeNode range = null;
			StringToTreeWriteLeaf(node: ref range, str: ref r, myList:ref leaves);

			//Console.ReadKey();

			return new DoubleTree(rangeTree: range, domainTree: domain);

		}


		// Deserialize tree from string
		static public int StringToTreeReadLeaf(ref TreeNode node, ref string str, ref MyList myList, int pos = 0, TreeNode parent = null, bool currentNodeIsTheLeftChild = true) {
			char ch = '0'; // since zeros may have been trimmed from the end, we assume zero, if we reached the end of string
			if (pos < str.Length) ch = str[pos];
			pos++;
			node = new TreeNode(parentNode: parent, currentNodeIsTheLeftChild: currentNodeIsTheLeftChild);
			if (ch == '1') {
				pos = StringToTreeReadLeaf(node: ref node.left, str: ref str, myList: ref myList, pos: pos, parent: node, currentNodeIsTheLeftChild: true);
				pos = StringToTreeReadLeaf(node: ref node.right, str: ref str, myList: ref myList, pos: pos, parent: node, currentNodeIsTheLeftChild: false);
			}
			else {
				myList.AddLast(node);
			}
			return pos;
		}

		static public int StringToTreeWriteLeaf(ref TreeNode node, ref string str,ref MyList myList, int pos=0, TreeNode parent=null, bool currentNodeIsTheLeftChild=true ) {
			char ch = '0'; // since zeros may have been trimmed from the end, we assume zero, if we reached the end of string
			if (pos < str.Length) ch = str[pos];
			pos++;
			if (ch == '0') {
				TreeNode sigmaNode = myList.ReadNode();
				node = new TreeNode(parentNode: parent, currentNodeIsTheLeftChild: currentNodeIsTheLeftChild, sigmaNode: sigmaNode);
				//if (sigmaNode == null) Console.WriteLine("FOUND NULL AT STRINGTOTREEwriteLEAF"); else Console.WriteLine("SO FAR SO GOOD" + currentNodeIsTheLeftChild);

			}
			else
				node = new TreeNode(parentNode: parent, currentNodeIsTheLeftChild: currentNodeIsTheLeftChild);
			if (ch == '1') {
				pos = StringToTreeWriteLeaf(node: ref node.left, str: ref str, myList: ref myList, pos: pos, parent: node, currentNodeIsTheLeftChild: true);
				pos = StringToTreeWriteLeaf(node: ref node.right, str: ref str,myList: ref myList, pos: pos, parent: node, currentNodeIsTheLeftChild: false);
			}
			return pos;
		}

	

		static public string TreeToStringNewWithPointer(TreeNode node, TreeNode Pointer) {
			if (node.left == null) {//We have reached a leaf.
				if (node == Pointer) return "3";//The first leaf in range maps to Pointer in domain where a 3 is written instead of a 0.
				return "0";
			}

			return "1" + TreeToStringNewWithPointer(node.left, Pointer) + TreeToStringNewWithPointer(node.right, Pointer);
		}

		//overload:it is called with the range tree, firstleaf of domain and returns sigma.domain
		static public string TreeToStringNewWithInversePointer(TreeNode node, TreeNode firstLeaf, ref TreeNode sigmaOfFirstLeaf) {
			if (node.left == null) {//We have reached a leaf.
				if (node.sigma == firstLeaf) sigmaOfFirstLeaf=node;//The first leaf in domain maps to current node in range where a 3 is written instead of a 0.
				return "0";
			}

			return "1" + TreeToStringNewWithInversePointer(node.left, firstLeaf,ref sigmaOfFirstLeaf) + TreeToStringNewWithInversePointer(node.right, firstLeaf,ref sigmaOfFirstLeaf);
		}


		static public string TreeToStringNewWithInversePointer(TreeNode node,  TreeNode domainSigma) {
			if (node.left == null) {//We have reached a leaf.
				if (node.sigma == domainSigma) return "3";//The first leaf in domain maps to current node in range where a 3 is written instead of a 0.
				return "0";
			}

			return "1" + TreeToStringNewWithInversePointer(node.left, domainSigma) + TreeToStringNewWithInversePointer(node.right, domainSigma);
		}

		#endregion

		#region Leaf Labels


		

		// Leaf labels of a DoubleTree
		static public int DoubleTreeLength(DoubleTree doubleTree) {
			string r = TreeLeafLabelsAsForest(doubleTree.range);
			string d = TreeLeafLabelsAsForest(doubleTree.domain);
			if (r == "") return 0;// doubleTree is the identity
			//The second leftmost leaf is not used in the labelling because that corresponds to a gap outside the support of the forest.
			r = r.Substring(1);
			d = d.Substring(1);

			//endtrimming the common 'R's of r and d. 
			int minLength = r.Length < d.Length ? r.Length : d.Length;
			int counter = 0;			
			for (int i = 0; i < minLength; i++) {
				//Console.WriteLine("r=" + r + " d=" + minLength+"counter=" +counter);
				if (r[r.Length-1 - counter] != 'R' || d[r.Length-1 - counter] != 'R') break;
				++counter;
			}
			if (counter > 0 && counter < minLength) { r = r.Substring(0, r.Length - counter); d = d.Substring(0, d.Length - counter); }

			return WordLength(r, d);


		}

		static public string DoubleTreeLeafLabels(DoubleTree doubleTree) {
			string r = TreeLeafLabelsAsForest(doubleTree.range);
			string d=TreeLeafLabelsAsForest(doubleTree.domain);
			if (r.Length > 0 && (r[r.Length - 1] != 'R' || d[d.Length-1]!='R')) {//we assume r and d are of same length
				r += 'R';
				d += 'R';
			}
			return r + " " + d;
		}



		static private int WordLength(string r, string d) {
			int[,] mtx=new int[,] {{2,4,2,1,3},{4,4,2,3,3},{2,2,2,1,1},{1,3,1,2,2},{3,3,1,2,2}};
			int length = 0;

			for (int i = 0; i < r.Length; i++) {
				length += mtx[GapLabels(r[i]), GapLabels(d[i])];
			}

				return length;
		}
		static private int GapLabels(char ch) {
			switch (ch) {
				case 'I': return 0;
				case 'N': return 1;
				case 'L': return 2;
				case 'R': return 3;
				case 'X': return 4;					
			}
			return -1;
		}


		static public void TreeLeafLabels(TreeNode node, ref string treeLeafLabels) {
			if (node.left == null) {//We have reached a leaf.
				if (node.currentNodeIsTheLeftChild == true) treeLeafLabels += "N";
				else treeLeafLabels += "I";
				return;
			}
			TreeLeafLabels(node.left, ref treeLeafLabels);
			TreeLeafLabels(node.right, ref treeLeafLabels);
		}


		static public string TreeLeafLabelsAsForest(TreeNode tree) {
			//We think of the tree as the range of a doubleTree.
			string r = "";
			//a tree is seen as a sequence of subtrees along the main arc /\ of the tree. A trivial subtree is a leaf.
			//reading left side of the range.
			//READING THE LEFT SIDE OF THE TREE
			TreeNode cur = tree.left;//tree cannot be null. it has to be at least the identity.
			
			while (cur != null) {//if range is a leaf the loop does nothing. if range is a caret the loop gives nothing. first leaf is not recorded
				if (cur.left != null) {//cur is not the leftmost leaf.
				 
					if (cur.right.left == null) //tree is of depth 0  so an 'L' is written
						r = "L" + r;
					else {
						string treeLeafLabels = "";
						TreeLeafLabels(cur.right, ref treeLeafLabels);
						r = "L" + treeLeafLabels.Substring(1) + r;//the first letter of the subtree is changed from N to L.
					}
				}
				
				cur = cur.left;				
			}
			//READING THE RIGHT SIDE OF THE TREE
			cur = tree.right;
			//r = r + "|";//adding the pointer at 1/2
			int treecounter = 0;
			
			while (cur != null) {//if range is a leaf the loop does nothing. if range is a caret the loop gives nothing. last leaf is not recorded
				if (cur.right != null) {// cur is not the rightmost leaf
					if (cur.left.left == null) //tree is of depth 0  so a 'R' is written except if it is the 1/2 leaf
						r = r + (treecounter==0?'L':'R');
					else {
						string treeLeafLabels = "";
						 TreeLeafLabels(cur.left,ref treeLeafLabels);
						r = r + (treecounter==0?'L':'X') + treeLeafLabels.Substring(1); //the first letter of the subtree is changed from N to X.
					}
				}
				cur = cur.right;
				++treecounter;
			}//format for range: tree1, tree2, tree3, | tree 4, tree5  

			if (tree.right != null) {//adding the label of the rightmostleaf
				if (tree.right.left == null)
					r += "L";//pointer is on the rightmost leaf
				else r += 'R';//pointer is not on the rightmost leaf
			}
			return r;
		}



		#endregion


		static public int[] DoubleTreeNormalFormExp(DoubleTree doubleTree){
			MyList leaves = new MyList();
			TreeLeaves(doubleTree.range, ref leaves);
			MyNode cur = leaves.First;
			int nrange = 0;			
			int ndomain =0;
			int[] exponents = new int[2*leaves.Length];
			while (cur != null) {
				exponents[nrange] = LeafLeftDepth(cur.Value);
				exponents[2*leaves.Length-ndomain-1] = -LeafLeftDepth(cur.Value.sigma);
				++nrange;
				++ndomain;
				cur = cur.Next;
			}
			--exponents[0];//The parentNode is a special case for range Tree.
			++exponents[2*leaves.Length-1];//The parentNode is a special case for domain Tree.
			return exponents;

		}

		static public int LeafLeftDepth(TreeNode node) {
			int depth = 0;
			TreeNode cur = node;
			while (cur != null) {
				if (cur.currentNodeIsTheLeftChild == false) break;
				++depth;
				cur = cur.parent;
			}
			return depth;	

		}


		static public void TreeLeaves(TreeNode node,ref MyList mylist) {
			if (node.left == null) { mylist.AddLast(node); return ; }//We have reached a leaf.
			TreeLeaves(node.left,ref mylist);
			TreeLeaves(node.right,ref mylist);
		}

		//TreeLeavesOfRange makes the leaves of the domain point to the range. It is used in DoubleTreeInverse
		static private void TreeLeavesOfRange(TreeNode node) {
			if (node.left == null) { node.sigma.sigma = node; return; }//We have reached a leaf.
			TreeLeavesOfRange(node.left);
			TreeLeavesOfRange(node.right);
		}

		//inverse of doubleTrees
		static public void DoubleTreeInverse(ref DoubleTree doubleTree) {
			TreeLeavesOfRange(doubleTree.range);//adds links to the domains of the leaves
			doubleTree = new DoubleTree(rangeTree: doubleTree.domain,domainTree: doubleTree.range);
		}


		

		#region Change of base 2,4,64


		static public string base2toBase4(string input) {
			string output = "";
			input = input + "00";
			string segment;
			for (int i = 0; i < input.Length - 2; i += 2) {
				segment = input.Substring(i, 2);
				output += TupleBase2To4(ref segment);
			}
			return output;
		}


		static private char TupleBase2To4(ref string tupleBase4) {
			char digit4='0';
			switch (tupleBase4) {
				case "00": digit4 = '0'; break;
				case "01": digit4 = '1'; break;
				case "10": digit4 = '2'; break;
				case "11": digit4 = '3'; break;
				default: Console.WriteLine("Error:Base2to4 not such 2 digits in base 4"); break;
			}
			return digit4;
		}

		static public string base4toBase2(string input) {
			string output = "";

			for (int i = 0; i < input.Length; i++) {
				output += Base4ToTupleBase2(input[i]);
			}
			return output;
		}


		static private string Base4ToTupleBase2(char digitBase4) {
			string  tupleBase2 = "";
			switch (digitBase4) {
				case '0': tupleBase2 = "00"; break;
				case '1': tupleBase2 = "01"; break;
				case '2': tupleBase2 = "10"; break;
				case '3': tupleBase2 = "11"; break;
				default: Console.WriteLine("Error:Base2to4 not such 2 digits in base 4"); break;
			}
			return tupleBase2;
		}

		static private string base4toBase64(string input) {
			string output = "";
			input = input + "000";
			string segment;
			for (int i = 0; i < input.Length - 3; i += 3) {
				segment = input.Substring(i, 3);
				output += TripleBase4to64(ref segment);
			}
			return output;
		}



		static private char TripleBase4to64(ref string tripleBase4)//base4=d1d2d3 where the digits are between 0 and 3. 
	{
			char digit64 = '0';
			switch (tripleBase4) {
				case "000": digit64 = '0'; break;
				case "001": digit64 = '1'; break;
				case "002": digit64 = '2'; break;
				case "003": digit64 = '3'; break;
				case "010": digit64 = '4'; break;
				case "011": digit64 = '5'; break;
				case "012": digit64 = '6'; break;
				case "013": digit64 = '7'; break;
				case "020": digit64 = '8'; break;
				case "021": digit64 = '9'; break;
				case "022": digit64 = ':'; break;
				case "023": digit64 = ';'; break;
				case "030": digit64 = '<'; break;
				case "031": digit64 = '='; break;
				case "032": digit64 = '>'; break;
				case "033": digit64 = '?'; break;
				case "100": digit64 = '@'; break;
				case "101": digit64 = 'A'; break;
				case "102": digit64 = 'B'; break;
				case "103": digit64 = 'C'; break;
				case "110": digit64 = 'D'; break;
				case "111": digit64 = 'E'; break;
				case "112": digit64 = 'F'; break;
				case "113": digit64 = 'G'; break;
				case "120": digit64 = 'H'; break;
				case "121": digit64 = 'I'; break;
				case "122": digit64 = 'J'; break;
				case "123": digit64 = 'K'; break;
				case "130": digit64 = 'L'; break;
				case "131": digit64 = 'M'; break;
				case "132": digit64 = 'N'; break;
				case "133": digit64 = 'O'; break;
				case "200": digit64 = 'P'; break;
				case "201": digit64 = 'Q'; break;
				case "202": digit64 = 'R'; break;
				case "203": digit64 = 'S'; break;
				case "210": digit64 = 'T'; break;
				case "211": digit64 = 'U'; break;
				case "212": digit64 = 'V'; break;
				case "213": digit64 = 'W'; break;
				case "220": digit64 = 'X'; break;
				case "221": digit64 = 'Y'; break;
				case "222": digit64 = 'Z'; break;
				case "223": digit64 = '['; break;
				case "230": digit64 = '\\'; break;
				case "231": digit64 = ']'; break;
				case "232": digit64 = '^'; break;
				case "233": digit64 = '_'; break;
				case "300": digit64 = '`'; break;
				case "301": digit64 = 'a'; break;
				case "302": digit64 = 'b'; break;
				case "303": digit64 = 'c'; break;
				case "310": digit64 = 'd'; break;
				case "311": digit64 = 'e'; break;
				case "312": digit64 = 'f'; break;
				case "313": digit64 = 'g'; break;
				case "320": digit64 = 'h'; break;
				case "321": digit64 = 'i'; break;
				case "322": digit64 = 'j'; break;
				case "323": digit64 = 'k'; break;
				case "330": digit64 = 'l'; break;
				case "331": digit64 = 'm'; break;
				case "332": digit64 = 'n'; break;
				case "333": digit64 = 'o'; break;
				default: Console.WriteLine("Error:Base4to64 not such 3 digits in base 4"); break;
			}

			return digit64;
		}




		static private string base64toBase4(string input) {
			string output = "";
			for (int i = 0; i < input.Length; ++i) {
				output += Base64ToTripleBase4(input[i]);
			}
			return output;
		}

		static private string Base64ToTripleBase4(char digitBase64)//base4=d1d2d3 where the digits are between 0 and 3. 
	{
			string tripleBase4 = "";
			switch (digitBase64) {
				case '0': tripleBase4 = "000"; break;
				case '1': tripleBase4 = "001"; break;
				case '2': tripleBase4 = "002"; break;
				case '3': tripleBase4 = "003"; break;
				case '4': tripleBase4 = "010"; break;
				case '5': tripleBase4 = "011"; break;
				case '6': tripleBase4 = "012"; break;
				case '7': tripleBase4 = "013"; break;
				case '8': tripleBase4 = "020"; break;
				case '9': tripleBase4 = "021"; break;
				case ':': tripleBase4 = "022"; break;
				case ';': tripleBase4 = "023"; break;
				case '<': tripleBase4 = "030"; break;
				case '=': tripleBase4 = "031"; break;
				case '>': tripleBase4 = "032"; break;
				case '?': tripleBase4 = "033"; break;
				case '@': tripleBase4 = "100"; break;
				case 'A': tripleBase4 = "101"; break;
				case 'B': tripleBase4 = "102"; break;
				case 'C': tripleBase4 = "103"; break;
				case 'D': tripleBase4 = "110"; break;
				case 'E': tripleBase4 = "111"; break;
				case 'F': tripleBase4 = "112"; break;
				case 'G': tripleBase4 = "113"; break;
				case 'H': tripleBase4 = "120"; break;
				case 'I': tripleBase4 = "121"; break;
				case 'J': tripleBase4 = "122"; break;
				case 'K': tripleBase4 = "123"; break;
				case 'L': tripleBase4 = "130"; break;
				case 'M': tripleBase4 = "131"; break;
				case 'N': tripleBase4 = "132"; break;
				case 'O': tripleBase4 = "133"; break;
				case 'P': tripleBase4 = "200"; break;
				case 'Q': tripleBase4 = "201"; break;
				case 'R': tripleBase4 = "202"; break;
				case 'S': tripleBase4 = "203"; break;
				case 'T': tripleBase4 = "210"; break;
				case 'U': tripleBase4 = "211"; break;
				case 'V': tripleBase4 = "212"; break;
				case 'W': tripleBase4 = "213"; break;
				case 'X': tripleBase4 = "220"; break;
				case 'Y': tripleBase4 = "221"; break;
				case 'Z': tripleBase4 = "222"; break;
				case '[': tripleBase4 = "223"; break;
				case '\\': tripleBase4 = "230"; break;
				case ']': tripleBase4 = "231"; break;
				case '^': tripleBase4 = "232"; break;
				case '_': tripleBase4 = "233"; break;
				case '`': tripleBase4 = "300"; break;
				case 'a': tripleBase4 = "301"; break;
				case 'b': tripleBase4 = "302"; break;
				case 'c': tripleBase4 = "303"; break;
				case 'd': tripleBase4 = "310"; break;
				case 'e': tripleBase4 = "311"; break;
				case 'f': tripleBase4 = "312"; break;
				case 'g': tripleBase4 = "313"; break;
				case 'h': tripleBase4 = "320"; break;
				case 'i': tripleBase4 = "321"; break;
				case 'j': tripleBase4 = "322"; break;
				case 'k': tripleBase4 = "323"; break;
				case 'l': tripleBase4 = "330"; break;
				case 'm': tripleBase4 = "331"; break;
				case 'n': tripleBase4 = "332"; break;
				case 'o': tripleBase4 = "333"; break;

				default: Console.WriteLine("Error:Base4to64 not such 3 digits in base 4"); break;
			}

			return tripleBase4;
		}




        #endregion


	}

}
