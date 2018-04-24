using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVolumeOnStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        AudioSource src = GetComponent<AudioSource>();

        src.volume = GameOptions.Instance.musicVolume / 100.0f;
	}
}
