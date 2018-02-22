﻿using UnityEngine;

namespace DForm
{
	[RequireComponent (typeof (MeshFilter))]
	[ExecuteInEditMode]
	public class DFormManager : DFormManagerBase
	{
		public enum RefreshMode { Update, Pause, Stop }
		public RefreshMode refreshMode = RefreshMode.Update;

		[SerializeField, HideInInspector]
		private MeshFilter meshFilter;
		//[SerializeField, HideInInspector]
		[SerializeField]
		private DFormerComponent[] deformers;

		private void Awake ()
		{
			meshFilter = GetComponent<MeshFilter> ();
			ChangeTarget (meshFilter);
			UpdateDeformerReferences ();
		}

		private void Update ()
		{
			switch (refreshMode)
			{
				case RefreshMode.Update:
					DeformChunks ();
					ApplyChunksToTarget ();
					ResetChunks ();
					return;
				case RefreshMode.Pause:
					return;
				case RefreshMode.Stop:
					ResetChunks ();
					ApplyChunksToTarget ();
					return;
			}
		}

		public void UpdateDeformerReferences ()
		{
			deformers = GetComponents<DFormerComponent> ();
		}

		private void DeformChunks ()
		{
			// Call Pre Modify
			for (var deformerIndex = 0; deformerIndex < deformers.Length; deformerIndex++)
				deformers[deformerIndex].PreModify ();
			
			// Modify chunks
			for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
				for (var deformerIndex = 0; deformerIndex < deformers.Length; deformerIndex++)
					chunks[chunkIndex].vertexData = deformers[deformerIndex].Modify (chunks[chunkIndex].vertexData);

			// Call Post Modify
			for (var deformerIndex = 0; deformerIndex < deformers.Length; deformerIndex++)
				deformers[deformerIndex].PostModify ();
		}
	}
}