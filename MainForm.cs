using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WW.Cad.Model;
using WW.Cad.Actions;
using WW.Cad.Model.Entities;
using WW.Math;

namespace WinFormsEditExample {
    /// <summary>
    /// The main form with a property grid on the left
    /// and a DXF view control on the right.
    /// </summary>
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        public MainForm(DxfModel model) {
            InitializeComponent();
            viewControl.ModelFishNet = model;
            viewControl.ModelAxis = CreateModelWithAxes();
        }

        private static DxfModel CreateModelWithAxes()
        {
            DxfModel model;
            model = new DxfModel();

            DxfLine xaxis = new DxfLine(new Point2D(0d, 0d), new Point2D(10d, 0d));
            xaxis.Color = EntityColors.LightGray;
            model.Entities.Add(xaxis);

            DxfLine yaxis = new DxfLine(new Point2D(0d, 0d), new Point2D(0d, 10d));
            yaxis.Color = EntityColors.LightGray;
            model.Entities.Add(yaxis);

            DxfText originLabel = new DxfText("0", new Point3D(-1d, -1d, 0d), 0.5d);
            originLabel.Color = EntityColors.GreenYellow;
            model.Entities.Add(originLabel);

            DxfText xaxisLabel = new DxfText("10", new Point3D(10d, -1d, 0d), 0.5d);
            xaxisLabel.Color = EntityColors.GreenYellow;
            model.Entities.Add(xaxisLabel);

            DxfText yaxisLabel = new DxfText("10", new Point3D(-1, 10d, 0d), 0.5d);
            yaxisLabel.Color = EntityColors.GreenYellow;
            model.Entities.Add(yaxisLabel);

            return model;
        }

        private void viewControl_EntitySelected(object sender, EntityEventArgs e) {
        }

        private void addLwPolylineToolStripMenuItem_Click(object sender, EventArgs e) {
            DxfLwPolylineCreator interactor =
                new DxfLwPolylineCreator(viewControl.ModelPoly, viewControl.PolyGraphics3D.GraphicsConfig.NodeSize);
            viewControl.StartInteraction(
                interactor, 
                new DxfLwPolylineCreator.WinFormsDrawable(interactor, viewControl.DynamicGdiGraphics3D)
            );
            viewControl.ModelPoly.Entities.Add(interactor.Entity);
            interactor.Entity.Color = EntityColors.Yellow;
        }

        private void addLineToolStripMenuItem_Click(object sender, EventArgs e) {
            DxfLineCreator interactor = 
                new DxfLineCreator(viewControl.ModelFishNet, viewControl.GdiGraphics3D.GraphicsConfig.NodeSize);
            viewControl.StartInteraction(
                interactor, 
                new DxfLineCreator.WinFormsDrawable(interactor, viewControl.DynamicGdiGraphics3D)
            );
            viewControl.ModelFishNet.Entities.Add(interactor.Entity);
            interactor.Entity.Color = EntityColors.Green;
        }
    }
}
