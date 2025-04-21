using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PPShaderControls : MonoBehaviour {
	[SerializeField] private Volume postProcessingVolume;
	[SerializeField] private bool disable;

	[Header("Post Processing Profiles")]
	[SerializeField] private VolumeProfile postProfileMain;
	[SerializeField] private VolumeProfile postProfileSecondary;

	public void MainPostProcess() {

	}

	public void SecondaryPostProcess() {

	}
}
