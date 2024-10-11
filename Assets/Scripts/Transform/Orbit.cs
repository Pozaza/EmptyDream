using UnityEngine;

public class Orbit : MonoBehaviour {
	public float xSpread, ySpread, zSpread, yOffset, rotSpeed;
	public Transform centerPoint;
	public bool rotateClockwise;
	float timer;

	void Update() {
		timer += Time.deltaTime * rotSpeed;
		Rotate();
	}
	void Rotate() {
		float x = Mathf.Cos(timer) * (rotateClockwise ? -1 : 1) * xSpread;
		float y = Mathf.Cos(timer) * ySpread;
		float z = Mathf.Sin(timer) * zSpread;
		Vector3 pos = new(x, rotateClockwise ? yOffset : y, z);
		transform.position = pos + centerPoint.position;
	}
}