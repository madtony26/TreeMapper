using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace TreeMapper
{
	public class TreeMapperArgs : EventArgs
	{
		public string Message{ get; set; }
	}

	public class TreeMapper : ThreadingExtensionBase
	{
		public double Randomness = 0;
		public double XScale = 1;
		public double YScale = 1;
		public double XShift = 0;
		public double YShift = 0;
		public int Density = 1;
//		public TreeInfo SelectedTree = GameObject.FindObjectOfType<TreeCollection>().m_prefabs[0];

		public event EventHandler<TreeMapperArgs> TreeMapperEvent;

		public TreeMapper()
		{
			this.random = new System.Random ();
		}

		public void ClearTrees()
		{
			DeleteAllTrees ();
		}

		public void ImportTrees(BoundingBox BoundingBox)
		{
			//RaiseTreeMapperEvent("Starting Import Trees..");
			Bitmap bitmap = null;
			
			bitmap = TreeCoverClient.LoadTreeCover (BoundingBox);
			
			int trees_found = 0;

			if(bitmap == null)
			{
				return;
			}

			for (int x = 0; x < 1081; x += Density)
			{
				for(int y = 0; y < 1081; y += Density)
				{
					System.Drawing.Color pixel_color;
					
					pixel_color = bitmap.GetPixel(x,y);
					
					double intensity = ((int)pixel_color.G - (int)pixel_color.R) / 179.0;
					if(intensity > 0.0)
					{
						trees_found ++;
						//RaiseTreeMapperEvent(string.Format("Adding tree {0} {1}", x, y));
						TreeCollection treeCollection = GameObject.FindObjectOfType<TreeCollection>();
						IList<TreeInfo> treeInfos = treeCollection.m_prefabs;
						
						//			treeInfo = (from TreeInfo info in treeInfos
						//			            where info.name.Equals (name)
						//			            select info).FirstOrDefault();
						TreeInfo treeInfo = treeInfos [4];
						
						AddTreeFromPixelPosition(x, y, treeInfo);
					}
				}
				
				Thread.Sleep(20);
			}
		}

		private System.Random random;

		private void AddTreeFromPixelPosition(int x, int y, TreeInfo TreeInfo)
		{
			double shiftedX = (x - 540) + XShift;
			double shiftedY = (540 - y) + YShift;
			double scaledX = shiftedX * XScale;
			double scaledY = shiftedY * YScale;
			double randomizedX = scaledX + (0.5 - random.NextDouble()) * Randomness;
			double randomizedY = scaledY + (0.5 - random.NextDouble()) * Randomness;

			SimulationManager.instance.AddAction (AddTree (randomizedX, randomizedY, SimulationManager.instance.m_randomizer, TreeInfo));
		}
				
		private IEnumerator AddTree(double x, double  y, ColossalFramework.Math.Randomizer rr, TreeInfo tree) 
		{
			//RaiseTreeMapperEvent (string.Format ("Added Tree {0} {1}", x, y));
			 
			uint treeNum;
			TreeManager tree_manager = TreeManager.instance;
			try
			{
				tree_manager.CreateTree (out treeNum, ref rr, tree, new Vector3 ((float)x, 0, (float)y), false);
			}
			catch(Exception ex)
			{
				//try-catch just to prevent crashing by ignoring invalid trees and letting valid trees get created
				//RaiseTreeMapperEvent (ex.Message);
			}
			yield return null;
		}

		private void DeleteAllTrees() {
			int r = TreeManager.TREEGRID_RESOLUTION; // 540
			TreeManager tm = TreeManager.instance;

			if ( tm.m_treeCount == 0 )
				return;

			uint tot = 0;
			for (int i=0; i<r*r; i++) {
				uint id = tm.m_treeGrid[i];
				if (id != 0) {
					while (id != 0 && tot++ < TreeManager.MAX_MAP_TREES) {
						uint next = tm.m_trees.m_buffer[id].m_nextGridTree;
						SimulationManager.instance.AddAction( DelTree(id) );
						id = next;
					};
				}
			}
		}

		private IEnumerator DelTree(uint id) {
			TreeManager.instance.ReleaseTree(id);
			yield return null;
		}			

		private void RaiseTreeMapperEvent(string Message)
		{
			if (TreeMapperEvent != null) 
			{
				TreeMapperArgs args = new TreeMapperArgs();
				args.Message = Message;
				TreeMapperEvent(this, args);
			}
		}

	}
}

