﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGun : MonoBehaviour {

	[SerializeField]
	private Transform gunOrigin;

	private List<GameObject> enemiesInRange = new List<GameObject>();

	public void Update() {

		// Draw a line for testing
		if(closestEnemy != null)
			Debug.DrawRay(gunOrigin.transform.position, closestEnemy.transform.position - gunOrigin.transform.position);
	}

	public void OnTriggerEnter(Collider collider) {
		GameObject go = collider.gameObject;
		if(go.CompareTag("Enemy")) {
			AddEnemy(go);
		}
	}

	public void OnTriggerExit(Collider collider) {
		GameObject go = collider.gameObject;
		if(go.CompareTag("Enemy")) {
			RemoveEnemy(go);
		}
	}

	private void AddEnemy(GameObject enemy) {
		if(!enemiesInRange.Contains(enemy))
			enemiesInRange.Add(enemy);
	}

	private void RemoveEnemy(GameObject enemy) {
		if(enemiesInRange.Contains(enemy))
			enemiesInRange.Remove(enemy);
	}

	GameObject closestEnemy {
		get {
			float closestDist = float.MaxValue;
			GameObject closestEnemy = null;

			foreach(GameObject enemy in enemiesInRange) {
				Vector3 dir = enemy.transform.position - transform.position;
				float dist = dir.magnitude;
				if(dist < closestDist) {
					RaycastHit hit;
					if(Physics.Raycast(gunOrigin.transform.position, dir, out hit, Mathf.Infinity)) {
						if(hit.collider.gameObject == enemy) {
							closestEnemy = enemy;
							closestDist = dist;
						}
					}
				}
			}

			return closestEnemy;
		}
	}

}
