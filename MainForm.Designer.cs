namespace WinFormsEditExample {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            WW.Cad.Model.DxfModel dxfModel1 = new WW.Cad.Model.DxfModel();
            WW.Cad.Model.Objects.DxfLayout dxfLayout1 = new WW.Cad.Model.Objects.DxfLayout();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            WW.Cad.Model.Tables.DxfUcs dxfUcs1 = new WW.Cad.Model.Tables.DxfUcs();
            WW.Cad.Model.Tables.DxfLineType dxfLineType1 = new WW.Cad.Model.Tables.DxfLineType();
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.viewControl = new WinFormsEditExample.ViewControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.addLwPolylineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFishNetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Controls.Add(this.viewControl);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(792, 544);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.Size = new System.Drawing.Size(792, 568);
            this.toolStripContainer.TabIndex = 2;
            this.toolStripContainer.Text = "toolStripContainer1";
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.menuStrip1);
            // 
            // viewControl
            // 
            this.viewControl.BackColor = System.Drawing.Color.Black;
            this.viewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewControl.Location = new System.Drawing.Point(0, 0);
            this.viewControl.ModelFishNet = null;
            this.viewControl.ModelAxis = null;
            dxfLayout1.CurrentStyleSheet = "";
            dxfLayout1.CustomPrintScaleDenominator = 1D;
            dxfLayout1.CustomPrintScaleNumerator = 1D;
            dxfLayout1.Elevation = 0D;
            dxfLayout1.ExtensionDictionary = null;
            dxfLayout1.InsertionBasePoint = ((WW.Math.Point3D)(resources.GetObject("dxfLayout1.InsertionBasePoint")));
            dxfLayout1.LastActiveViewport = null;
            dxfLayout1.Limits = ((WW.Math.Geometry.Rectangle2D)(resources.GetObject("dxfLayout1.Limits")));
            dxfLayout1.MaxExtents = ((WW.Math.Point3D)(resources.GetObject("dxfLayout1.MaxExtents")));
            dxfLayout1.MinExtents = ((WW.Math.Point3D)(resources.GetObject("dxfLayout1.MinExtents")));
            dxfLayout1.Name = "Layout1";
            dxfLayout1.Options = WW.Cad.Model.Objects.LayoutOptions.PaperSpaceLinetypeScaling;
            dxfLayout1.PageSetupName = "";
            dxfLayout1.PaperImageOrigin = ((WW.Math.Point2D)(resources.GetObject("dxfLayout1.PaperImageOrigin")));
            dxfLayout1.PaperSizeName = "";
            dxfLayout1.PlotConfigurationFile = "C:\\Program Files\\AutoCAD 2002\\plotters\\DWF ePlot (optimized for plotting).pc3";
            dxfLayout1.PlotLayoutFlags = ((WW.Cad.Model.Objects.PlotLayoutFlags)((((WW.Cad.Model.Objects.PlotLayoutFlags.UseStandardScale | WW.Cad.Model.Objects.PlotLayoutFlags.PlotPlotStyles) 
            | WW.Cad.Model.Objects.PlotLayoutFlags.PrintLineweights) 
            | WW.Cad.Model.Objects.PlotLayoutFlags.DrawViewportsFirst)));
            dxfLayout1.PlotOrigin = ((WW.Math.Point2D)(resources.GetObject("dxfLayout1.PlotOrigin")));
            dxfLayout1.PlotPaperSize = ((WW.Math.Size2D)(resources.GetObject("dxfLayout1.PlotPaperSize")));
            dxfLayout1.PlotPaperUnits = WW.Cad.Model.Objects.PlotPaperUnits.Millimeters;
            dxfLayout1.PlotRotation = WW.Cad.Model.Objects.PlotRotation.QuarterCounterClockwise;
            dxfLayout1.PlotType = WW.Cad.Model.Objects.PlotType.DrawingExtents;
            dxfLayout1.PlotViewName = "";
            dxfLayout1.PlotWindowArea = ((WW.Math.Geometry.Rectangle2D)(resources.GetObject("dxfLayout1.PlotWindowArea")));
            dxfLayout1.ShadePlotCustomDpi = ((short)(0));
            dxfLayout1.ShadePlotMode = WW.Cad.Model.ShadePlotMode.AsDisplayed;
            dxfLayout1.ShadePlotResolution = WW.Cad.Model.Objects.ShadePlotResolution.Draft;
            dxfLayout1.StandardScaleFactor = 1D;
            dxfLayout1.StandardScaleType = ((short)(16));
            dxfLayout1.TabOrder = 1;
            dxfUcs1.Elevation = 0D;
            dxfUcs1.ExtensionDictionary = null;
            dxfUcs1.IsExternallyDependent = false;
            dxfUcs1.IsReferenced = false;
            dxfUcs1.IsResolvedExternalRef = false;
            dxfUcs1.Name = null;
            dxfUcs1.Origin = ((WW.Math.Point3D)(resources.GetObject("dxfUcs1.Origin")));
            dxfUcs1.OrthographicBackDOrigin = ((WW.Math.Vector3D)(resources.GetObject("dxfUcs1.OrthographicBackDOrigin")));
            dxfUcs1.OrthographicBottomDOrigin = ((WW.Math.Vector3D)(resources.GetObject("dxfUcs1.OrthographicBottomDOrigin")));
            dxfUcs1.OrthographicFrontDOrigin = ((WW.Math.Vector3D)(resources.GetObject("dxfUcs1.OrthographicFrontDOrigin")));
            dxfUcs1.OrthographicLeftDOrigin = ((WW.Math.Vector3D)(resources.GetObject("dxfUcs1.OrthographicLeftDOrigin")));
            dxfUcs1.OrthographicReference = null;
            dxfUcs1.OrthographicRightDOrigin = ((WW.Math.Vector3D)(resources.GetObject("dxfUcs1.OrthographicRightDOrigin")));
            dxfUcs1.OrthographicTopDOrigin = ((WW.Math.Vector3D)(resources.GetObject("dxfUcs1.OrthographicTopDOrigin")));
            dxfUcs1.OrthographicViewType = WW.Cad.Model.OrthographicType.None;
            dxfUcs1.XAxis = ((WW.Math.Vector3D)(resources.GetObject("dxfUcs1.XAxis")));
            dxfUcs1.YAxis = ((WW.Math.Vector3D)(resources.GetObject("dxfUcs1.YAxis")));
            dxfLayout1.Ucs = dxfUcs1;
            dxfLayout1.UcsOrthographicType = WW.Cad.Model.OrthographicType.None;
            dxfLayout1.UnprintableMarginBottom = 0D;
            dxfLayout1.UnprintableMarginLeft = 0D;
            dxfLayout1.UnprintableMarginRight = 0D;
            dxfLayout1.UnprintableMarginTop = 0D;
            dxfLineType1.Description = "";
            dxfLineType1.ExtensionDictionary = null;
            dxfLineType1.IsExternallyDependent = false;
            dxfLineType1.IsReferenced = true;
            dxfLineType1.IsResolvedExternalRef = false;
            dxfLineType1.Name = "ByLayer";
            dxfModel1.ByLayerLineType = dxfLineType1;
            dxfModel1.Filename = null;
            dxfModel1.NumberOfSaves = 1;
            dxfModel1.SecurityFlags = WW.Cad.Model.SecurityFlags.None;
            this.viewControl.ModelPoly = dxfModel1;
            this.viewControl.Name = "viewControl";
            this.viewControl.Size = new System.Drawing.Size(792, 544);
            this.viewControl.TabIndex = 0;
            this.viewControl.EntitySelected += new System.EventHandler<WinFormsEditExample.EntityEventArgs>(this.viewControl_EntitySelected);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLwPolylineToolStripMenuItem,
            this.addLineToolStripMenuItem,
            this.addFishNetToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(792, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip";
            // 
            // addLwPolylineToolStripMenuItem
            // 
            this.addLwPolylineToolStripMenuItem.Name = "addLwPolylineToolStripMenuItem";
            this.addLwPolylineToolStripMenuItem.Size = new System.Drawing.Size(114, 20);
            this.addLwPolylineToolStripMenuItem.Text = "Add LWPOLYLINE";
            this.addLwPolylineToolStripMenuItem.Click += new System.EventHandler(this.addLwPolylineToolStripMenuItem_Click);
            // 
            // addLineToolStripMenuItem
            // 
            this.addLineToolStripMenuItem.Name = "addLineToolStripMenuItem";
            this.addLineToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.addLineToolStripMenuItem.Text = "Add LINE";
            this.addLineToolStripMenuItem.Click += new System.EventHandler(this.addLineToolStripMenuItem_Click);
            // 
            // addFishNetToolStripMenuItem
            // 
            this.addFishNetToolStripMenuItem.Name = "addFishNetToolStripMenuItem";
            this.addFishNetToolStripMenuItem.Size = new System.Drawing.Size(84, 20);
            this.addFishNetToolStripMenuItem.Text = "Add FishNet";
            this.addFishNetToolStripMenuItem.Click += new System.EventHandler(this.addFishNetToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 568);
            this.Controls.Add(this.toolStripContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "WW.Cad Advanced Viewer example (drag to pan, use scroll wheel for zoom, click for" +
    " selection)";
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.PerformLayout();
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ViewControl viewControl;
        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addLwPolylineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addFishNetToolStripMenuItem;
    }
}
