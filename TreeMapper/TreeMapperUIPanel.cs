using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.UI;
using System.Reflection;
using System.Timers;
using UnityEngine;
using System.IO;
using System.Net;
using System.Collections;
using System.Threading;

namespace TreeMapper
{
	public class TreeMapperUIPanel: UIPanel
	{
		UILabel title;
		
		UITextField boundingBoxTextBox;
		UILabel boundingBoxLabel;
		
		UILabel informationLabel;

		UITextField xScaleTextBox;
		UILabel xScaleLabel;

		UITextField yScaleTextBox;
		UILabel yScaleLabel;

		UITextField xOffsetTextBox;
		UILabel xOffsetLabel;

		UITextField yOffsetTextBox;
		UILabel yOffsetLabel;
		
		UITextField randomnessTextBox;
		UILabel randomnessLabel;

		UITextField densityTextBox;
		UILabel densityLabel;

//		UIDropDown treeDropdown;
//		UILabel treeLabel;

		UILabel errorLabel;

		UIButton importButton;

		public override void Awake()
		{
			this.isInteractive = true;
			this.enabled = true;
			
			width = 500;
			
			title = AddUIComponent<UILabel>();
			
			boundingBoxTextBox = AddUIComponent<UITextField>();
			boundingBoxLabel = AddUIComponent<UILabel>();		
			
			informationLabel = AddUIComponent<UILabel>();
			
			randomnessTextBox = AddUIComponent<UITextField>();
			randomnessLabel = AddUIComponent<UILabel>();

			xScaleTextBox = AddUIComponent<UITextField> ();
			xScaleLabel = AddUIComponent<UILabel> ();

			yScaleTextBox = AddUIComponent<UITextField> ();
			yScaleLabel = AddUIComponent<UILabel> ();

			xOffsetTextBox = AddUIComponent<UITextField> ();
			xOffsetLabel = AddUIComponent<UILabel> ();

			yOffsetTextBox = AddUIComponent<UITextField> ();
			yOffsetLabel = AddUIComponent<UILabel> ();

			densityLabel = AddUIComponent<UILabel>();
			densityTextBox = AddUIComponent<UITextField>();

//			treeLabel = AddUIComponent<UILabel> ();
//			treeDropdown = AddUIComponent<UIDropDown> ();
						
			errorLabel = AddUIComponent<UILabel>();
			
			importButton = AddUIComponent<UIButton>();
			base.Awake();
		}

		public override void Start()
		{
			base.Start();
			
			relativePosition = new Vector3(396, 58);
			backgroundSprite = "MenuPanel2";
			isInteractive = true;
			SetupControls();
		}
		
		public void SetupControls()
		{
			title.text = "Tree Mapper";
			title.relativePosition = new Vector3(15, 15);
			title.textScale = 0.9f;
			title.size = new Vector2(200, 30);
			var vertPadding = 30;
			var x = 15;
			var y = 50;
			
			x = 15;
			y += vertPadding;
			
			SetLabel(randomnessLabel, "Randomness Factor", x, y);
			SetTextBox(randomnessTextBox, "40", x + 120, y);
			y += vertPadding;

			SetLabel(xScaleLabel, "X Scale", x, y);
			SetTextBox(xScaleTextBox, "16", x + 120, y);
			y += vertPadding;

			SetLabel(yScaleLabel, "Y Scale", x, y);
			SetTextBox(yScaleTextBox, "20", x + 120, y);
			y += vertPadding;

			SetLabel(xOffsetLabel, "X Offset", x, y);
			SetTextBox(xOffsetTextBox, "0", x + 120, y);
			y += vertPadding;
			
			SetLabel(yOffsetLabel, "Y Offset", x, y);
			SetTextBox(yOffsetTextBox, "0", x + 120, y);
			y += vertPadding;

			SetLabel(densityLabel, "Density", x, y);
			SetTextBox(densityTextBox, "3", x + 120, y);
			y += vertPadding;

			SetLabel(boundingBoxLabel, "Bounding Box", x, y);
			//Default is Frankfort, KY, which includes some interesting terrain and trees
			SetTextBox(boundingBoxTextBox, "-84.774950,38.264822,-84.980892,38.103126", x + 120, y);
			y += vertPadding - 5;

//			SetLabel (treeLabel, "Select a Tree:", x, y);
//			InitializeTreeDropwdown (x, y);
			
			SetButton(importButton, "Import Trees using Parameters", y);
			importButton.eventClick += importButton_eventClick;
			height = y + vertPadding + 6;
		}

		private void importButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
		{
			try
			{
				int density = int.Parse(densityTextBox.text);

				BoundingBox boundingBox = GetBoundingBox();

				TreeMapper treeMapper = new TreeMapper();

				treeMapper.XScale = double.Parse (xScaleTextBox.text);
				treeMapper.YScale = double.Parse(yScaleTextBox.text);

				treeMapper.XShift = double.Parse(xOffsetTextBox.text);
				treeMapper.YShift = double.Parse(yOffsetTextBox.text);

				treeMapper.Density = int.Parse(densityTextBox.text);
				treeMapper.Randomness = double.Parse(randomnessTextBox.text);

//				treeMapper.SelectedTree = GetTreeByName(treeDropdown.selectedValue);

				treeMapper.ClearTrees();
				treeMapper.TreeMapperEvent += TreeMapperEvent;
				Thread thread = new Thread(()=>	treeMapper.ImportTrees(boundingBox));
				thread.Start();
			}
			catch (Exception ex)
			{
				errorLabel.text = ex.ToString();
			}
		}

		private void TreeMapperEvent(object sender, TreeMapperArgs args)
		{
			errorLabel.text = args.Message;
		}

		private TreeInfo GetTreeByName(string name)
		{
			TreeInfo treeInfo;

			TreeCollection treeCollection= GameObject.FindObjectOfType<TreeCollection>();
			IList<TreeInfo> treeInfos = treeCollection.m_prefabs;

//			treeInfo = (from TreeInfo info in treeInfos
//			            where info.name.Equals (name)
//			            select info).FirstOrDefault();
			treeInfo = treeInfos [4];
			return treeInfo;
		}

		private BoundingBox GetBoundingBox()
		{
			BoundingBox boundingBox = new BoundingBox ();

			IList<string> boundingBoxText = boundingBoxTextBox.text.Split (',');

			boundingBox.MinimumLatitude = double.Parse (boundingBoxText [0].Trim ());
			boundingBox.MinimumLongitude = double.Parse (boundingBoxText [1].Trim ());
			boundingBox.MaximumLatitude = double.Parse (boundingBoxText [2].Trim ());
			boundingBox.MaximumLongitude = double.Parse (boundingBoxText [3].Trim ());

			return boundingBox;
		}

		private void InitializeTreeDropwdown(int x, int y)
		{
//			IEnumerable<string> treeNames = new List<string> ();
//
//			TreeCollection treeCollection = GameObject.FindObjectOfType<TreeCollection>();
//			IEnumerable<TreeInfo> treeInfos = treeCollection.m_prefabs;
//
//			treeNames = (from TreeInfo treeInfo in treeInfos
//			             select treeInfo.name);
//
//			InitializeDropdown (treeDropdown, treeNames, x, y);
		}
		
		private void SetButton(UIButton okButton, string p1,int x, int y)
		{
			okButton.text = p1;
			okButton.normalBgSprite = "ButtonMenu";
			okButton.hoveredBgSprite = "ButtonMenuHovered";
			okButton.disabledBgSprite = "ButtonMenuDisabled";
			okButton.focusedBgSprite = "ButtonMenuFocused";
			okButton.pressedBgSprite = "ButtonMenuPressed";
			okButton.size = new Vector2(50, 18);
			okButton.relativePosition = new Vector3(x, y - 3);
			okButton.textScale = 0.8f;
		}
		
		private void SetButton(UIButton okButton, string p1, int y)
		{
			okButton.text = p1;
			okButton.normalBgSprite = "ButtonMenu";
			okButton.hoveredBgSprite = "ButtonMenuHovered";
			okButton.disabledBgSprite = "ButtonMenuDisabled";
			okButton.focusedBgSprite = "ButtonMenuFocused";
			okButton.pressedBgSprite = "ButtonMenuPressed";
			okButton.size = new Vector2(260, 24);
			okButton.relativePosition = new Vector3((int)(width - okButton.size.x) / 2,y);
			okButton.textScale = 0.8f;
		}
		
		private void SetTextBox(UITextField scaleTextBox, string p, int x, int y)
		{
			scaleTextBox.relativePosition = new Vector3(x, y - 4);
			scaleTextBox.horizontalAlignment = UIHorizontalAlignment.Left;
			scaleTextBox.text = p;
			scaleTextBox.textScale = 0.8f;
			scaleTextBox.color = Color.black;
			scaleTextBox.cursorBlinkTime = 0.45f;
			scaleTextBox.cursorWidth = 1;
			scaleTextBox.selectionBackgroundColor = new Color(233,201,148,255);
			scaleTextBox.selectionSprite = "EmptySprite";
			scaleTextBox.verticalAlignment = UIVerticalAlignment.Middle;
			scaleTextBox.padding = new RectOffset(5, 0, 5, 0);
			scaleTextBox.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
			scaleTextBox.normalBgSprite = "TextFieldPanel";
			scaleTextBox.hoveredBgSprite = "TextFieldPanelHovered";
			scaleTextBox.focusedBgSprite = "TextFieldPanel";
			scaleTextBox.size = new Vector3(width - 120 - 30, 20);
			scaleTextBox.isInteractive = true;
			scaleTextBox.enabled = true;
			scaleTextBox.readOnly = false;
			scaleTextBox.builtinKeyNavigation = true;
		}

		private void InitializeDropdown(UIDropDown dropDown, IEnumerable<string> items, int x, int y)
		{
			dropDown.relativePosition = new Vector3(x, y - 4);
			dropDown.horizontalAlignment = UIHorizontalAlignment.Left;
			dropDown.textScale = 0.8f;
			dropDown.color = Color.black;
			dropDown.verticalAlignment = UIVerticalAlignment.Middle;
			dropDown.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
			dropDown.normalBgSprite = "TextFieldPanel";
			dropDown.hoveredBgSprite = "TextFieldPanelHovered";
			dropDown.focusedBgSprite = "TextFieldPanel";
			dropDown.size = new Vector3(width - 120 - 30, 20);
			dropDown.isInteractive = true;
			dropDown.enabled = true;
			dropDown.builtinKeyNavigation = true;
			foreach (string item in items) 
			{
				dropDown.AddItem (item);
			}
		}
		
		private void SetLabel(UILabel pedestrianLabel, string p, int x, int y)
		{
			pedestrianLabel.relativePosition = new Vector3(x, y);
			pedestrianLabel.text = p;
			pedestrianLabel.textScale = 0.8f;
			pedestrianLabel.size = new Vector3(120,20);
		}
	}
}