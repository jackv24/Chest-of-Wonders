using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.IO;
using NodeCanvas.Framework;

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

	[Test]
	public void PersistentBlackboardSavesAndLoads()
	{
		var blackboardObj = new GameObject("Blackboard", typeof(Blackboard), typeof(PersistentBlackboard));
		var blackboard = blackboardObj.GetComponent<Blackboard>();
		var persistent = blackboardObj.GetComponent<PersistentBlackboard>();
		persistent.Setup(saveManager);

		Variable testBool = blackboard.AddVariable("Test Bool", true);
		saveManager.SaveGame(true);
		testBool.value = false;

		// Temp load event was already expended, so call setup again (as if we've re-entered the scene)
		persistent.Setup(saveManager);

		Assert.IsTrue((bool)testBool.value);
	}

	[Test]
	public void PersistentBlackboardSavesAndLoadsWithExtraVariables()
	{
		var blackboardObj = new GameObject("Blackboard", typeof(Blackboard), typeof(PersistentBlackboard));
		var blackboard = blackboardObj.GetComponent<Blackboard>();
		var persistent = blackboardObj.GetComponent<PersistentBlackboard>();
		persistent.Setup(saveManager);

		blackboard.AddVariable("Test Bool", true);
		saveManager.SaveGame(true);
		blackboard.AddVariable("Test String", "Test");

		// Temp load event was already expended, so call setup again (as if we've re-entered the scene)
		persistent.Setup(saveManager);

		Assert.NotNull(blackboard.GetVariable("Test String"));
	}
}
