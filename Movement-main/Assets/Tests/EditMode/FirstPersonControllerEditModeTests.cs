using NUnit.Framework;
using UnityEngine;
using Q3Movement;

public class FirstPersonControllerEditModeTests {
    [Test]
    public void FirstPersonControllerPrefab_HasRequiredComponents() {
        GameObject firstPersonControllerPrefab = Resources.Load<GameObject>("FirstPersonController");
        Assert.IsNotNull(firstPersonControllerPrefab);
        Assert.IsNotNull(firstPersonControllerPrefab.GetComponent<Q3PlayerController>());
        Assert.IsNotNull(firstPersonControllerPrefab.GetComponent<CharacterController>());
        Assert.IsNotNull(firstPersonControllerPrefab.GetComponentInChildren<Camera>());
    }
}
