using UnityEngine;

public class RelativeRotater : MonoBehaviour {
	public Transform point;
	public float speed;
	public Vector3 axis;

	void Update() => transform.RotateAround(point.transform.position, axis, speed * Time.deltaTime);
}