﻿using UnityEngine;

namespace Deform.Deformers
{
	public class SquashAndStretchDeformer : DeformerComponent
	{
		[Tooltip ("You can also scale the axis' transform on the z axis to get the same effect.")]
		public float amount = 0f;
		public Transform axis;

		private Matrix4x4
			scaleSpace,
			meshSpace;

		// Calculations that don't need to be calculated for each vertex and can be cached
		private float finalAmount;
		private float oneOverFinalAmount;

		// This is called on the main thread before the vertex data is modified.
		// It's the best place to cache any info you only need to calculate once or
		// that you'll need when on another thread.
		public override void PreModify ()
		{
			base.PreModify ();

			// Calculate the amount to squash/stretch
			finalAmount = (amount + 1f) + axis.localScale.z;
			if (finalAmount == 0f)
				finalAmount = Mathf.Epsilon;
			oneOverFinalAmount = 1f / finalAmount;

			if (axis == null)
			{
				axis = new GameObject ("Axis").transform;
				axis.transform.SetParent (transform);
			}

			var rotation = Quaternion.Euler (-axis.eulerAngles.x, -axis.eulerAngles.y, axis.eulerAngles.z);
			scaleSpace = Matrix4x4.TRS (Vector3.zero, rotation, Vector3.one);
			meshSpace = scaleSpace.inverse;
		}

		// This method runs on another thread, hence the TransformData instead of Transform
		// Also, think of a Chunk as a mesh
		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds bounds)
		{
			// Here's where the vertices are actually being modified.
			for (var vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				var position = chunk.vertexData[vertexIndex].position;

				var positionOnAxis = scaleSpace.MultiplyPoint3x4 (position);

				positionOnAxis.x *= oneOverFinalAmount;
				positionOnAxis.y *= oneOverFinalAmount;
				positionOnAxis.z *= finalAmount;

				chunk.vertexData[vertexIndex].position = meshSpace.MultiplyPoint3x4 (positionOnAxis);
			}

			// The chunk that is returned is applied to the mesh on the main thread
			return chunk;
		}
	}
}