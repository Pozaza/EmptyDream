using NaughtyAttributes;
using UnityEngine;

public class WaypointsMove : MonoBehaviour {
	public Transform[] waypoints;
	public float patrolSpeed = 1.5f;
	public SpriteRenderer spriteToFlip;
	public bool loop, hasEyes = true;
	[ShowIf("hasEyes")] public Transform eyes;
	[ShowIf("loop")] public float minPauseDuration = .75f;
	[ShowIf("loop")] public float maxPauseDuration = 1.0f;
	float _curTime = .4f;
	int _currentWaypoint = 0;

	void Update() {
		if (_currentWaypoint < waypoints.Length)
			Patrol();
		else if (loop)
			_currentWaypoint = 0;
	}
	void Patrol() {
		Vector3 target = waypoints[_currentWaypoint].position;
		target.y = transform.position.y;
		Vector3 moveDirection = target - transform.position;
		float pauseDuration = Random.Range(minPauseDuration, maxPauseDuration);
		if (moveDirection.magnitude < .5f) {
			if (_curTime == 0)
				_curTime = Time.time;
			if ((Time.time - _curTime) >= pauseDuration) {
				if (moveDirection.x < 0) {
					spriteToFlip.flipX = true;
					if (hasEyes)
						eyes.localRotation = Quaternion.Euler(0, 180, 0);
				} else {
					spriteToFlip.flipX = false;
					if (hasEyes)
						eyes.localRotation = Quaternion.Euler(0, 0, 0);
				}
				_currentWaypoint++;
				_curTime = 0;
			}
		} else
			transform.Translate(patrolSpeed * Time.deltaTime * moveDirection.normalized);
	}
}