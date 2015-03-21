using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
	[System.Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;

		public Count (int min, int max)
		{
			this.minimum = min;
			this.maximum = max;
		}
	}

	public int columns = 8;
	public int rows = 8;
	public Count wallCount = new Count (5, 9);
	public Count food = new Count (1, 5);
	public GameObject exit;
	public GameObject[] outerWallTiles;
	public GameObject[] wallTiles;
	public GameObject[] floorTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;

	private Transform outerWallsHolder;
	private Transform floorsHolder;
	private Transform itemsHolder;
	private List<Vector3> gridPositions = new List<Vector3> ();

	void InitializeGrid ()
	{
		for (int x = 0; x < rows; x++)
			for (int y = 0; y < columns; y++)
				if (!((x == 0 && y == 0) || (x == (columns - 1) && y == (rows - 1))))
					gridPositions.Add (new Vector3 (x, y, 0));
	}

	Vector3 GetRandomPos ()
	{
		int randomIndex = Random.Range (0, gridPositions.Count);
		Vector3 randomPos = gridPositions [randomIndex];
		gridPositions.RemoveAt (randomIndex);
		return randomPos;
	}

	public void SetupScene (int level)
	{
		// 0. Intialize List
		InitializeGrid ();

		// 1. Load outer walls
		LoadOuterWalls ();
		
		// 2. Load floors
		LoadFloors (floorTiles);

		itemsHolder = new GameObject ("Items").transform;

		// 3. Load inner walls
		LoadObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum, itemsHolder);

		// 4. Load food
		LoadObjectAtRandom (foodTiles, food.minimum, food.maximum, itemsHolder);

		// 5. Load enemies
		int enemyCount = (int)Mathf.Log (level, 2f);
		LoadObjectAtRandom (enemyTiles, enemyCount, enemyCount, itemsHolder);

		// 6. Load exit
		Instantiate (exit, new Vector3 (rows - 1, columns - 1), Quaternion.identity);

		// 7. Load player.

		gridPositions.Clear ();
	}

	void LoadFloors (GameObject[] floorTiles)
	{
		floorsHolder = new GameObject ("Floors").transform;
		for (int i = 0; i < gridPositions.Count; i++) {
			int randomObjectCount = Random.Range (0, floorTiles.Length);
			GameObject instance = Instantiate (floorTiles [randomObjectCount], gridPositions [i], Quaternion.identity) as GameObject;
			instance.transform.SetParent (floorsHolder);
		}

		int randomObjectCount2 = Random.Range (0, floorTiles.Length);
		GameObject instance2 = Instantiate (floorTiles [randomObjectCount2], Vector3.zero, Quaternion.identity) as GameObject;
		instance2.transform.SetParent (floorsHolder);
	}
	
	void LoadObjectAtRandom (GameObject[] objects, int min, int max, Transform parent)
	{
		int randomObjectCount = Random.Range (min, max);
		for (int i = 0; i < randomObjectCount; i++) {
			int randomObjItem = Random.Range (0, objects.Length);
			GameObject item = Instantiate (objects [randomObjItem], GetRandomPos (), Quaternion.identity) as GameObject;
			item.transform.SetParent(parent);
		}
	}

	void LoadOuterWalls ()
	{
		outerWallsHolder = new GameObject ("OuterWalls").transform;
		for (int x = -1; x <= columns; x++) {
			LoadRandomOuterWall (new Vector3 (x, -1));
			LoadRandomOuterWall (new Vector3 (x, rows));
		}

		for (int y = 0; y < rows; y++) {
			LoadRandomOuterWall (new Vector3 (-1, y));
			LoadRandomOuterWall (new Vector3 (columns, y));
		}

	}

	void LoadRandomOuterWall (Vector3 location)
	{
		int randomItemIndex = Random.Range (0, outerWallTiles.Length);
		GameObject instance = Instantiate (outerWallTiles [randomItemIndex], location, Quaternion.identity) as GameObject;
		instance.transform.SetParent (outerWallsHolder);
	}
}
