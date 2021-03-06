﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(SpawnManager)), RequireComponent(typeof(UIManager))]
public class LevelManager : MonoBehaviour {

	private SpawnManager spawnManager;

	private UIManager uiManager;

	[SerializeField]
	private Nexus nexus;

	private bool lost = false;

	[SerializeField]
	private int maxWaves = 1;

	private int currentWave = 0;

	[SerializeField]
	private GameObject endGameCanvas;

	[SerializeField]
	private AudioSource buildSound;
	
	[SerializeField]
	private AudioSource waveSound;

	private GameObject player;
	
	private LevelPhase levelPhase = LevelPhase.BuildPhase;
	// Automatic call the start function of each phase
	private LevelPhase Phase {
		get {
			return levelPhase;
		}
		set {
			switch(value) {
			case LevelPhase.BuildPhase: 
				StartBuildPhase();
				break;
			case LevelPhase.WavePhase: 
				StartNextWavePhase();
				break;
			case LevelPhase.EndPhase: 
				StartEndPhase();
				break;
			}
			uiManager.OnPhaseChanged(value);
			levelPhase = value;
		}
	}

	private void Awake() {
		Time.timeScale = 1;
		spawnManager = GetComponent<SpawnManager>();
		uiManager = GetComponent<UIManager>();
		uiManager.NextWave(currentWave, maxWaves);
	}

	private void Start() {
		Phase = LevelPhase.BuildPhase;

		player = GameObject.FindGameObjectWithTag("Player");
	}


	private void Update () {		
		switch(Phase) {
			case LevelPhase.BuildPhase: 
				UpdateBuildPhase();
				break;
			case LevelPhase.WavePhase: 
				UpdateWavePhase();
				break;
			case LevelPhase.EndPhase: 
				UpdateEndPhase();
				break;
		}

	}

	private void UpdateBuildPhase() {
		// If the player presses 'G' the build phase ends and the wave starts spawning
		if(Input.GetKeyDown(KeyCode.G)) {
			Phase = LevelPhase.WavePhase;
		}
	}

	private float timeBetweenSpawnsInSec = 2f;
	private float nextSpawn = 2f;
	private float spawnTime = 20f;
	private void UpdateWavePhase() {
		// Not the best way to detect no enemies, but it works ok
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

		if(!spawnManager.SpawningEnemies && enemies.Length == 0) {
			if(currentWave == maxWaves) 
				Phase = LevelPhase.EndPhase;
			else
				Phase = LevelPhase.BuildPhase;			
		}

		// If the nexus gets destroyed go to the endphase
		if(nexus.IsDestroyed) {
			lost = true;
			Phase = LevelPhase.EndPhase;
		}
	}

	private void UpdateEndPhase() {
		// If the player wants to end this level he can proceed to the menu?
	}

	private void StartBuildPhase() {
		// Give the player money
		FindObjectOfType<Player>().Money += 50;

		spawnManager.PrepareNextWave(currentWave+1);

		buildSound.Play();
		waveSound.Stop();
	}

	private void StartNextWavePhase() {
		spawnManager.SpawnWave(currentWave);
		currentWave++;
		uiManager.NextWave(currentWave, maxWaves);

		buildSound.Stop();
		waveSound.Play();
	}

	private void StartEndPhase() {
		Time.timeScale = 0;
		if(lost)
			Debug.Log("Lost");
		else
			Debug.Log("Won");

		endGameCanvas.SetActive(true);
		endGameCanvas.GetComponent<EndGameCanvas>().SetWinningStatus(!lost);

		player.GetComponent<FirstPersonController>().enabled = false;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;

		buildSound.Play();
		waveSound.Stop();
	}

}

public enum LevelPhase {
	BuildPhase,
	WavePhase,
	EndPhase
}
