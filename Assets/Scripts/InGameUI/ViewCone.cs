using System.Collections.Generic;
using KrampUtils;
using NaughtyAttributes;
using Roomgen;
using UnityEngine;

public class ViewCone : MonoBehaviour {
	[BoxGroup("Cone")] public float fov = 60;
	private float m_range;
	[BoxGroup("Cone")] public float runRange = 10;
	[BoxGroup("Cone")] public float sneakRange = 5;
	[BoxGroup("Cone")] public float unangledRange = 2;
	[BoxGroup("Cone")] public Transform trackedObject;
	[BoxGroup("Cone")] public LayerMask visionMask;

	[SerializeField] private MeshFilter m_meshFilter;
	[SerializeField] private MeshRenderer m_renderer;
	[SerializeField] private int m_additionalRaycastDelta = 1;

	[SerializeField] private float m_displaySmoothing = 4;

	private Vector3[] m_targetConePositions = new Vector3[4];
	private Vector3[] m_previousTargetConePositions = new Vector3[4];
	private Vector3[] m_conePositions = new Vector3[4];
	private int m_frameCounter = 0;

	private bool m_enabled = true;

	public bool Detect() {
		if (!m_enabled) return false;
		if (Vector3.SqrMagnitude(trackedObject.position - transform.position) > m_range * m_range) return false;

		if (Physics.Raycast(transform.position, trackedObject.transform.position - transform.position, out var hit, m_range, visionMask, QueryTriggerInteraction.Ignore)) {
			if (hit.collider.transform != trackedObject) return false;
			if (hit.distance <= unangledRange) return true;
			if (Vector3.Angle(transform.forward, (trackedObject.transform.position - transform.position).NoY()) > fov / 2) return false;
			return true;
		}
		return false;
	}

	public void SetFacing(Vector3 direction) {
		transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
	}

	private void Awake() {
		m_range = runRange;
		m_frameCounter = Random.Range(0, m_additionalRaycastDelta);
		m_meshFilter.mesh = new Mesh();

		m_meshFilter.mesh.SetVertices(m_conePositions);
		m_meshFilter.mesh.SetTriangles(new List<int>() {
			0, 1, 2, 0, 2, 3
		}, 0);
		m_meshFilter.mesh.RecalculateNormals();
		m_meshFilter.mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 15);
	}

	private Vector3 GetRaycastedPosition(float angle) {
		var direction = Quaternion.Euler(0, angle, 0) * transform.forward;
		var point = Physics.Raycast(transform.position, direction, out var hit, m_range, visionMask, QueryTriggerInteraction.Ignore) ? hit.point : transform.position + direction * m_range;
		point = transform.InverseTransformPoint(point);
		return point.NoY();
	}

	private void Update() {
		float previousRange = m_range;
		if (Game.MainGameInfo.Ballin && InputSubscribe.Sneaking) {
			m_range = sneakRange;
		} else {
			m_range = runRange;
		}
		if (previousRange != m_range) {
			UpdateCones();
		}
		for (int i = 0; i < m_conePositions.Length; i++) {
				m_conePositions[i] = Vector3.Lerp(m_conePositions[i], Vector3.Lerp(m_previousTargetConePositions[i], m_targetConePositions[i], (m_frameCounter % m_additionalRaycastDelta) / (float)m_additionalRaycastDelta), Time.deltaTime * m_displaySmoothing);
			}

		m_meshFilter.mesh.SetVertices(m_conePositions);
	}

	private void FixedUpdate() {
		UpdateCones();
	}

	public void SetActive(bool v) {
		m_enabled = v;
		m_renderer.enabled = v;
	}

	private void UpdateCones() {
		if (!m_meshFilter) return;
        m_frameCounter++;
        if (m_frameCounter % m_additionalRaycastDelta != 0) return;
        System.Array.Copy(m_targetConePositions, m_previousTargetConePositions, m_targetConePositions.Length);
        m_targetConePositions[0] = new Vector3(0, Room.STANDARD_FLOOR_Y + 0.05f, 0);
        m_targetConePositions[1] = GetRaycastedPosition(-fov / 2f);
        m_targetConePositions[2] = GetRaycastedPosition(Mathf.Clamp(Vector3.SignedAngle(transform.forward, (trackedObject.transform.position - transform.position).NoY(), Vector3.up), -fov / 2f + 1f, fov / 2f - 1f));
        m_targetConePositions[3] = GetRaycastedPosition(fov / 2f);
	}
}
