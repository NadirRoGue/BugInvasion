using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class SpawnZoneController : MonoBehaviour
{

	public int _spawnZone = -1;
	public int _waveSoldiersToSpawn = 0;
	public float _waveSpawnPace = 0.0f;

	//private Queue<GameEnemyInstance> _idleSoldiers = new Queue<GameEnemyInstance>();
	private List<GameEnemyInstance> _soldiers = new List<GameEnemyInstance> ();

	public float _spawnCountdown;

	private Queue<byte> _targets;

	public void setTargets (Queue<byte> targets)
	{
		lock (this) {
			_targets = targets;
			foreach (GameEnemyInstance gei in _soldiers) {
				giveOrderToSoldier (gei);
			}
		}
	}

	public void setWaveData (int armySize, float spawnPace)
	{
		_waveSoldiersToSpawn = armySize;
		_waveSpawnPace = spawnPace;

		_spawnCountdown = _waveSpawnPace;
	}

	public void addIldeSoldier (GameEnemyInstance gei)
	{
		giveOrderToSoldier (gei);
		//_idleSoldiers.Enqueue (gei);
	}

	private void giveOrderToSoldier (GameEnemyInstance gei)
	{
		if (gei != null && !gei.isDead ()) {

			EnemyController ec = gei.getGameInstance ().GetComponent<EnemyController> ();
			byte source = 0;
			if (!ec.orderCompleted ()) {
				source = ec.getPreviousCompletedTarget ();
			} else {
				source = ec.currentTarget ();
			}

			byte target = _targets.Dequeue ();
			while (target == source) {
				_targets.Enqueue (target);
				target = _targets.Dequeue ();
			}

			if (World.getInstance ().getObjectBySpwan (target) != null) { 

				bool validOrder = true;

				if (target != 0) {
					Vector3 soldierPos = gei.getGameInstance ().transform.position;
					Vector3 finalTargetPos = SpawnTable.getInstance ().getTargetSpawns () [0].getAveragePosition ();

					Vector3 targetPos = SpawnTable.getInstance ().getTowerSpawns () [target].getAveragePosition ();

					if ((soldierPos - targetPos).sqrMagnitude > (soldierPos - finalTargetPos).sqrMagnitude)
						validOrder = false;
				}

				if (validOrder) {
					SwarmOrder so = SwarmController.getInstance ().createOrder (source, target, _spawnZone);
					if (gei.getGameInstance ().GetComponent<EnemyController> () != null)
						gei.getGameInstance ().GetComponent<EnemyController> ().setOrder (so);
				}
				_targets.Enqueue (target);
			}
		}
	}

	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (_waveSoldiersToSpawn > 0) {
			
			if (_spawnCountdown <= 0.0f) {
				
				byte order = _targets.Dequeue ();

				GameEnemyInstance gei = null;
				if ((gei = SwarmController.getInstance ().spawnSoldier ("Araña Exploradora", _spawnZone, order)) != null) {
					_soldiers.Add (gei);
					_spawnCountdown = _waveSpawnPace;
					_waveSoldiersToSpawn--;
				}

				_targets.Enqueue (order);
			} else {
				_spawnCountdown -= Time.deltaTime;
			}
		}

		/*if (_idleSoldiers.Count > 0) {
			GameEnemyInstance gei = _idleSoldiers.Dequeue ();
			giveOrderToSoldier (gei);
		}*/
	}
}
