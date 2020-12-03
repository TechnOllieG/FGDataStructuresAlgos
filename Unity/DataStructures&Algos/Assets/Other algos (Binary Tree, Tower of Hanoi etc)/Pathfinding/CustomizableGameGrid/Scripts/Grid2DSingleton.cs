using System;
using UnityEngine;

namespace TechnOllieG.CustomizableGameGrid
{
	public class Grid2DSingleton : Grid2D
	{
		public static Grid2D Instance { get; private set; }
		
		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
		}
        
		// Since reset is called when a component is added, we can check to see if any other RPGGrid scripts exist
		private void Reset()
		{
			if (Instance != null && Instance != this)
			{
				Debug.LogWarning("There can only be one RPGGrid script in the scene");
				DestroyImmediate(this);
			}
		}

		protected override void ErrorCheck()
		{
			if(Instance == null)
				throw new NullReferenceException("RPGGrid is not instantiated in the scene, please add the script to an object");
		}
	}
}
