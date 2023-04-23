using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixtureSource(typeof(LevelProvider))]
public class LevelSmokeTests {
    private string m_levelToSmoke;
    private LogSeverityTracker m_logTracker;
    public LevelSmokeTests(string levelToSmoke) {
        m_levelToSmoke = levelToSmoke;
    }

    [OneTimeSetUp]
    public void LoadScene() {
        SceneManager.LoadScene(m_levelToSmoke);
        m_logTracker = new LogSeverityTracker();
    }

    [Test, Order(1)]
    public void LoadsCleanly() {
        m_logTracker.AssertCleanLog();
    }

    [UnityTest, Order(2)]
    public IEnumerator RunsCleanly() {
        // wait some arbitrary time
        yield return new WaitForSeconds(5);
        m_logTracker.AssertCleanLog();
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest, Order(3)]
    public IEnumerator UnloadsCleanly() {
        // how you unload is game-dependent 
        yield return SceneManager.LoadSceneAsync("MainMenu");
        m_logTracker.AssertCleanLog();
    }
}
