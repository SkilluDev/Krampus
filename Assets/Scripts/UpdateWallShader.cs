using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class UpdateWallShader : KrampusBehaviour {
	[SerializeField] private Vector2 m_wallFade = new Vector2(-5, 5);
	[SerializeField] private float m_smoothTime = 5f;
	[SerializeField] private float m_offsetDistance = 5f;
	[SerializeField] private float m_fadeOriginRayLength = 10f;
	private float m_distance = 0f;
	private Vector3 m_fadeOriginPosition;
	private void Update() {
		var aboveKrampus = Kramp.Kamera.Matrix.MultiplyVector(Vector3.forward);

		if (Physics.Raycast(transform.position, aboveKrampus, out var hit, m_fadeOriginRayLength, 1 << 6)) {
			m_distance = Mathf.Lerp(m_distance, hit.distance - m_offsetDistance, m_smoothTime * Time.deltaTime);
		} else {
			m_distance = Mathf.Lerp(m_distance, m_offsetDistance, m_smoothTime * Time.deltaTime);
		}
		m_fadeOriginPosition = m_distance * aboveKrampus;


		Shader.SetGlobalVector("_KrampusPosition", transform.position + m_fadeOriginPosition);
		Shader.SetGlobalVector("_WallFadeSetting", m_wallFade);
	}
}
