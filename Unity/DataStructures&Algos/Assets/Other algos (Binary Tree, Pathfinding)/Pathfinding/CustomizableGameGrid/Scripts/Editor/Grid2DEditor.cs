using System;
using UnityEditor;
using UnityEngine;
using PointVisualizationMode = TechnOllieG.CustomizableGameGrid.Grid2D.PointVisualizationMode;
using Axes = TechnOllieG.CustomizableGameGrid.Grid2D.Axes;
using ObjectSnapping = TechnOllieG.CustomizableGameGrid.Grid2D.ObjectSnapping;

namespace TechnOllieG.CustomizableGameGrid
{
	[CustomEditor(typeof(Grid2D))]
	public class Grid2DEditor : Editor
	{
		private bool _showVisualizePointMaterials = false;
		private bool _showDebugOptions = false;

		private bool _showGridPoints = false;
		private bool _showGridPointObjects = false;
		private bool _dropdownGridLines = false;
		private bool _dropdownGridCorners = false;
        
		private SerializedProperty _defaultGridPointMaterial;
		private SerializedProperty _visualizedPointMaterials;
		private SerializedProperty _gridAxes;
		private SerializedProperty _gridPointWidth;
		private SerializedProperty _gridPointLength;
		private SerializedProperty _scaleWidth;
		private SerializedProperty _scaleDepth;
		private SerializedProperty _scaleLength;
        
		private SerializedProperty _instantiateGridPointObjects;
		private SerializedProperty _gridPointObjectSnapping;
		private SerializedProperty _customGridPointObjects;
		private SerializedProperty _customObjectPrefab;
        
		private SerializedProperty _calculateGridLines;

		private SerializedProperty _showGridLines;
		private SerializedProperty _gridLineColor;
		private SerializedProperty _gridLineDepthOffset;
		private SerializedProperty _customizeLineWidth;
		private SerializedProperty _customLineWidth;
        
		private SerializedProperty _showGridLinesInEditor;
		private SerializedProperty _gridLineEditorColor;
		private SerializedProperty _gridLineEditorDepthOffset;
        
		private SerializedProperty _showGridCornersInEditor;
		private SerializedProperty _gridCornerEditorColor;
		private SerializedProperty _gridCornerRadius;

		private void OnEnable()
		{
			_defaultGridPointMaterial = serializedObject.FindProperty("defaultGridPointMaterial");
			_visualizedPointMaterials = serializedObject.FindProperty("visualizedPointMaterials");
			_gridAxes = serializedObject.FindProperty("gridAxes");
			_gridPointWidth = serializedObject.FindProperty("gridPointWidth");
			_gridPointLength = serializedObject.FindProperty("gridPointLength");
			_scaleWidth = serializedObject.FindProperty("scaleWidth");
			_scaleDepth = serializedObject.FindProperty("scaleDepth");
			_scaleLength = serializedObject.FindProperty("scaleLength");
            
			_instantiateGridPointObjects = serializedObject.FindProperty("instantiateGridPointObjects");
			_gridPointObjectSnapping = serializedObject.FindProperty("gridPointObjectSnapping");
			_customGridPointObjects = serializedObject.FindProperty("customGridPointObjects");
			_customObjectPrefab = serializedObject.FindProperty("customObjectPrefab");

			_calculateGridLines = serializedObject.FindProperty("calculateGridLines");
            
			_showGridLines = serializedObject.FindProperty("showGridLines");
			_gridLineColor = serializedObject.FindProperty("gridLineColor");
			_gridLineDepthOffset = serializedObject.FindProperty("gridLineDepthOffset");
			_customizeLineWidth = serializedObject.FindProperty("customizeLineWidth");
			_customLineWidth = serializedObject.FindProperty("customLineWidth");

			_showGridLinesInEditor = serializedObject.FindProperty("showGridLinesInEditor");
			_gridLineEditorColor = serializedObject.FindProperty("gridLineEditorColor");
			_gridLineEditorDepthOffset = serializedObject.FindProperty("gridLineEditorDepthOffset");
            
			_showGridCornersInEditor = serializedObject.FindProperty("showGridCornersInEditor");
			_gridCornerEditorColor = serializedObject.FindProperty("gridCornerEditorColor");
			_gridCornerRadius = serializedObject.FindProperty("gridCornerRadius");
		}

		public override void OnInspectorGUI()
		{
			float oldLabelWidth = EditorGUIUtility.labelWidth;
			
			const int minLabelWidth = 100;
			const int maxLabelWidth = 250;
			const int marginInsideFoldout = 20;
			const float elementTextWidth = 150f;

			var script = (Grid2D) target;

			const string x = "X";
			const string y = "Y";
			const string z = "Z";
            
			string widthAxis;
			string depthAxis;
			string lengthAxis;
            
			switch((Axes)_gridAxes.enumValueIndex)
			{
				case Axes.XY:
					widthAxis = x;
					depthAxis = z;
					lengthAxis = y;
					break;
				case Axes.XZ:
					widthAxis = x;
					depthAxis = y;
					lengthAxis = z;
					break;
				case Axes.ZY:
					widthAxis = z;
					depthAxis = x;
					lengthAxis = y;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			
			GUIStyle titleStyle = new GUIStyle() {richText = true, alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold};

			EditorGUILayout.LabelField("<color=white>General</color>", titleStyle);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent("Axes", "Which axes to generate the grid on"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
			_gridAxes.enumValueIndex = (int) (Axes) EditorGUILayout.EnumPopup((Axes) Enum.ToObject(typeof(Axes), _gridAxes.enumValueIndex));
			EditorGUILayout.EndHorizontal();
            
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent("Grid Dimensions", "Amount of grid points along selected axes"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
			EditorGUIUtility.labelWidth = 10;
			_gridPointWidth.intValue = EditorGUILayout.IntField(widthAxis, _gridPointWidth.intValue);
			_gridPointLength.intValue = EditorGUILayout.IntField(lengthAxis, _gridPointLength.intValue);
			EditorGUIUtility.labelWidth = oldLabelWidth;
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 0f;
			EditorGUILayout.LabelField(new GUIContent("Grid Point Scale", "The scale of each grid point in units, if grid point objects are generated they will also have the same scale"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
			EditorGUIUtility.labelWidth = 38f;
			_scaleWidth.floatValue = EditorGUILayout.FloatField(new GUIContent("Width", $"the width component of the grid point scale, in this case, the {widthAxis.ToLower()} component"), _scaleWidth.floatValue);
			EditorGUIUtility.labelWidth = 40f;
			_scaleDepth.floatValue = EditorGUILayout.FloatField(new GUIContent("Depth", $"the depth component of the grid point scale, in this case, the {depthAxis.ToLower()} component"), _scaleDepth.floatValue);
			EditorGUIUtility.labelWidth = 45f;
			_scaleLength.floatValue = EditorGUILayout.FloatField(new GUIContent("Length", $"the length component of the grid point scale, in this case, the {lengthAxis.ToLower()} component"), _scaleLength.floatValue);
			EditorGUIUtility.labelWidth = oldLabelWidth;
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("<color=white>Grid Point Objects</color>", titleStyle);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent("Instantiate Grid Point Objects", "If true, will instantiate objects on every grid point which you can change the material of, using methods to visualize different things"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
			_instantiateGridPointObjects.boolValue = EditorGUILayout.Toggle(_instantiateGridPointObjects.boolValue);
			EditorGUILayout.EndHorizontal();
			
			if (serializedObject.hasModifiedProperties)
			{
				serializedObject.ApplyModifiedProperties();
				script.CalculateGrid();
			}

			if (_instantiateGridPointObjects.boolValue)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(new GUIContent("Grid Point Object Snapping", $"How the grid point objects should be aligned compared to the actual grid point (changes the objects {depthAxis.ToLower()} value)"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
				_gridPointObjectSnapping.enumValueIndex = (int) (ObjectSnapping) EditorGUILayout.EnumPopup((ObjectSnapping) Enum.ToObject(typeof(ObjectSnapping), _gridPointObjectSnapping.enumValueIndex));
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(new GUIContent("Custom Grid Point Objects", "If true, will instantiate a custom object on every grid point instead of cubes"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
				_customGridPointObjects.boolValue = EditorGUILayout.Toggle(_customGridPointObjects.boolValue);
				EditorGUILayout.EndHorizontal();

				if (_customGridPointObjects.boolValue)
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent("Custom Object Prefab", "The prefab that will be instantiated on every grid point"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
					_customObjectPrefab.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField(_customObjectPrefab.objectReferenceValue, typeof(GameObject), false);
					EditorGUILayout.EndHorizontal();
				}

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(new GUIContent("Default Grid Point Material", "The default material for all grid point objects, default will be used if null"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
				_defaultGridPointMaterial.objectReferenceValue = (Material)EditorGUILayout.ObjectField(_defaultGridPointMaterial.objectReferenceValue, typeof(Material), false);
				EditorGUILayout.EndHorizontal();

				if (serializedObject.hasModifiedProperties)
				{
					serializedObject.ApplyModifiedProperties();
					script.CalculateGrid();
				}
                
				EditorGUILayout.BeginHorizontal();
				_showVisualizePointMaterials = EditorGUILayout.Foldout(_showVisualizePointMaterials, new GUIContent("Visualized Point Materials", "The enum names of the materials which can be used to visualize the grid point objects, add more options by editing the enum in this script"));
				EditorGUILayout.EndHorizontal();
                
				if (_showVisualizePointMaterials)
				{
					_visualizedPointMaterials.arraySize = Enum.GetNames(typeof(PointVisualizationMode)).Length;
                    
					for (int i = 0; i < _visualizedPointMaterials.arraySize; i++)
					{
						SerializedProperty arrayElement = _visualizedPointMaterials.GetArrayElementAtIndex(i);
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
						EditorGUILayout.LabelField($"{(PointVisualizationMode)i}:", GUILayout.Width(elementTextWidth));
						arrayElement.objectReferenceValue = EditorGUILayout.ObjectField(arrayElement.objectReferenceValue, typeof(Material), true);
						EditorGUILayout.EndHorizontal();
					}
				}
            
				if (serializedObject.hasModifiedProperties)
				{
					serializedObject.ApplyModifiedProperties();
				}
			}

			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("<color=white>Grid Lines & Corners</color>", titleStyle);
            
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent("Calculate Grid Lines & Corners", "If false, it wont save the vertices for the grid lines/corners. It helps to minimize memory usage/processing time when calculating for bigger grids where line rendering is not intended"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
			_calculateGridLines.boolValue = EditorGUILayout.Toggle(_calculateGridLines.boolValue);
			EditorGUILayout.EndHorizontal();

			if (_calculateGridLines.boolValue)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(new GUIContent("Customize Line Width", "If the grid lines in play mode should have a custom width"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
				_customizeLineWidth.boolValue = EditorGUILayout.Toggle(_customizeLineWidth.boolValue);
				EditorGUILayout.EndHorizontal();

				if (_customizeLineWidth.boolValue)
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent($"Custom Line Width", $"The line width in units"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
					_customLineWidth.floatValue = EditorGUILayout.FloatField(_customLineWidth.floatValue);
					EditorGUILayout.EndHorizontal();
				}
			}

			if (serializedObject.hasModifiedProperties)
			{
				serializedObject.ApplyModifiedProperties();
				script.CalculateGridLineVertices();
			}

			if (_calculateGridLines.boolValue)
			{
				EditorGUILayout.Space();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(new GUIContent("Show Grid Lines in Playmode", "Whether or not to render grid lines in playmode/build"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
				_showGridLines.boolValue = EditorGUILayout.Toggle(_showGridLines.boolValue);
				EditorGUILayout.EndHorizontal();

				if (_showGridLines.boolValue)
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent("Grid Line Color", "What color to use to render the grid lines"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
					_gridLineColor.colorValue = EditorGUILayout.ColorField(_gridLineColor.colorValue);
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent($"Grid Line {depthAxis} Offset", $"By default the line renders at the same {depthAxis.ToLower()} level as the surface of the grid cubes, this offset can shift that value"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
					_gridLineDepthOffset.floatValue = EditorGUILayout.FloatField(_gridLineDepthOffset.floatValue);
					EditorGUILayout.EndHorizontal();
				}
			}

			EditorGUILayout.Space();
			
			_showDebugOptions = EditorGUILayout.Foldout(_showDebugOptions, "Debug");

			if (_showDebugOptions)
			{
				if (_calculateGridLines.boolValue)
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
					EditorGUILayout.LabelField(new GUIContent("Show Grid Lines In Editor", "Whether or not to render grid lines in editor"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
					_showGridLinesInEditor.boolValue = EditorGUILayout.Toggle(_showGridLinesInEditor.boolValue);
					EditorGUILayout.EndHorizontal();

					if (_showGridLinesInEditor.boolValue)
					{
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
						EditorGUILayout.LabelField(new GUIContent("Grid Line Editor Color", "What color to use to render the editor's grid lines"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
						_gridLineEditorColor.colorValue = EditorGUILayout.ColorField(_gridLineEditorColor.colorValue);
						EditorGUILayout.EndHorizontal();

						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
						EditorGUILayout.LabelField(new GUIContent($"Grid Line Editor {depthAxis} Offset", $"By default the line renders at the same {depthAxis.ToLower()} level as the surface of the grid cubes, this offset can shift that value"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
						_gridLineEditorDepthOffset.floatValue = EditorGUILayout.FloatField(_gridLineEditorDepthOffset.floatValue);
						EditorGUILayout.EndHorizontal();

						EditorGUILayout.Space();
					}

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
					EditorGUILayout.LabelField(new GUIContent("Show Grid Corners In Editor", "Whether or not to render spheres at grid corners in editor"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
					_showGridCornersInEditor.boolValue = EditorGUILayout.Toggle(_showGridCornersInEditor.boolValue);
					EditorGUILayout.EndHorizontal();
				}

				if (_showGridCornersInEditor.boolValue)
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
					EditorGUILayout.LabelField(new GUIContent("Grid Corner Editor Color", "What color to use to render the editor's grid corner spheres"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
					_gridCornerEditorColor.colorValue = EditorGUILayout.ColorField(_gridCornerEditorColor.colorValue);
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
					EditorGUILayout.LabelField(new GUIContent("Grid Corner Radius", "The radius of the rendered corner spheres"), GUILayout.MinWidth(minLabelWidth), GUILayout.MaxWidth(maxLabelWidth));
					_gridCornerRadius.floatValue = EditorGUILayout.FloatField(_gridCornerRadius.floatValue);
					EditorGUILayout.EndHorizontal();
				}

				EditorGUILayout.Space();
                
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
				_showGridPoints = EditorGUILayout.Foldout(_showGridPoints, "Grid Points");
				EditorGUILayout.EndHorizontal();
                
				if (_showGridPoints)
				{
					Vector3[] gridPoints = script.gridPoints;
					GUIStyle richTextStyle = new GUIStyle() {richText = true};
                    
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
					EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
					if(gridPoints != null) EditorGUILayout.LabelField("Size of array: " + gridPoints.Length);
					else EditorGUILayout.LabelField("Size of array: 0");
					EditorGUILayout.EndHorizontal();

					if (gridPoints != null)
					{
						for (int i = 0; i < gridPoints.Length; i++)
						{
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
							EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
							EditorGUILayout.LabelField($"Index {i}:", GUILayout.Width(elementTextWidth));
							EditorGUILayout.LabelField($"<color=white><color=red>X</color> = {gridPoints[i].x}, <color=lime>Y</color> = {gridPoints[i].y}, <color=aqua>Z</color> = {gridPoints[i].z}</color>", richTextStyle);
							EditorGUILayout.EndHorizontal();
						}
					}
				}
				
				EditorGUILayout.Space();
				 
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
				_showGridPointObjects = EditorGUILayout.Foldout(_showGridPointObjects, "Grid Point Objects");
				EditorGUILayout.EndHorizontal();
                
				if (_showGridPointObjects)
				{
					GameObject[] gridPointObjects = script.gridPointObjects;

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
					EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
					if(gridPointObjects != null) EditorGUILayout.LabelField("Size of array: " + gridPointObjects.Length);
					else EditorGUILayout.LabelField("Size of array: 0");
					EditorGUILayout.EndHorizontal();

					if (gridPointObjects != null)
					{
						for (int i = 0; i < gridPointObjects.Length; i++)
						{
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
							EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
							EditorGUILayout.LabelField($"Index {i}:", GUILayout.Width(elementTextWidth));
							EditorGUILayout.ObjectField(gridPointObjects[i], typeof(GameObject), true);
							EditorGUILayout.EndHorizontal();
						}
					}
				}

				EditorGUILayout.Space();
                
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
				_dropdownGridLines = EditorGUILayout.Foldout(_dropdownGridLines, "Grid Line Vertices");
				EditorGUILayout.EndHorizontal();
                
				if (_dropdownGridLines)
				{
					Vector3[] gridLines = script.gridLines;
					GUIStyle richTextStyle = new GUIStyle() {richText = true};
                    
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
					EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
					if(gridLines != null) EditorGUILayout.LabelField("Size of array: " + gridLines.Length);
					else EditorGUILayout.LabelField("Size of array: 0");
					EditorGUILayout.EndHorizontal();

					if (gridLines != null)
					{
						for (int i = 0; i < gridLines.Length; i++)
						{
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
							EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
							EditorGUILayout.LabelField($"Index {i}:", GUILayout.Width(elementTextWidth));
							EditorGUILayout.LabelField($"<color=white><color=red>X</color> = {gridLines[i].x}, <color=lime>Y</color> = {gridLines[i].y}, <color=aqua>Z</color> = {gridLines[i].z}</color>", richTextStyle);
							EditorGUILayout.EndHorizontal();
						}
					}
				}
                
				EditorGUILayout.Space();
                
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
				_dropdownGridCorners = EditorGUILayout.Foldout(_dropdownGridCorners, "Grid Corners");
				EditorGUILayout.EndHorizontal();
                
				if (_dropdownGridCorners)
				{
					Vector3[] gridCorners = script.gridCorners;
					GUIStyle richTextStyle = new GUIStyle() {richText = true};
                    
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
					EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
					if(gridCorners != null) EditorGUILayout.LabelField("Size of array: " + gridCorners.Length);
					else EditorGUILayout.LabelField("Size of array: 0");
					EditorGUILayout.EndHorizontal();
                    
					
					if (gridCorners != null)
					{
						for (int i = 0; i < gridCorners.Length; i++)
						{
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
							EditorGUILayout.LabelField("", GUILayout.Width(marginInsideFoldout));
							EditorGUILayout.LabelField($"Index {i}:", GUILayout.Width(elementTextWidth));
							EditorGUILayout.LabelField($"<color=white><color=red>X</color> = {gridCorners[i].x}, <color=lime>Y</color> = {gridCorners[i].y}, <color=aqua>Z</color> = {gridCorners[i].z}</color>", richTextStyle);
							EditorGUILayout.EndHorizontal();
						}
					}
				}
			}
            
			if (serializedObject.hasModifiedProperties)
			{
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}