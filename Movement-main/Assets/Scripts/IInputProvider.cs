using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputProvider{

    float GetDeltaTime();
    Vector2 GetMovementInput();
    bool GetJumpButtonDown();
    bool GetJumpButton();
}
