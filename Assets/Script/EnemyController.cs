using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class EnemyController : MonoBehaviour {

	public GameEnemyInstance _enemyInstance {
		get;
		set;
	}

	private SwarmOrder _order;
	private bool _orderCompleted = false;
	private byte _previousCompletedOrder = 0;
	private Vector3 _nextTarget = new Vector3();
	private bool _walkStablish = false;

	private Creature _attackInstance;

	private bool _attacking = false;
	private float _attackDelay = 0.0f;

	private Vector3 _rotationInitPos = new Vector3();
	private float _rotationAlpha = 0.0f;
	private float _alphaSpeed = 0.05f;

	private float _walkSpeed = 5.0f;

	private static readonly float MIN_DISTANCE = 1.0f; // Squared distance, our min distance must be 3.0f
	private static readonly float MIN_DISTANCE_TO_FINAL_TARGET = 0.1f;//0.25f;//Mathf.Pow(0.5f, 2.0f);

	private float _minDistanceToCheck = MIN_DISTANCE;

	void Awake() {
		gameObject.AddComponent<Rigidbody> ().isKinematic = true;
		gameObject.AddComponent<BoxCollider> ().isTrigger = true;
	}

	void Start () {
		Vector3 boundsSize = _enemyInstance.getTemplate ().getBounds ().extents;
		gameObject.GetComponent<BoxCollider> ().size = boundsSize * 2.0f;
	}
	
	void Update () {

		if (_attacking) {
			//_enemyInstance.animatorSwitch (true);
			if (_attackDelay <= 0.0f) {
				
				if (_attackInstance != null && !_attackInstance.isDead()) {
					transform.LookAt (_attackInstance.getPosition ());
					_enemyInstance.animatorSwitch (true);
					_attackInstance.onAttack (_enemyInstance, _enemyInstance.getEnemyTemplate ().getAttackDamage ());
					_attackDelay = _enemyInstance.getEnemyTemplate ().getAttackFrequency ();
				} else {
					_attackInstance = null;
					_attacking = false;

					// NO TARGET AND NO PATH TO FOLLOW, REQUEST ORDERS!
					if (_order.getPath () == null || _order.getPath ().Count == 0) {
						SwarmController.getInstance().notifyIdleSoldier(_enemyInstance);
						_enemyInstance.animatorSwitch (false);
					}
				}

			} else {
				_attackDelay -= Time.deltaTime;
			}
		} else {
			if (!_walkStablish || distanceToNextTarget () < _minDistanceToCheck) {

				if (_order != null) {
					if (_order.getPath ().Count > 0) {
						_nextTarget = _order.nextPosition ();

						if (_order.getPath ().Count == 0)
							_minDistanceToCheck = MIN_DISTANCE_TO_FINAL_TARGET;

						// Compute initial position (in the direction of the forward vector, forming a 
						// rect triangle with the next position, to be used for interpolation and create
						// a smooth "look at"
						Vector2 enemyPos = new Vector2 (transform.position.x, transform.position.z);

						// Direction vector to next target
						Vector2 targetPos = new Vector2 (_nextTarget.x, _nextTarget.z);
						targetPos -= enemyPos;
						targetPos.Normalize ();

						// Forward vector
						Vector2 enemyForward = new Vector2 (transform.forward.x, transform.forward.z);
						float angle = getAngleBetween (enemyForward, targetPos);
						float cosAngle = Mathf.Cos (angle);

						// Compute angle using arc tangent difference
						float distToTarget = (_nextTarget - transform.position).magnitude;

						_rotationInitPos = transform.position + (transform.forward * distToTarget * cosAngle);
						_rotationAlpha = 0.0f;

						_walkStablish = true;
					} else {
						_walkStablish = false;

						_orderCompleted = true;
						_previousCompletedOrder = _order.getTargetId ();

						// DESTINATION REACHED AND NO TARGET TO ATTACK, REQUEST ORDERS!
						if (_attackInstance == null || _attackInstance.isDead()) {
							_attackInstance = null;
							SwarmController.getInstance().notifyIdleSoldier(_enemyInstance);
							_enemyInstance.animatorSwitch (false);
						} else {
							_attacking = true;
							_enemyInstance.updateAnimation (true);
						}
					}
				}
			}

			if (_walkStablish) {
				_enemyInstance.animatorSwitch (true);
				_enemyInstance.updateAnimation (false);
				// Smooth look at
				if (_rotationAlpha < 1.0) {
					Vector3 nextLookAt = Formulas.lerpVector (_rotationInitPos, _nextTarget, _rotationAlpha);
					_rotationAlpha += _alphaSpeed;
					transform.LookAt (nextLookAt);
				}

				// Walk
				transform.position += (transform.forward * _walkSpeed * Time.deltaTime);
			}
		}
	}

	private float distanceToNextTarget() {
		return (_nextTarget - transform.position).sqrMagnitude;
	}

	private float getAngleBetween(Vector2 dirVector, Vector2 targetVector) {
		float forwardAngle = Mathf.Atan2 (dirVector.y, dirVector.x);
		float targetAngle = Mathf.Atan2 (targetVector.y, targetVector.x);

		return forwardAngle - targetAngle;
	}

	public void setOrder(SwarmOrder order) {

		_enemyInstance.animatorSwitch (false);
		_orderCompleted = false;

		if (_walkStablish) {
			_walkStablish = false;
			order.recalculatePath (transform.position);
		} else if (_attacking) {
			_attacking = false;
		}

		_order = order;
		if (_order != null)
			_attackInstance = World.getInstance ().getObjectBySpwan (_order.getTargetId ());
		else
			Debug.Log ("EnemyController.setOrder(): Orden nula!");
	}

	public bool orderCompleted() {
		return _orderCompleted;
	}

	public byte currentTarget() {
		return _order.getTargetId ();
	}

	public byte getPreviousCompletedTarget() {
		return _previousCompletedOrder;
	}

	void OnTriggerEnter(Collider other) {
		AmmunitionController ac = null;
		if ((ac = other.transform.GetComponent<AmmunitionController> ()) != null) {
			GameTowerInstance gti = ac.getEffector () as GameTowerInstance;
			_enemyInstance.onAttack (gti, gti.getAttackDmg());

			Destroy (other.transform.gameObject);
		}
	}
}
