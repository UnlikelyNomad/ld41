using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorOnStart : MonoBehaviour {

    public Image image = null;
    public Text text = null;
    public Light l = null;
    public SpriteRenderer sr = null;

    private static List<ColorOnStart> instances = new List<ColorOnStart>();

	// Use this for initialization
	void Start () {
        image = GetComponent<Image>();
        text = GetComponent<Text>();
        l = GetComponent<Light>();
        sr = GetComponent<SpriteRenderer>();

        UpdateColor();

        instances.Add(this);
	}

    void OnDestroy() {
        instances.Remove(this);
    }

    public static void ResetInstances() {
        instances.Clear();
    }

    void UpdateColor() {
        Color c = GameOptions.Instance.MainColor;
        if (image != null) {
            image.color = c;
        }

        if (text != null) {
            text.color = c;
        }

        if (l != null) {
            l.color = c;
        }

        if (sr != null) {
            sr.color = c;
        }
    }

    public static void UpdateAllColors() {
        for (int i = 0; i < instances.Count; ++i) {
            instances[i].UpdateColor();
        }
    }
}
