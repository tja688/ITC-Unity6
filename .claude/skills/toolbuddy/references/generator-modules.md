# Curvy Generator Modules (Index)

Location: Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/CG Modules

How to use
- Use this index to locate the module file.
- Open the module class and read [SerializeField] and Refresh/Process methods for exact parameters.

Input
- InputSplinePath.cs: Provide a spline path as CGPath.
- InputSplineShape.cs: Provide a spline shape as CGShape.
- InputMesh.cs: Provide a Mesh input.
- InputGameObject.cs: Provide GameObject input.
- InputSpots.cs: Provide CGSpots input.
- InputTransformSpots.cs: Provide Transform spots input.

Build / Create
- BuildRasterizedPath.cs: Convert path to rasterized path.
- BuildShapeExtrusion.cs: Extrude shape along path.
- BuildVolumeCaps.cs: Generate caps for volumes.
- BuildVolumeMesh.cs: Build a volume mesh from volume data.
- BuildVolumeSpots.cs: Generate spots along a volume.
- CreateMesh.cs: Create mesh and optional colliders/renderers.
- CreateGameObject.cs: Instantiate game objects from spots or paths.
- CreatePathLineRenderer.cs: LineRenderer from path.

Modify
- ConformPath.cs: Conform a path to a surface.
- DeformMesh.cs: Deform a mesh along a path/shape.
- ModifierMixPaths.cs: Blend multiple paths.
- ModifierMixShapes.cs: Blend multiple shapes.
- ModifierVariableMixShapes.cs: Variable blending across path.
- ModifierPathRelativeTranslation.cs: Offset path along its own reference frame.
- ModifierTRSPath.cs: Transform a path.
- ModifierTRSShape.cs: Transform a shape.
- ModifierTRSMesh.cs: Transform a mesh.
- GameObjectToMesh.cs: Convert GameObject to Mesh data.

Debug / Utility
- DebugRasterizedPath.cs
- DebugVMesh.cs
- DebugVolume.cs
- Note.cs
- ResourceExportingModule.cs

Related Base Classes
- Base module types live under: Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/CG Modules/Base
