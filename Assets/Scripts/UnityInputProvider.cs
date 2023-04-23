using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityInputProvider : IInputProvider {

    public float GetDeltaTime() {
        return Time.deltaTime; 
    }
    public Vector2 GetMovementInput() {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public bool GetJumpButtonDown() {
        return Input.GetButtonDown("Jump");
    }

    public bool GetJumpButton() {
        return Input.GetButton("Jump");
    }
}
