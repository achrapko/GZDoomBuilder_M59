
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.GZBuilder;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class LinedefInfoPanel : UserControl
	{
		private int hexenformatwidth;
		private int doomformatwidth;
		private List<UniversalFieldInfo> fieldInfos;
		private int[] labelPositionsY = new[] { 39, 58, 77 }; //mxd
		
		// Constructor
		public LinedefInfoPanel()
		{
			// Initialize
			InitializeComponent();

			// Hide stuff when in Doom format
			hexenformatwidth = infopanel.Width;
			doomformatwidth = infopanel.Width - 190;
		}
		
		// This shows the info
		public void ShowInfo(Linedef l)
		{
			bool upperunpegged, lowerunpegged;
			string peggedness;
			int defaultPanelWidth = 270; //mxd

			//mxd
			if (General.Map.UDMF && fieldInfos == null) {
				fieldInfos = General.Map.Config.SidedefFields;
			}
			
			// Show/hide stuff depending on format
			if(!General.Map.FormatInterface.HasActionArgs)
			{
				arglbl1.Visible = false;
				arglbl2.Visible = false;
				arglbl3.Visible = false;
				arglbl4.Visible = false;
				arglbl5.Visible = false;
				arg1.Visible = false;
				arg2.Visible = false;
				arg3.Visible = false;
				arg4.Visible = false;
				arg5.Visible = false;
				infopanel.Width = doomformatwidth;
			}
			else
			{
				arglbl1.Visible = true;
				arglbl2.Visible = true;
				arglbl3.Visible = true;
				arglbl4.Visible = true;
				arglbl5.Visible = true;
				arg1.Visible = true;
				arg2.Visible = true;
				arg3.Visible = true;
				arg4.Visible = true;
				arg5.Visible = true;
				infopanel.Width = hexenformatwidth;
			}

			//mxd. Hide activation or tag and rearrange labels 
			if(!General.Map.FormatInterface.HasBuiltInActivations && General.Map.FormatInterface.HasNumericLinedefActivations) { //Hexen map format?
				activation.Visible = true;
				activationlabel.Visible = true;
				taglabel.Visible = false;
				tag.Visible = false;
				
				foreach(LinedefActivateInfo ai in General.Map.Config.LinedefActivates) {
					if(l.Activate == ai.Index) {
						activation.Text = ai.Title;
						break;
					}
				}

				activation.Top = labelPositionsY[0];
				activationlabel.Top = labelPositionsY[0];
				unpegged.Top = labelPositionsY[0];
				peglabel.Top = labelPositionsY[0];

				length.Top = labelPositionsY[1];
				lengthlabel.Top = labelPositionsY[1];
				frontoffset.Top = labelPositionsY[1];
				frontoffsetlabel.Top = labelPositionsY[1];

				angle.Top = labelPositionsY[2];
				anglelabel.Top = labelPositionsY[2];
				backoffset.Top = labelPositionsY[2];
				backoffsetlabel.Top = labelPositionsY[2];

			} else {
				activation.Visible = false;
				activationlabel.Visible = false;
				taglabel.Visible = true;
				tag.Visible = true;

				length.Top = labelPositionsY[0];
				lengthlabel.Top = labelPositionsY[0];
				unpegged.Top = labelPositionsY[0];
				peglabel.Top = labelPositionsY[0];

				angle.Top = labelPositionsY[1];
				anglelabel.Top = labelPositionsY[1];
				frontoffset.Top = labelPositionsY[1];
				frontoffsetlabel.Top = labelPositionsY[1];

				tag.Top = labelPositionsY[2];
				taglabel.Top = labelPositionsY[2];
				backoffset.Top = labelPositionsY[2];
				backoffsetlabel.Top = labelPositionsY[2];
			}
			
			// Get line action information
			LinedefActionInfo act = General.Map.Config.GetLinedefActionInfo(l.Action);
			
			// Determine peggedness
			upperunpegged = l.IsFlagSet(General.Map.Config.UpperUnpeggedFlag);
			lowerunpegged = l.IsFlagSet(General.Map.Config.LowerUnpeggedFlag);
			if(upperunpegged && lowerunpegged)
				peggedness = "Upper & Lower";
			else if(upperunpegged)
				peggedness = "Upper";
			else if(lowerunpegged)
				peggedness = "Lower";
			else
				peggedness = "None";
			
			// Linedef info
			infopanel.Text = " Linedef " + l.Index + " ";
			action.Text = act.ToString();
			length.Text = l.Length.ToString("0.##");
			angle.Text = l.AngleDeg + "\u00B0";
			if(tag.Visible) { //mxd
				tag.Text = l.Tag + (General.Map.Options.TagLabels.ContainsKey(l.Tag) ? " (" + General.Map.Options.TagLabels[l.Tag] + ")" : string.Empty);
			}
			unpegged.Text = peggedness;

			//mxd
			bool hasArg0Str = General.Map.UDMF && Array.IndexOf(GZGeneral.ACS_SPECIALS, l.Action) != -1 && l.Fields.ContainsKey("arg0str");
			
			// Arguments
			arglbl1.Text = hasArg0Str ? "Script name:" : act.Args[0].Title + ":"; //mxd
			arglbl2.Text = act.Args[1].Title + ":";
			arglbl3.Text = act.Args[2].Title + ":";
			arglbl4.Text = act.Args[3].Title + ":";
			arglbl5.Text = act.Args[4].Title + ":";
			arglbl1.Enabled = act.Args[0].Used;
			arglbl2.Enabled = act.Args[1].Used;
			arglbl3.Enabled = act.Args[2].Used;
			arglbl4.Enabled = act.Args[3].Used;
			arglbl5.Enabled = act.Args[4].Used;
			arg1.Enabled = act.Args[0].Used;
			arg2.Enabled = act.Args[1].Used;
			arg3.Enabled = act.Args[2].Used;
			arg4.Enabled = act.Args[3].Used;
			arg5.Enabled = act.Args[4].Used;

			//mxd
			if (hasArg0Str) {
				arg1.Text = '"' + l.Fields["arg0str"].Value.ToString() + '"';
			} else {
				setArgumentText(act.Args[0], arg1, l.Args[0]);
			}
			setArgumentText(act.Args[1], arg2, l.Args[1]);
			setArgumentText(act.Args[2], arg3, l.Args[2]);
			setArgumentText(act.Args[3], arg4, l.Args[3]);
			setArgumentText(act.Args[4], arg5, l.Args[4]);

			// Front side available?
			if(l.Front != null)
			{
				int addedWidth = 0; //mxd
				
				// Show sidedef info
				frontpanel.Visible = true; //mxd

				frontpanel.Text = " Front Sidedef " + l.Front.Index;
				frontsector.Text = " Sector " + l.Front.Sector.Index;
				frontsector.Visible = true;
				
				//mxd
				if(General.Map.UDMF) {
					//light
					frontoffsetlabel.Text = "Front light:";
					setUDMFLight(l.Front, frontoffsetlabel, frontoffset);

					//global offset
					frontpanel.Text += ". Offset: " + l.Front.OffsetX + ", " + l.Front.OffsetY;

					bool hasTopFields = false;
					bool hasMiddleFields = false;
					bool hasBottomFields = false;
					
					//sidedef top
					if(checkPairedUDMFFields(l.Front.Fields, "offsetx_top", "offsety_top", frontTopUDMFOffsetLabel, frontTopUDMFOffset))
						hasTopFields = true;
					if (checkPairedUDMFFields(l.Front.Fields, "scalex_top", "scaley_top", frontTopUDMFScaleLabel, frontTopUDMFScale))
						hasTopFields = true;

					//sidedef middle
					if (checkPairedUDMFFields(l.Front.Fields, "offsetx_mid", "offsety_mid", frontMidUDMFOffsetLabel, frontMidUDMFOffset))
						hasMiddleFields = true;
					if (checkPairedUDMFFields(l.Front.Fields, "scalex_mid", "scaley_mid", frontMidUDMFScaleLabel, frontMidUDMFScale))
						hasMiddleFields = true;

					//sidedef bottom
					if (checkPairedUDMFFields(l.Front.Fields, "offsetx_bottom", "offsety_bottom", frontBottomUDMFOffsetLabel, frontBottomUDMFOffset))
						hasBottomFields = true;
					if (checkPairedUDMFFields(l.Front.Fields, "scalex_bottom", "scaley_bottom", frontBottomUDMFScaleLabel, frontBottomUDMFScale))
						hasBottomFields = true;

					//visibility
					panelUDMFFrontTop.Visible = hasTopFields;
					panelUDMFFrontMid.Visible = hasMiddleFields;
					panelUDMFFrontBottom.Visible = hasBottomFields;

					//size
					if(hasTopFields) addedWidth = 64;
					if(hasMiddleFields) addedWidth += 64;
					if(hasBottomFields) addedWidth += 64;
				} else {
					frontoffsetlabel.Text = "Front offset:";
					frontoffset.Text = l.Front.OffsetX + ", " + l.Front.OffsetY;
					frontoffsetlabel.Enabled = true;
					frontoffset.Enabled = true;

					panelUDMFFrontTop.Visible = false;
					panelUDMFFrontMid.Visible = false;
					panelUDMFFrontBottom.Visible = false;
				}

				//mxd. Resize panel
				frontpanel.Width = defaultPanelWidth + addedWidth + 12;
				flowLayoutPanelFront.Width = defaultPanelWidth + addedWidth;

				fronthighname.Text = l.Front.HighTexture;
				frontmidname.Text = l.Front.MiddleTexture;
				frontlowname.Text = l.Front.LowTexture;
				DisplaySidedefTexture(fronthightex, labelTextureFrontTop, l.Front.HighTexture, l.Front.HighRequired());
				DisplaySidedefTexture(frontmidtex, labelTextureFrontMid, l.Front.MiddleTexture, l.Front.MiddleRequired());
				DisplaySidedefTexture(frontlowtex, labelTextureFrontBottom, l.Front.LowTexture, l.Front.LowRequired());

				//mxd. Position label
				frontsector.Left = frontpanel.Width - frontsector.Width - 12;
			}
			else
			{
				// Show no info
				//mxd
				if(General.Map.UDMF) {
					frontoffsetlabel.Text = "Front light:";
					frontoffset.Text = "--";
				} else {
					frontoffsetlabel.Text = "Front offset:";
					frontoffset.Text = "--, --";
				}

				frontoffsetlabel.Enabled = false;
				frontoffset.Enabled = false;

				fronthightex.BackgroundImage = null;
				frontmidtex.BackgroundImage = null;
				frontlowtex.BackgroundImage = null;

				frontpanel.Visible = false; //mxd
			}

			// Back size available?
			if(l.Back != null)
			{
				int addedWidth = 0; //mxd
				
				// Show sidedef info
				backpanel.Visible = true; //mxd
				backpanel.Text = " Back Sidedef " + l.Back.Index;
				backsector.Text = " Sector " + l.Back.Sector.Index;
				backsector.Visible = true;

				//mxd
				if(General.Map.UDMF) {
					//light
					backoffsetlabel.Text = "Back light:";
					setUDMFLight(l.Back, backoffsetlabel, backoffset);

					//global offset
					backpanel.Text += ". Offset: " + l.Back.OffsetX + ", " + l.Back.OffsetY;

					bool hasTopFields = false;
					bool hasMiddleFields = false;
					bool hasBottomFields = false;

					//sidedef top
					if (checkPairedUDMFFields(l.Back.Fields, "offsetx_top", "offsety_top", backTopUDMFOffsetLabel, backTopUDMFOffset))
						hasTopFields = true;
					if (checkPairedUDMFFields(l.Back.Fields, "scalex_top", "scaley_top", backTopUDMFScaleLabel, backTopUDMFScale))
						hasTopFields = true;

					//sidedef middle
					if (checkPairedUDMFFields(l.Back.Fields, "offsetx_mid", "offsety_mid", backMidUDMFOffsetLabel, backMidUDMFOffset))
						hasMiddleFields = true;
					if (checkPairedUDMFFields(l.Back.Fields, "scalex_mid", "scaley_mid", backMidUDMFScaleLabel, backMidUDMFScale))
						hasMiddleFields = true;

					//sidedef bottom
					if (checkPairedUDMFFields(l.Back.Fields, "offsetx_bottom", "offsety_bottom", backBottomUDMFOffsetLabel, backBottomUDMFOffset))
						hasBottomFields = true;
					if (checkPairedUDMFFields(l.Back.Fields, "scalex_bottom", "scaley_bottom", backBottomUDMFScaleLabel, backBottomUDMFScale))
						hasBottomFields = true;

					//visibility
					panelUDMFBackTop.Visible = hasTopFields;
					panelUDMFBackMid.Visible = hasMiddleFields;
					panelUDMFBackBottom.Visible = hasBottomFields;

					//size
					if(hasTopFields) addedWidth = 64;
					if(hasMiddleFields) addedWidth += 64;
					if(hasBottomFields) addedWidth += 64;
				} else {
					backoffsetlabel.Text = "Back offset:";
					backoffset.Text = l.Back.OffsetX + ", " + l.Back.OffsetY;
					backoffsetlabel.Enabled = true;
					backoffset.Enabled = true;

					panelUDMFBackTop.Visible = false;
					panelUDMFBackMid.Visible = false;
					panelUDMFBackBottom.Visible = false;
				}

				//mxd. Resize panel
				backpanel.Width = defaultPanelWidth + addedWidth + 12;
				flowLayoutPanelBack.Width = defaultPanelWidth + addedWidth;

				backhighname.Text = l.Back.HighTexture;
				backmidname.Text = l.Back.MiddleTexture;
				backlowname.Text = l.Back.LowTexture;
				DisplaySidedefTexture(backhightex, labelTextureBackTop, l.Back.HighTexture, l.Back.HighRequired());
				DisplaySidedefTexture(backmidtex, labelTextureBackMid, l.Back.MiddleTexture, l.Back.MiddleRequired());
				DisplaySidedefTexture(backlowtex, labelTextureBackBottom, l.Back.LowTexture, l.Back.LowRequired());

				//mxd. Position label
				backsector.Left = backpanel.Width - backsector.Width - 12;
			}
			else
			{
				// Show no info
				//mxd
				if(General.Map.UDMF) {
					backoffsetlabel.Text = "Back light:";
					backoffset.Text = "--";
				} else {
					backoffsetlabel.Text = "Back offset:";
					backoffset.Text = "--, --";
				}

				backoffsetlabel.Enabled = false;
				backoffset.Enabled = false;

				backhightex.BackgroundImage = null;
				backmidtex.BackgroundImage = null;
				backlowtex.BackgroundImage = null;

				backpanel.Visible = false; //mxd
			}

			//mxd. Flags
			Dictionary<string, string> activations = new Dictionary<string, string>();
			foreach(LinedefActivateInfo ai in General.Map.Config.LinedefActivates) {
				activations.Add(ai.Key, ai.Title);
			}

			flags.Items.Clear();
			foreach(KeyValuePair<string, bool> group in l.Flags) {
				if(group.Value) {
					ListViewItem item;
					if (General.Map.Config.LinedefFlags.ContainsKey(group.Key)) {
						item = new ListViewItem(General.Map.Config.LinedefFlags[group.Key]);
					} else if (activations.ContainsKey(group.Key)) {
						item = new ListViewItem(activations[group.Key]);
					} else {
						item = new ListViewItem(group.Key);
					}

					item.Checked = true;
					flags.Items.Add(item);
				}
			}

			//mxd. Flags panel visibility and size
			flagsPanel.Visible = (flags.Items.Count > 0);
			if(flags.Items.Count > 0) {
				int itemWidth = flags.Items[0].GetBounds(ItemBoundsPortion.Entire).Width;
				if(itemWidth == 0) itemWidth = 96;
				flags.Width = itemWidth * (int)Math.Ceiling(flags.Items.Count / 5.0f);
				flagsPanel.Width = flags.Width + flags.Left * 2;
			}

			// Show the whole thing
			this.Show();
			this.Update();
		}

		//mxd
		private bool checkPairedUDMFFields(UniFields fields, string paramX, string paramY, Label label, Label value) {
			float dx = getDefaultUDMFValue(paramX);
			float dy = getDefaultUDMFValue(paramY);
			float x = dx;
			float y = dy;

			if(fields.ContainsKey(paramX))
				x = (float)fields[paramX].Value;
			if(fields.ContainsKey(paramY))
				y = (float)fields[paramY].Value;

			if(x != dx || y != dy) {
				value.Text = String.Format("{0:0.##}", x) + ", " + String.Format("{0:0.##}", y);
				value.Enabled = true;
				label.Enabled = true;
				return true;
			}

			value.Text = "--, --";
			value.Enabled = false;
			label.Enabled = false;
			return false;
		}

		//mxd
		private void setUDMFLight(Sidedef sd, Label label, Label value) {
			if(sd.Fields.ContainsKey("light")) {
				int light = (int)sd.Fields["light"].Value;
				
				if (sd.Fields.ContainsKey("lightabsolute") && Boolean.Parse(sd.Fields["lightabsolute"].Value.ToString()))
					value.Text = light + " (abs.)";
				else
					value.Text = light + " (" + Math.Min(255, Math.Max(0, (light + sd.Sector.Brightness))) + ")";

				value.Enabled = true;
				label.Enabled = true;
			} else {
				value.Text = "--";
				label.Enabled = false;
				value.Enabled = false;
			}
		}

		//mxd
		private float getDefaultUDMFValue(string valueName) {
			foreach (UniversalFieldInfo fi in fieldInfos) {
				if (fi.Name == valueName) return (float)fi.Default;
			}
			return 0;
		}

		//mxd
		private void setArgumentText(ArgumentInfo info, Label label, int value) {
			TypeHandler th = General.Types.GetArgumentHandler(info);
			th.SetValue(value);
			label.Text = th.GetStringValue();

			if(value < 1 || !General.Map.Options.TagLabels.ContainsKey(value)) return;

			if(th is ThingTagHandler || th is LinedefTagHandler || th is SectorTagHandler) {
				label.Text += " (" + General.Map.Options.TagLabels[value] + ")";
			}
		}

		// When visible changed
		protected override void OnVisibleChanged(EventArgs e)
		{
			// Hiding panels
			if(!this.Visible)
			{
				fronthightex.BackgroundImage = null;
				frontmidtex.BackgroundImage = null;
				frontlowtex.BackgroundImage = null;
				backhightex.BackgroundImage = null;
				backmidtex.BackgroundImage = null;
				backlowtex.BackgroundImage = null;
			}

			// Call base
			base.OnVisibleChanged(e);
		}

		// This shows a sidedef texture in a panel
		private void DisplaySidedefTexture(Panel panel, Label label, string name, bool required)
		{
			// Check if name is a "none" texture
			if((name.Length < 1) || (name == "-"))
			{
				label.Visible = false; //mxd
				
				// Determine image to show
				if(required) 
					General.DisplayZoomedImage(panel, Properties.Resources.MissingTexture);
				else
					panel.BackgroundImage = null;
			}
			else
			{
				//mxd
				ImageData texture = General.Map.Data.GetTextureImage(name);
				if(General.Settings.ShowTextureSizes && texture.ImageState == ImageLoadState.Ready && !(texture is UnknownImage)) {
					label.Visible = true;
					label.Text = texture.ScaledWidth + "x" + texture.ScaledHeight;
				} else {
					label.Visible = false;
				}
				
				// Set the image
				General.DisplayZoomedImage(panel, texture.GetPreview());
			}
		}
	}
}
