using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.IO;

public class SaveDataTests
{
	private SaveManager saveManager;

	[SetUp]
	public void Setup()
	{
		saveManager = new GameObject("Save Manager", typeof(SaveManager)).GetComponent<SaveManager>();
		saveManager.SaveSlot = -1;
		saveManager.LoadGame(true);
	}

	[TearDown]
	public void Teardown()
	{
		if (File.Exists(saveManager.SaveLocation))
			File.Delete(saveManager.SaveLocation);

		Object.DestroyImmediate(saveManager.gameObject);
	}

	[Test]
	public void SaveFileWritten()
	{
		saveManager.SaveGame(true);

		Assert.IsTrue(File.Exists(saveManager.SaveLocation));
	}

	[Test]
	public void PersistentObjectSavesAndLoads()
	{
		var obj = new GameObject("TestObj");

		var persistOne = new PersistentObject(saveManager);
		persistOne.Setup(obj);
		persistOne.SaveState(true);

		saveManager.SaveGame(false);

		bool isActivated = false;
		var persistTwo = new PersistentObject(saveManager);
		persistTwo.OnStateLoaded += activated => isActivated = activated;
		persistTwo.Setup(obj);

		saveManager.LoadGame(false);

		Assert.IsTrue(isActivated);
	}
}
