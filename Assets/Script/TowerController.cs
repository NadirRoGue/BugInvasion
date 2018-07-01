using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class TowerController : MonoBehaviour {

	private GameTowerInstance _towerInstance;
	private GameObject _particleSystem;
	private Dictionary<int, GameEnemyInstance> _knownList = new Dictionary<int, GameEnemyInstance> ();
	private GameEnemyInstance _enemy = null;
	private Transform _yaw;
	private Transform _pitch;
	private float _shotCountDown = 0.0f;
	private GameObject _colliderGizmo;

	public GameTowerInstance getTowerInstance() {
		return _towerInstance;
	}

	public void updateKnowlistRange() {
		_colliderGizmo.GetComponent<SphereCollider> ().radius = _towerInstance.getAttackRange ();
	}

	public void setTowerInstance(GameTowerInstance instance) {
		_towerInstance = instance;

		_colliderGizmo = new GameObject ();
		_colliderGizmo.name = "Collider_gizmo_tower_" + _towerInstance.getObjectId ();
		_colliderGizmo.transform.parent = transform;
		_colliderGizmo.layer = LayerMask.NameToLayer ("Ignore Raycast");
		_colliderGizmo.AddComponent<SphereCollider> ().isTrigger = true;
		_colliderGizmo.GetComponent<SphereCollider> ().radius = _towerInstance.getAttackRange();
		_colliderGizmo.GetComponent<SphereCollider> ().center = new Vector3(0, -_towerInstance.getTower().getAttackRange() + _towerInstance.getTower().getBounds().size.y, 0);
		_colliderGizmo.AddComponent<KnowlistController> ();

		Bounds b = instance.getTower ().getBounds ();
		_towerInstance.getGameInstance().AddComponent<BoxCollider> ().size = b.size;
		_towerInstance.getGameInstance().GetComponent<BoxCollider> ().center = new Vector3(0,b.size.y / 2,0);

		gameObject.AddComponent<AudioSource> ().loop = false;
		SoundTable.getInstance ().getAudioPlayer (gameObject.GetComponent<AudioSource> (), _towerInstance.getTower ().getAttackSoundName ());
	}

	void Start () {
		_yaw = GetComponent<Transform> ().Find ("control_bone/yaw_bone");
		_pitch = _yaw.Find ("pitch_bone");

		_particleSystem = ParticleSystemTable.getInstance ().instantiateParticleSystem (_towerInstance.getTower().getAttackParticleSystemName(), _pitch);
	}
	
	// Update is called once per frame
	void Update () {

		if (_towerInstance.isBeingRepaired ())
			return;
		
		if (_colliderGizmo != null) {
			_knownList = _colliderGizmo.GetComponent<KnowlistController> ().getKnowList ();
			if (_knownList.Count > 0) {
				selectTarget ();
			}
		}

		if (_enemy != null && !_enemy.isDead ()) {

			// YAW ROTATION
			Vector2 towerPos = new Vector2 (transform.position.x, transform.position.z);

			Transform target = _enemy.getGameInstance ().transform;

			Vector2 targetPos = new Vector2 (target.position.x, target.position.z);
			targetPos -= towerPos;
			targetPos.Normalize ();

			Vector2 towerForward = new Vector2 (transform.forward.x, transform.forward.z);

			float angle = getAngleBetween (towerForward, targetPos);

			Quaternion q = _yaw.rotation;
			Vector3 euler = q.eulerAngles;
			euler.y = angle;
			_yaw.rotation = Quaternion.Euler (euler);

			// PITCH ROTATION
			Vector3 targetPos3D = target.position;
			Vector3 baseTower = transform.position - targetPos3D;
			baseTower.Normalize ();

			Vector3 pitchPos = _pitch.position;
			//pitchPos.y += _pitch.position.y;
			pitchPos -= targetPos3D;
			pitchPos.Normalize ();

			Vector2 dirVector3 = new Vector2 (baseTower.y, baseTower.magnitude);
			Vector2 targetVector3 = new Vector2 (pitchPos.y, pitchPos.magnitude);

			float pitchAngle = getAngleBetween (dirVector3, targetVector3);

			Quaternion qPitch = _pitch.rotation;
			Vector3 eulerPitch = qPitch.eulerAngles;
			eulerPitch.x = pitchAngle;
			_pitch.rotation = Quaternion.Euler (eulerPitch);

			if (_shotCountDown <= 0.0f) {

				Vector3 spawnPos = _pitch.position;
				//spawnPos += (_pitch.forward + new Vector3 (0.0f, 0.0f, 0.7f));

				if (_particleSystem != null) {
					_particleSystem.GetComponent<ParticleSystem> ().Play ();
				}

				if (gameObject.GetComponent<AudioSource> () != null) {
					if (gameObject.GetComponent<AudioSource> ().isPlaying)
						gameObject.GetComponent<AudioSource> ().Stop ();
					gameObject.GetComponent<AudioSource> ().Play ();
				}

				GameObject ammo = _towerInstance.getAmmoTemplate().instantiate ();
				ammo.GetComponent<AmmunitionController> ().setKeyPos (spawnPos, target.position, _towerInstance);

				_shotCountDown = _towerInstance.getAttackFrequency ();
			} else if (_shotCountDown > 0.0f) {
				_shotCountDown -= Time.deltaTime;
			}
		} else {
			_enemy = null;
		}
	}

	private float getAngleBetween(Vector2 dirVector, Vector2 targetVector) {
		float forwardAngle = Mathf.Atan2(dirVector.y, dirVector.x) * Mathf.Rad2Deg;
		float targetAngle = Mathf.Atan2 (targetVector.y, targetVector.x) * Mathf.Rad2Deg;

		return forwardAngle - targetAngle;
	}

	private void selectTarget() {
		Dictionary<int, GameEnemyInstance> tempKnowlist = new Dictionary<int, GameEnemyInstance> (_knownList);

		if (_enemy != null && tempKnowlist.ContainsKey (_enemy.getObjectId ())) {
			return;
		} 

		float closer = Mathf.Infinity;
		GameEnemyInstance closerEnemy = null;
		Vector3 pos = transform.position;
		float temp = Mathf.Infinity;

		foreach (int geiId in tempKnowlist.Keys) {

			GameEnemyInstance gei = _knownList [geiId];

			if (gei != null && !gei.isDead ()) {
				if ((temp = (gei.getGameInstance ().transform.position - pos).sqrMagnitude) < closer) {
					closer = temp;
					closerEnemy = gei;
				}
			} else {
				_knownList.Remove (geiId);
			}
		}

		if (closerEnemy != null) {
			_enemy = closerEnemy;
		}
	}
}
