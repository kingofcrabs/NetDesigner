using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using WW.Actions;
using WW.Cad.Base;
using WW.Cad.Drawing;
using WW.Cad.Drawing.GDI;
using WW.Cad.Model;
using WW.Cad.Model.Entities;

using WW.Math;
using WW.Math.Geometry;

namespace WinFormsEditExample {
    /// <summary>
    /// This is a control that shows a DxfModel.
    /// Dragging with the mouse pans the drawing.
    /// Clicking on the drawing selects the closest entity and
    /// shows it in the property grid.
    /// Using the scroll wheel zooms in on the mouse position.
    /// </summary>
    public partial class ViewControl : UserControl {
        private DxfModel modelFishNet;
        private DxfModel modelAxis;
        private DxfModel modelPoly;
        private GDIGraphics3D fishNetGraphics3D;
        private GDIGraphics3D axisGraphics3D;
        private GDIGraphics3D dynamicGdiGraphics3D;
        private GDIGraphics3D polyGdiGraphics3D;
        private Bounds3D bounds;
        private Matrix4D modelTransform = Matrix4D.Identity;
        private Vector3D translation = Vector3D.Zero;
        private Point lastMouseLocation;
        private Point mouseClickLocation;
        private double scaleFactor = 1d;
        private bool mouseDown;
        private IInteractor interactor;
        private IInteractorWinFormsDrawable interactorDrawable;

        public event EventHandler<EntityEventArgs> EntitySelected;

        public ViewControl() {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            {
                GraphicsConfig graphicsConfig = new GraphicsConfig(BackColor);
                fishNetGraphics3D = new GDIGraphics3D(graphicsConfig);
            }
            {
                GraphicsConfig graphicsConfig = new GraphicsConfig(BackColor);
                graphicsConfig.FixedForegroundColor = System.Drawing.Color.Red;
                dynamicGdiGraphics3D = new GDIGraphics3D(graphicsConfig);
            }
            {
                GraphicsConfig graphicsConfig = new GraphicsConfig(BackColor);
                axisGraphics3D = new GDIGraphics3D(graphicsConfig);
            }
            {
                GraphicsConfig graphicsConfig = new GraphicsConfig(BackColor);
                polyGdiGraphics3D = new GDIGraphics3D(graphicsConfig);
            }
            this.KeyUp += new KeyEventHandler(ViewControl_KeyUp);
            modelPoly = new DxfModel();
            bounds = new Bounds3D();
        }

       
        #region models & graphics
        public DxfModel ModelPoly
        {
            get
            {
                return modelPoly;
            }
            set
            {
                modelPoly = value;
                if (modelPoly != null)
                {
                    dynamicGdiGraphics3D.CreateDrawables(modelPoly);
                    dynamicGdiGraphics3D.BoundingBox(bounds, modelTransform);
                    CalculateTo2DTransform();
                    Invalidate();
                }
            }
        }

        public DxfModel ModelAxis
        {
            get
            {
                return modelAxis;
            }
            set
            {
                modelAxis = value;
                if (modelAxis != null)
                {
                    axisGraphics3D.CreateDrawables(modelAxis);
                    axisGraphics3D.BoundingBox(bounds, modelTransform);
                    CalculateTo2DTransform();
                    Invalidate();
                }
            }
        }

        public DxfModel ModelFishNet {
            get { 
                return modelFishNet; 
            }
            set { 
                modelFishNet = value;
                if (modelFishNet != null) {
                    fishNetGraphics3D.CreateDrawables(modelFishNet);
                    fishNetGraphics3D.BoundingBox(bounds, modelTransform);
                    CalculateTo2DTransform();
                    Invalidate();
                }
            }
        }
        public GDIGraphics3D PolyGraphics3D
        {
            get { return polyGdiGraphics3D; }
        }

        public GDIGraphics3D GdiGraphics3D {
            get { return fishNetGraphics3D; }
        }

        public GDIGraphics3D DynamicGdiGraphics3D {
            get { return dynamicGdiGraphics3D; }
        }
        #endregion
        public void StartInteraction(IInteractor interactor, IInteractorWinFormsDrawable interactorDrawable) {
            if (interactor != null) {
                this.interactor = interactor;
                this.interactorDrawable = interactorDrawable;
                interactor.Deactivated += interactor_Deactivated;
                interactor.Activate();
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            fishNetGraphics3D.Draw(e.Graphics, ClientRectangle);
            polyGdiGraphics3D.Draw(e.Graphics, ClientRectangle);
            dynamicGdiGraphics3D.Draw(e.Graphics, ClientRectangle);
            axisGraphics3D.Draw(e.Graphics, ClientRectangle);
        }

        private Matrix4D CalculateTo2DTransform() {
            Matrix4D to2DTransform = Matrix4D.Identity;
            if (axisGraphics3D != null) {
                if (modelFishNet != null && bounds != null) {
                    double halfHeight = ClientSize.Height / 2;
                    double halfWidth = ClientSize.Width / 2;
                    double margin = 5d; // 5 pixels margin on each size.
                    to2DTransform =
                        Transformation4D.Translation(translation) *
                        Transformation4D.Translation(halfHeight, halfWidth, 0) *
                        Transformation4D.Scaling(scaleFactor) *
                        Transformation4D.Translation(-halfHeight, -halfWidth, 0) *
                        DxfUtil.GetScaleTransform(
                            bounds.Corner1,
                            bounds.Corner2,
                            bounds.Center,
                            new Point3D(margin, ClientSize.Height - margin, 0d),
                            new Point3D(ClientSize.Width - margin, margin, 0d),
                            new Point3D(ClientSize.Width / 2, ClientSize.Height / 2, 0d)
                        );
                }
                axisGraphics3D.To2DTransform = to2DTransform * modelTransform;
                polyGdiGraphics3D.To2DTransform = axisGraphics3D.To2DTransform;
                fishNetGraphics3D.To2DTransform = axisGraphics3D.To2DTransform;
                dynamicGdiGraphics3D.To2DTransform = axisGraphics3D.To2DTransform;
            }
            return to2DTransform;
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            CalculateTo2DTransform();
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            lastMouseLocation = e.Location;
            mouseClickLocation = e.Location;
            mouseDown = true;
            if (
                interactor != null &&
                interactor.ProcessMouseButtonDown(new CanonicalMouseEventArgs(e), GetInteractionContext())
            ) {
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            bool interactorProcessedMouseMove = false;
            if (
                (
                    interactor == null || 
                    !(
                        interactorProcessedMouseMove = interactor.ProcessMouseMove(
                            new CanonicalMouseEventArgs(e), GetInteractionContext()
                        )
                    )
                ) && (
                    mouseDown
                )
            ) {
                //drag event handle
                int dx = (e.X - lastMouseLocation.X);
                int dy = (e.Y - lastMouseLocation.Y);
                translation += new Vector3D(dx, dy, 0);
                CalculateTo2DTransform();
                Invalidate();
            } else if (interactorProcessedMouseMove) {
                Invalidate();
            }
            lastMouseLocation = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            mouseDown = false;

            if (
                interactor == null || 
                !interactor.ProcessMouseButtonUp(new CanonicalMouseEventArgs(e), GetInteractionContext())
            ) {
                // Select entity at mouse location if mouse didn't move
                // and show entity in property grid.
                if (mouseClickLocation == e.Location) {
                    Point2D referencePoint = new Point2D(e.X, e.Y);
                    double distance;
                    IList<RenderedEntityInfo> closestEntities =
                        EntitySelector.GetClosestEntities(
                            modelFishNet,
                            GraphicsConfig.BlackBackground,
                            fishNetGraphics3D.To2DTransform,
                            referencePoint,
                            out distance
                        );
                    if (closestEntities.Count > 0) {
                        DxfEntity entity = closestEntities[0].Entity;
                        OnEntitySelected(new EntityEventArgs(entity));
                    }
                }
            } else {
                Invalidate();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);

            Matrix4D oldTo2DTransform = CalculateTo2DTransform();
            int compare = Math.Sign(e.Delta);
            // wheel movement is forward 
            if (compare > 0) {
                scaleFactor *= 1.1d;
            }
                // wheel movement is backward 
            else if (compare < 0) {
                scaleFactor /= 1.1d;
            }

            // --- Begin of correction on the translation to zoom into mouse position.
            // Comment out this section to zoom into center of model.
            Point3D currentScreenPoint = new Point3D(e.X, e.Y, 0d);
            Point3D modelPoint = oldTo2DTransform.GetInverse().Transform(currentScreenPoint);
            Matrix4D intermediateTo2DTransform = CalculateTo2DTransform();
            Point3D screenPoint = intermediateTo2DTransform.Transform(modelPoint);
            translation += (currentScreenPoint - screenPoint);
            // --- End of translation correction.

            CalculateTo2DTransform();

            Invalidate();
        }

        protected virtual void OnEntitySelected(EntityEventArgs e) {
            if (EntitySelected != null) {
                EntitySelected(this, e);
            }
        }
        void ViewControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteEntitiesInPoly();
                modelPoly.Entities.Clear();
                polyGdiGraphics3D.Clear();
                fishNetGraphics3D.CreateDrawables(modelFishNet);
                Invalidate();
            }
            
        }

        private void DeleteEntitiesInPoly()
        {
            if (modelPoly.Entities.Count == 0)
                return;
            for (int i = modelFishNet.Entities.Count - 1; i > -1; i--)
            {
                foreach (var polyEntity in modelPoly.Entities)
                {
                    if (polyEntity.InteractionControlPoints.Count < 3)
                        continue;
                    if (Polygon.IsInPolygon(modelFishNet.Entities[i].InteractionControlPoints, polyEntity.InteractionControlPoints))
                        modelFishNet.Entities.RemoveAt(i);
                }
            }
        }
        private void interactor_Deactivated(object sender, EventArgs e) {
            if (interactor != null) {
                interactor.Deactivated -= new EventHandler(interactor_Deactivated);
                interactor = null;
                fishNetGraphics3D.Clear();
                fishNetGraphics3D.CreateDrawables(modelFishNet);
                polyGdiGraphics3D.Clear();
                polyGdiGraphics3D.CreateDrawables(modelPoly);
                dynamicGdiGraphics3D.Clear();
                Invalidate();
            }
        }

        private InteractionContext GetInteractionContext() {
            return new InteractionContext(
                new Rectangle2D(ClientRectangle.Left, ClientRectangle.Top, ClientRectangle.Right, ClientRectangle.Bottom),
                fishNetGraphics3D.To2DTransform, 
                true,
                BackColor
            );
        }

    }
}
