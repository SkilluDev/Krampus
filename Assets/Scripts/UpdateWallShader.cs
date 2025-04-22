using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class UpdateWallShader : KrampusBehaviour {
	[SerializeField] private Vector2 m_wallFade = new Vector2(-1, 1);
	[SerializeField] private float m_smoothTime = 3f;
	[SerializeField] private float m_offsetDistance = 10f;
	[SerializeField] private float m_fadeOriginRayLength = 5f;
	private Vector3 m_fadeOriginPosition;
	private void Update() {
		float distance = -m_offsetDistance;
		var aboveKrampus = Kramp.Kamera.Matrix.MultiplyVector(Vector3.forward);

		if (Physics.Raycast(transform.position, aboveKrampus, out var hit, m_fadeOriginRayLength, 1 << 6)) {
			distance = Mathf.Lerp(-m_offsetDistance, -m_offsetDistance - (m_fadeOriginRayLength - hit.distance), m_smoothTime * Time.deltaTime);
		}
		m_fadeOriginPosition = distance * aboveKrampus;


		Shader.SetGlobalVector("_KrampusPosition", transform.position + m_fadeOriginPosition);
		Shader.SetGlobalVector("_WallFadeSetting", m_wallFade);
	}
}
