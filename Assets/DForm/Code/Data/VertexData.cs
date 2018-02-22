﻿using UnityEngine;

namespace Deform
{
	[System.Serializable]
	public struct VertexData
	{
		[SerializeField, HideInInspector]
		private Vector3 _basePosition;
		public Vector3 basePosition { get { return _basePosition; } }
		public Vector3 position;
		public Vector3 normal;
		public Vector4 tangent;

		public VertexData (Vector3 basePosition, Vector3 position, Vector3 normal, Vector4 tangent)
		{
			_basePosition = basePosition;
			this.position = position;
			this.normal = normal;
			this.tangent = tangent;
		}

		public VertexData (Vector3 position, Vector3 normal, Vector4 tangent)
		{
			_basePosition = position;
			this.position = position;
			this.normal = normal;
			this.tangent = tangent;
		}

		public void ResetPosition ()
		{
			position = basePosition;
		}
	}
}