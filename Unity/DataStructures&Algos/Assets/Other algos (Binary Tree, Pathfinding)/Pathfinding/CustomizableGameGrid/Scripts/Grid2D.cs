using System;
using System.Collections.Generic;
using UnityEngine;

namespace TechnOllieG.CustomizableGameGrid
{
	// ===============================================================================================
	// ================================== Grid2D.cs by TechnOllieG ===================================
	// ===== Purpose: To generate a 2D plane of grid points, can be used in both 2D and 3D games =====
	// ===============================================================================================
	[DefaultExecutionOrder(-50)]
	public class Grid2D : MonoBehaviour
	{
		public enum PointVisualizationMode
		{
			Path,
			EndOfPath
		}
		
		public enum Axes
		{
			XY,
			XZ,
			ZY
		}
    
		public enum GridCorners
		{
			BottomLeft = 0,
			BottomRight = 1,
			TopLeft = 2,
			TopRight = 3
		}

		public enum ObjectSnapping
		{
			MeshBoundsTopToGrid,
			Centered,
			MeshBoundsBottomToGrid
		}

		#region Inspector Variables
		public Material defaultGridPointMaterial = default;
		public Material[] visualizedPointMaterials = default;
		public Axes gridAxes = Axes.XZ;
		public int gridPointWidth = 1;
		public int gridPointLength = 1;
		public float scaleWidth = 1f;
		public float scaleDepth = 1f;
		public float scaleLength = 1f;
		public bool instantiateGridPointObjects = true;
		public ObjectSnapping gridPointObjectSnapping = ObjectSnapping.MeshBoundsTopToGrid;
		public bool customGridPointObjects = false;
		public GameObject customObjectPrefab = default;
		public bool calculateGridLines = true;
		public bool showGridLines = true;
		public Color gridLineColor = Color.gray;
		public float gridLineDepthOffset = 0f;
		public bool customizeLineWidth = false;
		public float customLineWidth = 0f;

		// ========= DEBUG =========
        
		public bool showGridLinesInEditor = false;
		public Color gridLineEditorColor = Color.red;
		public float gridLineEditorDepthOffset = 0f;
		public bool showGridCornersInEditor = false;
		public Color gridCornerEditorColor = Color.red;
		public float gridCornerRadius = 0.2f;
		#endregion
        
		#region Public Variables
		public Vector3[] gridPoints;
		public GameObject[] gridPointObjects;
		public List<OccupiedPointData> occupiedGridPoints = new List<OccupiedPointData>();
        
		// Gridlines is an array of lines that contains coordinates of the vertices of all grid lines that should be drawn. The size of the gridLine array should always be a multiple of 2
		public Vector3[] gridLines = null;
		public Vector3[] gridCorners = null;
		#endregion

		#region Local Variables
		[SerializeField] protected Vector3 bottomLeftCoordinate;
		protected List<MeshRenderer> _visualizedPoints = new List<MeshRenderer>();
		protected Vector3 _gridPointScale = Vector3.one;
		protected Transform _transform;

		protected static readonly int zWrite = Shader.PropertyToID("_ZWrite");
		protected static readonly int cull = Shader.PropertyToID("_Cull");
		protected static readonly int dstBlend = Shader.PropertyToID("_DstBlend");
		protected static readonly int srcBlend = Shader.PropertyToID("_SrcBlend");

		protected const float _minPermittedLineWidth = 0.01f;
		#endregion

		#region Unity Event Functions
		protected void OnDrawGizmos()
		{
			if (gridLines == null)
				return;
			Axes ax = gridAxes;

			if (showGridCornersInEditor && gridCorners != null)
			{
				Gizmos.color = gridCornerEditorColor;
				foreach (Vector3 corner in gridCorners)
				{
					Gizmos.DrawSphere(corner, gridCornerRadius);
				}
				Gizmos.color = Color.white;
			}

			if (showGridLinesInEditor)
			{
				if (customizeLineWidth && customLineWidth > _minPermittedLineWidth)
				{
					Gizmos.color = gridLineEditorColor;
					for (int i = 0; i < gridLines.Length - 3; i += 4)
					{
						Vector3[] vertices = new Vector3[4];
						vertices[0] = gridLines[i].WhereComponent(ax, depth: gridLines[i].DepthComponent(ax) + gridLineEditorDepthOffset);
						vertices[1] = gridLines[i+1].WhereComponent(ax, depth: gridLines[i+1].DepthComponent(ax) + gridLineEditorDepthOffset);
						vertices[2] = gridLines[i+2].WhereComponent(ax, depth: gridLines[i+2].DepthComponent(ax) + gridLineEditorDepthOffset);
						vertices[3] = gridLines[i+3].WhereComponent(ax, depth: gridLines[i+3].DepthComponent(ax) + gridLineEditorDepthOffset);
						
						int[] triangles = new int[] 
						{
							2, 1, 0,
							0, 3, 2
						};
						
						Vector3[] normals = new Vector3[]
						{
							Vector3.up, 
							Vector3.up, 
							Vector3.up, 
							Vector3.up
						};

						Mesh mesh = new Mesh() {vertices = vertices, triangles = triangles, normals = normals};
						Gizmos.DrawMesh(mesh);
					}
					Gizmos.color = Color.white;
					return;
				}
				
				Gizmos.color = gridLineEditorColor;
				for (int i = 0; i < gridLines.Length; i += 2)
				{
					Gizmos.DrawLine(gridLines[i].WhereComponent(ax, depth: gridLines[i].DepthComponent(ax) + gridLineEditorDepthOffset),
						gridLines[i+1].WhereComponent(ax, depth: gridLines[i+1].DepthComponent(ax) + gridLineEditorDepthOffset));
				}
				Gizmos.color = Color.white;
			}
		}

		protected void OnRenderObject()
		{
			if (gridLines == null || !showGridLines)
				return;
			Axes ax = gridAxes;
            
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			Material mat = new Material(shader) {hideFlags = HideFlags.HideAndDontSave};
			mat.SetInt(srcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			mat.SetInt(dstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			// Turn backface culling off
			mat.SetInt(cull, (int)UnityEngine.Rendering.CullMode.Off);
			// Turn off depth writes
			mat.SetInt(zWrite, 0);

			if (customizeLineWidth && customLineWidth > _minPermittedLineWidth)
			{
				for (int i = 0; i < gridLines.Length; i += 4)
				{
					mat.SetPass(0);
					GL.PushMatrix();

					GL.Begin(GL.QUADS);
					GL.Color(gridLineColor);
					GL.Vertex(gridLines[i].WhereComponent(ax, depth: gridLines[i].DepthComponent(ax) + gridLineDepthOffset));
					GL.Vertex(gridLines[i+1].WhereComponent(ax, depth: gridLines[i+1].DepthComponent(ax) + gridLineDepthOffset));
					GL.Vertex(gridLines[i+2].WhereComponent(ax, depth: gridLines[i+2].DepthComponent(ax) + gridLineDepthOffset));
					GL.Vertex(gridLines[i+3].WhereComponent(ax, depth: gridLines[i+3].DepthComponent(ax) + gridLineDepthOffset));
					GL.End();
                
					GL.PopMatrix();
				}
				return;
			}
			for (int i = 0; i < gridLines.Length; i += 2)
			{
				mat.SetPass(0);
				GL.PushMatrix();

				GL.Begin(GL.LINES);
				GL.Color(gridLineColor);
				GL.Vertex(gridLines[i].WhereComponent(ax, depth: gridLines[i].DepthComponent(ax) + gridLineDepthOffset));
				GL.Vertex(gridLines[i+1].WhereComponent(ax, depth: gridLines[i+1].DepthComponent(ax) + gridLineDepthOffset));
				GL.End();
                
				GL.PopMatrix();
			}
		}
		#endregion
        
		#region Grid Generation
		public void CalculateGrid()
		{
			if (_transform == null)
			{
				_transform = transform;
			}
			Axes ax = gridAxes;
			
			_gridPointScale.WidthComponent(ax) = scaleWidth;
			_gridPointScale.DepthComponent(ax) = scaleDepth;
			_gridPointScale.LengthComponent(ax) = scaleLength;

			ResetGrid();
            
			if (gridPointLength == 0 || gridPointWidth == 0)
				return;
            
			CalculateBottomLeft(out bottomLeftCoordinate, out Vector3 bottomLeftGridPointCoordinate);

			// Calculates grid points and instantiates the grid point objects (if enabled) so the top surface is at the same depth level as this.transform.position.DepthComponentVal(ax)
			MeshFilter prefabMeshFilter;
			Vector3 boundSize = customGridPointObjects && customObjectPrefab != null && (prefabMeshFilter = customObjectPrefab.GetComponent<MeshFilter>()) != null  ? prefabMeshFilter.sharedMesh.bounds.size : Vector3.one;
			Quaternion objectRotation = CalculateObjectRotation();

			int index = 0;
			for (int i = 0; i < gridPointLength; i++)
			{
				for (int j = 0; j < gridPointWidth; j++)
				{
					gridPoints[index].WidthComponent(ax) = bottomLeftGridPointCoordinate.WidthComponentVal(ax) + _gridPointScale.WidthComponentVal(ax) * j;
					gridPoints[index].DepthComponent(ax) = bottomLeftGridPointCoordinate.DepthComponentVal(ax);
					gridPoints[index].LengthComponent(ax) = bottomLeftGridPointCoordinate.LengthComponentVal(ax) + _gridPointScale.LengthComponentVal(ax) * i;

					if (instantiateGridPointObjects)
					{
						if (customGridPointObjects && customObjectPrefab != null)
						{
							if(customObjectPrefab != default)
								gridPointObjects[index] = Instantiate(customObjectPrefab);
						}
						else
						{
							gridPointObjects[index] = GameObject.CreatePrimitive(PrimitiveType.Cube);
						}

						Vector3 objectScale = new Vector3(_gridPointScale.WidthComponentVal(ax) / boundSize.x, _gridPointScale.DepthComponentVal(ax), _gridPointScale.LengthComponentVal(ax) / boundSize.z);
						Vector3 objectPos = gridPoints[index];
						if (gridPointObjectSnapping == ObjectSnapping.MeshBoundsBottomToGrid ||
						    gridPointObjectSnapping == ObjectSnapping.MeshBoundsTopToGrid)
						{
							objectPos.DepthComponent(ax) += gridPointObjectSnapping == ObjectSnapping.MeshBoundsBottomToGrid ? (boundSize.y * objectScale.y) / 2f : -((boundSize.y * objectScale.y) / 2f);
						}
						gridPointObjects[index].transform.position = objectPos;
						gridPointObjects[index].name = $"Grid Point Object {index}";
                    
						gridPointObjects[index].transform.parent = _transform;
						gridPointObjects[index].transform.rotation = objectRotation;
						gridPointObjects[index].transform.localScale = objectScale;

						MeshRenderer currentObjectsMeshRenderer;
						if ((currentObjectsMeshRenderer = gridPointObjects[index].GetComponent<MeshRenderer>()) != null)
						{
							if (defaultGridPointMaterial == null)
							{
								defaultGridPointMaterial = currentObjectsMeshRenderer.sharedMaterial;
							}
							currentObjectsMeshRenderer.material = defaultGridPointMaterial;
						}
					}

					index++;
				}
			}
			
			if(calculateGridLines)
				CalculateGridLineVertices();
		}
		
		/// <summary>
		/// Calculates all vertices (2 or 4 per line depending on if a custom width is specified) for each line and saves them into gridLines
		/// </summary>
		public void CalculateGridLineVertices()
		{
			gridLines = null;
			gridCorners = null;
			
			if (!calculateGridLines)
				return;
			
			int lengthLinesAmount = gridPointLength + 1, // Amount of lines parallel to the length axis
				widthLinesAmount = gridPointWidth + 1; // Amount of lines parallel to the width axis

			CalculateCorners();

			if (customizeLineWidth && customLineWidth > _minPermittedLineWidth)
			{
				GridLinesCustomWidth(lengthLinesAmount, widthLinesAmount);
			}
			else
			{
				GridLines(lengthLinesAmount, widthLinesAmount);
			}
		}
		#endregion
        
		#region Grid Calculation Helper Methods
		/// <summary>
		/// Initializes Bottom Left Variables
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		protected void CalculateBottomLeft(out Vector3 bottomLeftCoordinate, out Vector3 bottomLeftGridPointCoordinate)
		{
			// Bottom left coordinate when looking straight at the grid
			// XY = -X, -Y corner
			// XZ = -X, -Z corner
			// YZ = -Y, -Z corner

			Vector3 pos = _transform.position;
			Axes ax = gridAxes;
			
			bottomLeftCoordinate = new Vector3();
			bottomLeftGridPointCoordinate = new Vector3();
            
			bottomLeftCoordinate.DepthComponent(ax) = pos.DepthComponentVal(ax);
			bottomLeftGridPointCoordinate.DepthComponent(ax) = bottomLeftCoordinate.DepthComponentVal(ax);
			
			if (gridPointWidth % 2 == 0) // if gridPointAmountX is even
			{
				bottomLeftCoordinate.WidthComponent(ax) = pos.WidthComponentVal(ax) - (gridPointWidth / 2f) * _gridPointScale.WidthComponentVal(ax);
				bottomLeftGridPointCoordinate.WidthComponent(ax) = bottomLeftCoordinate.WidthComponentVal(ax) + _gridPointScale.WidthComponentVal(ax) / 2f;
			}
			else
			{
				bottomLeftGridPointCoordinate.WidthComponent(ax) = pos.WidthComponentVal(ax) - ((gridPointWidth - 1) / 2f) * _gridPointScale.WidthComponentVal(ax);
				bottomLeftCoordinate.WidthComponent(ax) = bottomLeftGridPointCoordinate.WidthComponentVal(ax) - _gridPointScale.WidthComponentVal(ax) / 2f;
			}
            
			if (gridPointLength % 2 == 0) // if gridPointLength is even
			{
				bottomLeftCoordinate.LengthComponent(ax) = _transform.position.LengthComponentVal(ax) - (gridPointLength / 2f) * _gridPointScale.LengthComponentVal(ax);
				bottomLeftGridPointCoordinate.LengthComponent(ax) = bottomLeftCoordinate.LengthComponentVal(ax) + _gridPointScale.LengthComponentVal(ax) / 2f;
			}
			else
			{
				bottomLeftGridPointCoordinate.LengthComponent(ax) = _transform.position.LengthComponentVal(ax) - ((gridPointLength - 1) / 2f) * _gridPointScale.LengthComponentVal(ax);
				bottomLeftCoordinate.LengthComponent(ax) = bottomLeftGridPointCoordinate.LengthComponentVal(ax) - _gridPointScale.LengthComponentVal(ax) / 2f;
			}
		}

		/// <summary>
		/// Makes sure the width/height is not negative, destroys all grid point objects and resets relevant arrays;
		/// </summary>
		protected void ResetGrid()
		{
			if (gridPointWidth < 0)
			{
				gridPointWidth = Math.Abs(gridPointWidth);
			}

			if (gridPointLength < 0)
			{
				gridPointLength = Math.Abs(gridPointLength);
			}
            
			if (_transform.childCount > 0)
			{
				while (_transform.childCount > 0)
				{
					DestroyImmediate(_transform.GetChild(0).gameObject);
				}
			}

			gridPoints = new Vector3[gridPointWidth * gridPointLength];
			
			if (instantiateGridPointObjects)
				gridPointObjects = new GameObject[gridPointWidth * gridPointLength];
			else
				gridPointObjects = new GameObject[0];
		}

		/// <summary>
		/// Calculates grid line vertices (where each line is actually a quad) with the specified width (customizable in inspector)
		/// </summary>
		protected void GridLinesCustomWidth(int lengthLinesAmount, int widthLinesAmount)
		{
			Axes ax = gridAxes;
			gridLines = new Vector3[(lengthLinesAmount + widthLinesAmount) * 4];
			
			// Lines parallel to width axis
			int lineIndex = 0;
			for (int i = 0; i < lengthLinesAmount * 4; i += 4)
			{
				float lengthAxisCoordinate = gridCorners[(int)GridCorners.BottomLeft].LengthComponentVal(ax) + _gridPointScale.LengthComponentVal(ax) * lineIndex;

				Vector3 firstVertex = gridCorners[(int) GridCorners.BottomLeft];
				Vector3 secondVertex = gridCorners[(int) GridCorners.BottomRight];
				Vector3 thirdVertex = gridCorners[(int) GridCorners.BottomRight];
				Vector3 fourthVertex = gridCorners[(int) GridCorners.BottomLeft];
				firstVertex.LengthComponent(ax) = secondVertex.LengthComponent(ax) = lengthAxisCoordinate - customLineWidth / 2f;
				fourthVertex.LengthComponent(ax) = thirdVertex.LengthComponent(ax) = lengthAxisCoordinate + customLineWidth / 2f;

				firstVertex.WidthComponent(ax) -= customLineWidth / 2f;
				fourthVertex.WidthComponent(ax) -= customLineWidth / 2f;
				secondVertex.WidthComponent(ax) += customLineWidth / 2f;
				thirdVertex.WidthComponent(ax) += customLineWidth / 2f;

				gridLines[i] = firstVertex;
				gridLines[i + 1] = secondVertex;
				gridLines[i + 2] = thirdVertex;
				gridLines[i + 3] = fourthVertex;
				lineIndex++;
			}
            
			// Lines parallel to length axis
			lineIndex = 0;
			for (int i = lengthLinesAmount * 4; i < gridLines.Length - 3; i += 4)
			{
				float widthAxisCoordinate = gridCorners[(int)GridCorners.BottomLeft].WidthComponentVal(ax) + _gridPointScale.WidthComponentVal(ax) * lineIndex;
                
				Vector3 firstVertex = gridCorners[(int) GridCorners.BottomLeft];
				Vector3 secondVertex = gridCorners[(int) GridCorners.TopLeft];
				Vector3 thirdVertex = gridCorners[(int) GridCorners.TopLeft];
				Vector3 fourthVertex = gridCorners[(int) GridCorners.BottomLeft];
				firstVertex.WidthComponent(ax) = secondVertex.WidthComponent(ax) = widthAxisCoordinate - customLineWidth / 2f;
				fourthVertex.WidthComponent(ax) = thirdVertex.WidthComponent(ax) = widthAxisCoordinate + customLineWidth / 2f;
				
				firstVertex.LengthComponent(ax) -= customLineWidth / 2f;
				fourthVertex.LengthComponent(ax) -= customLineWidth / 2f;
				secondVertex.LengthComponent(ax) += customLineWidth / 2f;
				thirdVertex.LengthComponent(ax) += customLineWidth / 2f;

				gridLines[i + 3] = firstVertex;
				gridLines[i + 2] = secondVertex;
				gridLines[i + 1] = thirdVertex;
				gridLines[i] = fourthVertex;
				lineIndex++;
			}
		}

		/// <summary>
		/// Calculates all vertices (2 per line) for each line and saves them into gridLines
		/// </summary>
		protected void GridLines(int lengthLinesAmount, int widthLinesAmount)
		{
			Axes ax = gridAxes;
			gridLines = new Vector3[(lengthLinesAmount + widthLinesAmount) * 2];
			
			// Lines parallel to width axis
			int lineIndex = 0;
			for (int i = 0; i < lengthLinesAmount * 2; i += 2)
			{
				float lengthAxisCoordinate = gridCorners[(int)GridCorners.BottomLeft].LengthComponentVal(ax) + _gridPointScale.LengthComponentVal(ax) * lineIndex;

				Vector3 firstVertex = gridCorners[(int) GridCorners.BottomLeft];
				Vector3 secondVertex = gridCorners[(int) GridCorners.BottomRight];
				firstVertex.LengthComponent(ax) = secondVertex.LengthComponent(ax) = lengthAxisCoordinate;

				gridLines[i] = firstVertex;
				gridLines[i + 1] = secondVertex;
				lineIndex++;
			}
            
			// Lines parallel to length axis
			lineIndex = 0;
			for (int i = lengthLinesAmount * 2; i < gridLines.Length - 1; i += 2)
			{
				float widthAxisCoordinate = gridCorners[(int)GridCorners.BottomLeft].WidthComponentVal(ax) + _gridPointScale.WidthComponentVal(ax) * lineIndex;
                
				Vector3 firstVertex = gridCorners[(int) GridCorners.BottomLeft];
				Vector3 secondVertex = gridCorners[(int) GridCorners.TopLeft];
				firstVertex.WidthComponent(ax) = secondVertex.WidthComponent(ax) = widthAxisCoordinate;

				gridLines[i] = firstVertex;
				gridLines[i + 1] = secondVertex;
				lineIndex++;
			}
		}
		
		/// <summary>
		/// Calculates the corners of the grid and saves them to the array gridCorners;
		/// </summary>
		protected void CalculateCorners()
		{
			Axes ax = gridAxes;
			
			float gridDistanceWidth = gridPointWidth * _gridPointScale.WidthComponentVal(ax),
				gridDistanceLength = gridPointLength * _gridPointScale.LengthComponentVal(ax);
            
			// Declaring corner vertices of the grid
			gridCorners = new Vector3[4];
			gridCorners[(int)GridCorners.BottomLeft] = bottomLeftCoordinate;
			gridCorners[(int)GridCorners.BottomRight] = bottomLeftCoordinate.WhereComponent(ax, width: bottomLeftCoordinate.WidthComponentVal(ax) + gridDistanceWidth);
			gridCorners[(int)GridCorners.TopLeft] = bottomLeftCoordinate.WhereComponent(ax, length: bottomLeftCoordinate.LengthComponentVal(ax) + gridDistanceLength);
			gridCorners[(int)GridCorners.TopRight] = bottomLeftCoordinate.WhereComponent(ax, width: bottomLeftCoordinate.WidthComponentVal(ax) + gridDistanceWidth, length: bottomLeftCoordinate.LengthComponentVal(ax) + gridDistanceLength);
		}
		#endregion

		#region Utility Methods
		/// <summary>
		/// Takes the index of a grid point and returns the index of an adjacent grid point in a given direction. Will return -1 if there is no valid adjacent grid point in the specified direction. All values are in world space.
		/// </summary>
		/// <returns>Index of the adjacent grid point</returns>
		/// <exception cref="NullReferenceException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public int GetAdjacentIndex(int index, Direction direction)
		{
			Axes ax = gridAxes;

			switch (direction)
			{
				case Direction.NegativeX:
					if (ax != Axes.ZY) return NegativeWidth();
					Debug.Log("Invalid direction, grid is generated in the " + ax + "axes");
					return -1;

				case Direction.NegativeY:
					if (ax != Axes.XZ) return NegativeLength();
					Debug.Log("Invalid direction, grid is generated in the " + ax + "axes");
					return -1;

				case Direction.NegativeZ:
					switch (ax)
					{
						case Axes.XZ:
							return NegativeLength();
						case Axes.ZY:
							return NegativeWidth();
						default:
							Debug.Log("Invalid direction, grid is generated in the " + ax + "axes");
							return -1;
					}

				case Direction.PositiveX:
					if (ax != Axes.ZY) return PositiveWidth();
					Debug.Log("Invalid direction, grid is generated in the " + ax + "axes");
					return -1;
				
				case Direction.PositiveY:
					if (ax != Axes.XZ) return PositiveLength();
					Debug.Log("Invalid direction, grid is generated in the " + ax + "axes");
					return -1;

				case Direction.PositiveZ:
					switch (ax)
					{
						case Axes.XZ:
							return PositiveLength();
						case Axes.ZY:
							return PositiveWidth();
						default:
							Debug.Log("Invalid direction, grid is generated in the " + ax + "axes");
							return -1;
					}
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, $"The method GetAdjacentPoint does not contain a reference to the value {direction} in enum \"Direction\"");
			}

			int PositiveWidth()
			{
				if ((index + 1) % gridPointWidth == 0)
					return AtEdge();
                    
				return index + 1;
			}

			int PositiveLength()
			{
				// Calculates the first index at the edge of positive length axis
				int firstIndexAtEdge = gridPointWidth > gridPointLength ? (gridPointWidth - 1) * gridPointLength : gridPointWidth * (gridPointLength - 1);

				if (index >= firstIndexAtEdge)
					return AtEdge();

				return index + gridPointWidth;
			}

			int NegativeWidth()
			{
				if (index % gridPointWidth == 0) // Checks if current point is on the edge so that there is no point toward negative x
					return AtEdge();
                    
				return index - 1;
			}

			int NegativeLength()
			{
				if (index < gridPointWidth)
					return AtEdge();

				return index - gridPointWidth;
			}
			
			int AtEdge()
			{
				Debug.Log($"Current point is at the edge, there is no grid point in the direction specified");
				return -1;
			}
		}
        
		/// <summary>
		/// Calculates the rotation an object should have if the up vector of the object should be perpendicular to the depth axis vector of the current grid
		/// </summary>
		public Quaternion CalculateObjectRotation()
		{
			_transform = transform;
			Axes ax = gridAxes;
			
			switch (ax)
			{
				case Axes.XY:
					return Quaternion.LookRotation(_transform.up, -_transform.forward);
				case Axes.XZ:
					return Quaternion.LookRotation(_transform.forward, _transform.up);
				case Axes.ZY:
					return Quaternion.LookRotation(-_transform.up, _transform.right);
				default:
					return Quaternion.identity;
			}
		}
		
		/// <summary>
		/// Takes the index of a point and a PointVisualizationMode enum to temporarily change the material of the point object. Use ClearPath() to reset all points material back to default.
		/// </summary>
		/// <param name="indexOfPoint">The index of the point object to change materials of</param>
		/// <param name="pointVisualizationMode">Which visualization mode to use (set material for each mode in inspector)</param>
		public void VisualizePoint(int indexOfPoint, PointVisualizationMode pointVisualizationMode)
		{
			ErrorCheck();
			if (!instantiateGridPointObjects)
			{
				Debug.LogWarning("Please enable instantiating of grid point objects to visualize points");
			}
			if (indexOfPoint == -1)
				return;
			var meshRenderer = gridPointObjects[indexOfPoint].GetComponent<MeshRenderer>();
			_visualizedPoints.Add(meshRenderer);
			int index = (int) pointVisualizationMode;
			meshRenderer.material = visualizedPointMaterials[index];
		}

		/// <summary>
		/// Resets the material (if changed) of all grid points objects back to default.
		/// </summary>
		public void ClearPath()
		{
			ErrorCheck();
			foreach (MeshRenderer meshRenderer in _visualizedPoints)
			{
				meshRenderer.material = defaultGridPointMaterial;
			}
            
			_visualizedPoints = new List<MeshRenderer>();
		}

		/// <summary>
		/// Takes an index of a point and checks if the point is occupied or not, will send out data about the occupied point if true to be able to access a (chosen) object of the occupier.
		/// </summary>
		/// <param name="indexOfPoint">The index of a point to check</param>
		/// <param name="occupiedPointData">The out data of the occupied point if method returns true, if it returns false data will be empty</param>
		public bool IsPointOccupied(int indexOfPoint, out OccupiedPointData occupiedPointData)
		{
			ErrorCheck();
			int occupiedPointIndex = occupiedGridPoints.FindIndex(a => a.occupiedPointIndex == indexOfPoint);
			bool pointIsOccupied = occupiedPointIndex != -1;
            
			if (pointIsOccupied)
			{
				occupiedPointData = occupiedGridPoints[occupiedPointIndex];
				return true;
			}
			else
			{
				occupiedPointData = new OccupiedPointData();
				return false;
			}
		}

		/// <summary>
		/// Marks the point with the same index as input currentIndex as occupied and sets the OccupiedPointData's occupier variable to a reference to an object. Will remove the previously occupied point if a point with the same occupier reference is found.
		/// </summary>
		/// <param name="reference"></param>
		/// <param name="currentIndex"></param>
		public void SetPointOccupied(object reference, int currentIndex)
		{
			int index = occupiedGridPoints.FindIndex(a => a.occupier == reference);
			if (index != -1)
				occupiedGridPoints.RemoveAt(index);
			occupiedGridPoints.Add(new OccupiedPointData(reference, currentIndex));
		}

		/// <summary>
		/// Given the reference of the occupier it will mark the currently occupied point as not occupied
		/// </summary>
		/// <param name="reference"></param>
		public void ResetPointOccupied(object reference)
		{
			int index = occupiedGridPoints.FindIndex(a => a.occupier == reference);
			if (index != -1)
				occupiedGridPoints.RemoveAt(index);
			else
				Debug.Log("That point is not occupied!");
		}
		#endregion
        
		#region Utility Helper Methods
		/// <summary>
		/// Makes sure the singleton is instantiated and makes sure the current variables are accurate with the currently generated grid.
		/// </summary>
		protected virtual void ErrorCheck()
		{
			// Checks to see if the amount of points in the inspector corresponds with the generated array, if not asks to regenerate grid
			if (gridPoints.Length != gridPointWidth * gridPointLength || gridPointWidth == 0 || gridPointLength == 0)
				throw new ArgumentOutOfRangeException(nameof(gridPoints), gridPoints, "Please regenerate the RPGGrid after changing amount of points (please note that the amount of points in any axis cannot be 0 when generating)");
		}
		#endregion
	}
}