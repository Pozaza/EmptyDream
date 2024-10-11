using System.Collections;
using UnityEngine;

public class FlyAround : MonoBehaviour {
	public float speed = .75f;
	Vector2 _newPosition;
	Quaternion _newRot;
	bool _canChangePosition = true;

	void Start() => PositionChange();
	IEnumerator PositionChange() {
		yield return new WaitForSeconds(Random.Range(1f, 5f));
		_newPosition = new Vector2(Random.Range(-.25f, .25f), Random.Range(-.1f, .15f));
		_canChangePosition = true;
		_newRot.w = transform.localPosition.x > _newPosition.x ? 180 : -180;
		gameObject.GetChildObj(0).localPosition = new Vector3(transform.localPosition.x > _newPosition.x ? .01f : -.01f, gameObject.GetChildObj(0).localPosition.y, 0);
		GetComponent<SpriteRenderer>().flipX = !(transform.localPosition.x > _newPosition.x);
	}
	void Update() {
		float distance = Vector2.Distance(transform.localPosition, _newPosition);
		if (distance < .05f && _canChangePosition) {
			_canChangePosition = false;
			StartCoroutine(PositionChange());
		}
		_newRot.z = distance * 100;
		transform.localPosition = Vector2.Lerp(transform.localPosition, _newPosition, Time.deltaTime * speed);
		transform.localRotation = _newRot;
	}
}
