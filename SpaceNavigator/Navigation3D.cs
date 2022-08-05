namespace GeneralUnifiedTestSystemYard.SpaceNavigator;

using GeneralUnifiedTestSystemYard.Core;
using TDx.SpaceMouse.Navigation3D;
using SpaceMouse = TDx.SpaceMouse.Navigation3D.Navigation3D;

public class Navigation3D : INavigation3D,IGUTSYExtension
{
    private readonly SpaceMouse _navigation;
    public SpaceMouse Navigation => _navigation;

    public Matrix CameraMatrix { get; set; } = new();
    public Box ViewExtents { get; set; } = new();
    public double ViewFOV { get; set; } = 90;
    public Frustum ViewFrustrum { get; set; } = new();
    public bool ViewPerspective { get; set; } = true;
    public Point CameraTarget { get; set; } = new();
    public Plane ViewConstructionPlane { get; set; } = new();
    public bool ViewRotatable { get; set; } = true;
    public Point PointerPosition { get; set; } = new();



    public Point PivotPosition { get; set; } = new();
    public bool PivotVisible { get; set; } = false;
    public bool UserPivot { get; set; } = false;



    public Box ModelExtents { get; set; } = new();
    public Box SelectionExtents { get; set; } = new();
    public bool SelectionEmpty { get; set; } = true;
    public Matrix SelectionTransform { get; set; } = new();



    public Point LookAt { get; set; } = new();
    public Point LookFrom { get; set; } = new();
    public Vector LookDirection { get; set; } = new();
    public double LookAperture { get; set; } = 0;
    public bool SelectionOnly { get; set; } = false;



    public Matrix CoordinateSystem { get; set; } = new();
    public Matrix Frontview { get; set; } = new();

    public Navigation3D()
    {
        _navigation = new SpaceMouse(this);
        _navigation.MotionChanged += (sender, e) => Console.Out.WriteLine(e);
    }

    public string GetID() => "Navigation3D";

    /// <summary>
    /// Is called when the Navigation3D instance needs to get the camera matrix from the view.
    /// </summary>
    /// <returns>The camera matrix.</returns>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public Matrix GetCameraMatrix() => CameraMatrix;

    /// <summary>
    /// Is called when the Navigation3D instance needs to set the view's camera matrix.
    /// </summary>
    /// <param name="matrix">The camera <see cref="Matrix"/>.</param>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public void SetCameraMatrix(Matrix matrix) => CameraMatrix = matrix;

    /// <summary>
    /// Is called when the Navigation3D instance needs to get the extents of the view.
    /// </summary>
    /// <returns>The view's extents as a box or null.</returns>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public Box GetViewExtents() => ViewExtents;

    /// <summary>
    /// Is called when the Navigation3D instance needs to set the view's extents.
    /// </summary>
    /// <param name="extents">The view's extents to set.</param>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public void SetViewExtents(Box extents) => ViewExtents = extents;

    /// <summary>
    /// Is called when the Navigation3D instance needs to get the camera's field of view.
    /// </summary>
    /// <returns>The view's field of view.</returns>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public double GetViewFOV() => ViewFOV;

    /// <summary>
    /// Is called when the Navigation3D instance needs to set the camera's field of view.
    /// </summary>
    /// <param name="fov">The camera field of view to set.</param>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public void SetViewFOV(double fov) => ViewFOV = fov;

    /// <summary>
    /// Is called when the Navigation3D instance needs to get the view frustum.
    /// </summary>
    /// <returns>The view's frustum.</returns>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public Frustum GetViewFrustum() => ViewFrustrum;

    /// <summary>
    /// Is invoked when the Navigation3D instance needs to set the view frustum.
    /// </summary>
    /// <param name="frustum">The view <see cref="Frustum"/> to set.</param>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public void SetViewFrustum(Frustum frustum) => ViewFrustrum = frustum;

    /// <summary>
    /// Is invoked when the Navigation3D instance needs to get the view's projection type.
    /// </summary>
    /// <returns>true for a perspective view, false for an orthographic view, otherwise null.</returns>
    public bool IsViewPerspective() => ViewPerspective;

    /// <summary>
    /// Is invoked when the Navigation3D instance needs to get the camera's target.
    /// </summary>
    /// <returns>The position of the camera's target.</returns>
    /// <exception cref="TDx.SpaceMouse.Navigation3D.NoDataException">The camera does not have a target.</exception>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public Point GetCameraTarget() => CameraTarget;

    /// <summary>
    /// Is invoked when the Navigation3D instance needs to set the camera's target.
    /// </summary>
    /// <param name="target">The location of the camera's target to set.</param>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public void SetCameraTarget(Point target) => CameraTarget = target;

    /// <summary>
    /// Is invoked when the Navigation3D instance needs to get the view's construction plane.
    /// </summary>
    /// <returns>The <see cref="Plane"/> equation of the construction plane.</returns>
    /// <exception cref="TDx.SpaceMouse.Navigation3D.NoDataException">The view does not have a construction plane.</exception>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public Plane GetViewConstructionPlane() => ViewConstructionPlane;

    /// <summary>
    /// Is invoked when the Navigation3D instance needs to know whether the view can be rotated.
    /// </summary>
    /// <returns>true if the view can be rotated false if not, otherwise null.</returns>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public bool IsViewRotatable() => ViewRotatable;

    /// <summary>
    /// Is invoked when the Navigation3D instance needs to get the position of the pointer.
    /// </summary>
    /// <returns>The <see cref="Point"/> in world coordinates of the pointer on the projection plane.</returns>
    /// <exception cref="TDx.SpaceMouse.Navigation3D.NoDataException">The view does not have a pointer.</exception>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public Point GetPointerPosition() => PointerPosition;

    /// <summary>
    /// Is invoked when the Navigation3D instance needs to set the position of the pointer.
    /// </summary>
    /// <param name="position">The location of the pointer in world coordinates to set.</param>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public void SetPointerPosition(Point position)=> PointerPosition = position;



    /// <summary>
    /// Is called when the Navigation3D instance needs to get the position of the rotation pivot.
    /// </summary>
    /// <returns>The position of the pivot.</returns>
    /// <exception cref="TDx.SpaceMouse.Navigation3D.NoDataException">No pivot position.</exception>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public Point GetPivotPosition() => PivotPosition;

    /// <summary>
    /// Is called when the Navigation3D instance needs to set the position of the rotation pivot.
    /// </summary>
    /// <param name="value">The pivot <see cref="Point"/>.</param>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public void SetPivotPosition(Point position) => PivotPosition = position;

    /// <summary>
    /// Occurs when the Navigation3D instance needs to set the visibility of the pivot point.
    /// </summary>
    /// <param name="value">true if the pivot is visible otherwise false.</param>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public void SetPivotVisible(bool visible) => PivotVisible = visible;

    /// <summary>
    /// Is called when the Navigation3D instance needs to retrieve whether the user has manually set a pivot point.
    /// </summary>
    /// <returns>true if the user has set a pivot otherwise false.</returns>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public bool IsUserPivot() => UserPivot;



    /// <summary>
    /// Is called when the Navigation3D instance needs to get the extents of the model.
    /// </summary>
    /// <returns>The extents of the model in world coordinates.</returns>
    /// <exception cref="TDx.SpaceMouse.Navigation3D.NoDataException">There is no model in the scene.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public Box GetModelExtents() => ModelExtents;

    /// <summary>
    /// Is called when the Navigation3D instance needs to get the extents of the selection.
    /// </summary>
    /// <returns>The extents of the selection in world coordinates.</returns>
    /// <exception cref="TDx.SpaceMouse.Navigation3D.NoDataException">There is no selection.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public Box GetSelectionExtents() => SelectionExtents;

    /// <summary>
    /// Is called when the Navigation3D instance needs to get the extents of the selection.
    /// </summary>
    /// <returns>true if the selection set is empty, otherwise false.</returns>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public bool IsSelectionEmpty() => SelectionEmpty;

    /// <summary>
    /// Is called when the Navigation3D instance needs to get the selections's transform matrix.
    /// </summary>
    /// <returns>The selection's transform <see cref="Matrix"/> in world coordinates or null.</returns>
    /// <exception cref="TDx.SpaceMouse.Navigation3D.NoDataException">There is no selection.</exception>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public Matrix GetSelectionTransform() => SelectionTransform;

    /// <summary>
    /// Is called when the Navigation3D instance needs to set the selections's transform matrix.
    /// </summary>
    /// <param name="transform">The selection's transform <see cref="Matrix"/> in world coordinates.</param>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public void SetSelectionTransform(Matrix matrix) => SelectionTransform = matrix;



    /// <summary>
    /// Is called when the Navigation3D instance needs the result of the hit test.
    /// </summary>
    /// <returns>The hit position in world coordinates.</returns>
    /// <exception cref="TDx.SpaceMouse.Navigation3D.NoDataException">Nothing was hit.</exception>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public Point GetLookAt() => LookAt;

    /// <summary>
    /// Is called when the Navigation3D instance needs to set the source of the hit ray/cone.
    /// </summary>
    /// <param name="value">The source of the hit cone <see cref="Point"/>.</param>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public void SetLookFrom(Point eye) => LookFrom = eye;

    /// <summary>
    /// Is called when the Navigation3D instance needs to set the direction of the hit ray/cone.
    /// </summary>
    /// <param name="value">The direction of the ray/cone to set.</param>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public void SetLookDirection(Vector direction) => LookDirection = direction;

    /// <summary>
    /// Is called when the Navigation3D instance needs to set the aperture of the hit ray/cone.
    /// </summary>
    /// <param name="value">The aperture of the ray/cone on the near plane.</param>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public void SetLookAperture(double aperture) => LookAperture = aperture;

    /// <summary>
    /// Is called when the Navigation3D instance needs to set the selection filter.
    /// </summary>
    /// <param name="value">If true ignore non-selected items</param>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public void SetSelectionOnly(bool onlySelection) => SelectionOnly = onlySelection;



    /// <summary>
    /// Is called when the Navigation3D instance needs to get the coordinate system used by the client.
    /// </summary>
    /// <returns>The coordinate system matrix.</returns>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public Matrix GetCoordinateSystem() => CoordinateSystem;

    /// <summary>
    /// Is called when the Navigation3D instance needs to get the orientation of the front view.
    /// </summary>
    /// <returns>The orientation matrix of the front view.</returns>
    /// <exception cref="TDx.SpaceMouse.Navigation3D.NoDataException">No transform for the front view.</exception>
    /// <exception cref="System.InvalidOperationException">The call is invalid for the object's current state.</exception>
    /// <exception cref="System.NotImplementedException">The requested method or operation is not implemented.</exception>
    public Matrix GetFrontView() => Frontview;
}