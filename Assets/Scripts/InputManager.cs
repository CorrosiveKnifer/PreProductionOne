using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// Rachael Colaco, Michael Jordan
/// </summary>
/// 

#region InputEnums
public enum KeyType
{
    Q, W, E, R, T, Y, U, I, O, P,
    A, S, D, F, G, H, J, K, L,
    Z, X, C, V, B, N, M,
    NUM_ONE, NUM_TWO, NUM_THREE, NUM_FOUR, NUM_FIVE, NUM_SIX, NUM_SEVEN, NUM_EIGHT, NUM_NINE, NUM_ZERO,
    L_SHIFT, L_CTRL, L_ALT, TAB, ESC,
    R_SHIFT, R_CTRL, R_ALT, ENTER,
}
public enum MouseButton
{
    LEFT, MIDDLE, RIGHT
}
public enum ButtonType
{
    NORTH, SOUTH, EAST, WEST,
    START, SELECT, 
    LT, LB, LS, RT, RB, RS,
}

public enum StickType
{
    LEFT,
    RIGHT
}

#endregion

public class InputManager : MonoBehaviour
{
    #region Singleton

    private static InputManager _instance = null;
    public static InputManager instance 
    {
        get 
        {
            if (_instance == null)
            {
                GameObject loader = new GameObject();
                _instance = loader.AddComponent<InputManager>();
                return loader.GetComponent<InputManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != this)
        {
            InitialFunc();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Second Instance of InputManager was created, this instance was destroyed.");
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    #endregion

    protected Mouse mouse;
    protected Keyboard keyboard = Keyboard.current;
    protected int gamepadCount;
    //public Gamepad gamepad;

    //public struct Controller
    //{
    //    public Controller(bool _isKeyboard = false, int _controllerID = 0, Gamepad _gamepad = null)
    //    {
    //        isKeyboard = _isKeyboard;
    //        controllerID = _controllerID;
    //        gamepad = _gamepad;
    //    }
    //    public bool isKeyboard;
    //    public int controllerID;
    //    public Gamepad gamepad;
    //
    //}

    private void InitialFunc()
    {
        mouse = Mouse.current;
        Debug.Log($"{mouse.displayName} has connected as a PRIMARY_MOUSE to the InputManager.");
        keyboard = Keyboard.current;
        Debug.Log($"{keyboard.displayName} has connected as a PRIMARY_KEYBOARD to the InputManager.");
        gamepadCount = Gamepad.all.Count;

        if(gamepadCount > 0)
        {
            for (int i = 0; i < gamepadCount; i++)
            {
                Debug.Log($"{Gamepad.all[i].displayName} has connected as a GAMEPAD (ID: {i}) to the InputManager.");
            }
        }
    }

    private void Update()
    {
        if(gamepadCount < Gamepad.all.Count)
        {
            for (int i = gamepadCount; i < Gamepad.all.Count; i++)
            {
                Debug.Log($"{Gamepad.all[i].displayName} has connected as a GAMEPAD (ID: {i}) to the InputManager.");
            }
            gamepadCount = Gamepad.all.Count;
        }
    }

    // private Controller[] players = new Controller[2];

    /// <summary>
    /// Check if the player is assigned a controller
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    //public bool IsPlayerAssigned(int id)
    //{
    //    if (players[id].isKeyboard || players[id].gamepad != null)
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    /// <summary>
    /// check if the gamepad is connected otherwise it assigned as null
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    //public bool CheckGampadConnected(int playerid)
    //{
    //    //foreach(Gamepad padAvail in Gamepad.all)
    //    //{
    //    //    if (padAvail == players[playerid].gamepad)
    //    //    {
    //    //        return true;
    //    //    }
    //    //}
    //    //players[playerid] = new Controller(false, 0, null, -1);
    //    //
    //    return false;
    //}

    public int GetAnyGamePad()
    {
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            if(IsAnyGamePadInput(i))
            {
                return i;
            }
        }
        return -1;
    }

    public bool IsAnyGamePadInput(int padID)
    {
        for (int i = 0; i < Enum.GetNames(typeof(ButtonType)).Length; i++)
        {
            if (IsGamepadButtonPressed((ButtonType)i, padID))
            {
                return true;
            }
        }

        for (int j = 0; j < Enum.GetNames(typeof(StickType)).Length; j++)
        {
            if (GetGamepadStick((StickType)j, padID) != Vector2.zero)
            {
                return true;
            }
        }

        return false;
    }

    public ButtonControl GetGamepadButton(ButtonType button, int padID)
    {
        Gamepad pad = (padID < 0 || padID >= Gamepad.all.Count) ? Gamepad.current : Gamepad.all[padID];
        
        if (pad == null)
            return null;

        switch (button)
        {
            case ButtonType.NORTH:     { return pad.buttonNorth; }
            case ButtonType.SOUTH:     { return pad.buttonSouth; }
            case ButtonType.EAST:      { return pad.buttonEast; }
            case ButtonType.WEST:      { return pad.buttonWest; }
            case ButtonType.START:     { return pad.startButton; }
            case ButtonType.SELECT:    { return pad.selectButton; }
            case ButtonType.LT:        { return pad.leftTrigger; }
            case ButtonType.LB:        { return pad.leftShoulder; }
            case ButtonType.LS:        { return pad.leftStickButton; }
            case ButtonType.RT:        { return pad.rightTrigger; }
            case ButtonType.RB:        { return pad.rightShoulder; }
            case ButtonType.RS:        { return pad.rightStickButton; }

            default:
                return null;
        }
    }

    public Vector2 GetGamepadStick(StickType stick, int padID)
    {
        Gamepad pad = (padID < 0 || padID >= Gamepad.all.Count) ? Gamepad.current : Gamepad.all[padID];

        if (pad == null)
            return Vector2.zero;

        switch (stick)
        {
            case StickType.LEFT: { return new Vector2(pad.leftStick.x.ReadValue(), pad.leftStick.y.ReadValue()); }
            case StickType.RIGHT: { return new Vector2(pad.rightStick.x.ReadValue(), pad.rightStick.y.ReadValue()); }

            default:
                return Vector2.zero;
        }
    }

    public bool IsGamepadButtonDown(ButtonType button, int padID)
    {
        ButtonControl control = GetGamepadButton(button, padID);

        if (control == null)
            return false;

        return control.wasPressedThisFrame;
    }

    public bool IsGamepadButtonUp(ButtonType button, int padID)
    {
        ButtonControl control = GetGamepadButton(button, padID);

        if (control == null)
            return false;

        return control.wasReleasedThisFrame;
    }

    public bool IsGamepadButtonPressed(ButtonType button, int padID)
    {
        ButtonControl control = GetGamepadButton(button, padID);

        if (control == null)
            return false;

        return control.isPressed;
    }

    public bool IsGamepadButtonReleased(ButtonType button, int padID)
    {
        ButtonControl control = GetGamepadButton(button, padID);

        if (control == null)
            return false;

        return !(control.isPressed);
    }

    public void cancelController(int _index)
    {
        //if its assigned to keys
        //if (players[_index].isKeyboard)
        //{
        //    if (Keyboard.current.tabKey.wasPressedThisFrame)
        //    {
        //        players[_index] = new Controller(false, 0, null, -1);
        //        Debug.Log("deselect player " + _index.ToString() + " with keyboard");
        //    }
        //    
        //}
        //else
        //{
        //    if(players[_index].gamepad.buttonEast.wasPressedThisFrame)
        //    {
        //        players[_index] = new Controller(false, 0, null, -1);
        //        Debug.Log("deselect player " + _index.ToString() + " with controller");
        //    }
        //    
        //
        //}
    }

    /// <summary>
    /// waits for a confirmation to assign controller
    /// </summary>
    /// <param name="_index"></param>
    //public void confirmController(int _index)
    //{
    //    if (Keyboard.current.spaceKey.wasPressedThisFrame && !PlayerChoseKeyBoard())
    //    {
    //        players[_index] = new Controller(true, _index);            
    //        Debug.Log("confirm player " + _index.ToString() + " with keyboard");
    //    }
    //    else
    //    {
    //        confirmGamepad(_index);
    //    }
    //}

    /// <summary>
    /// comfirming for a gamepad
    /// </summary>
    /// <param name="_index"></param>
    //public void confirmGamepad(int _index)
    //{
    //    Gamepad currentGamepad = Gamepad.current;
    //    if (currentGamepad != null)
    //    {
    //        if (currentGamepad.buttonSouth.wasPressedThisFrame && UnasignedController(currentGamepad))
    //        {
    //
    //            players[_index] = new Controller(false, _index, currentGamepad);
    //
    //            Debug.Log("confirm player " + _index.ToString() + " with Controller");
    //            
    //        }
    //    }
    //}

    /// <summary>
    /// check if this the player controls are assigned both from the key and the gamepad
    /// </summary>
    /// <param name="_gamepad"></param>
    /// <returns></returns>
    //public bool UnasignedController(Gamepad _gamepad)
    //{
    //    foreach(Controller player in players)
    //    {
    //        if(player.isKeyboard )
    //        {
    //            return true;
    //        }
    //        if(player.gamepad == _gamepad)
    //        {
    //            return false;
    //        }
    //    }
    //    return true;
    //    
    //}
    //end of lobby selection
    //-----------------------------------------------------------------------------

    public bool IsAnyKeyPressed()
    {
        return keyboard.anyKey.isPressed;
    }

    public KeyControl GetKey(KeyType key)
    {
        switch (key)
        {
            case KeyType.Q: { return keyboard.qKey; }
            case KeyType.W: { return keyboard.wKey; }
            case KeyType.E: { return keyboard.eKey; }
            case KeyType.R: { return keyboard.rKey; }
            case KeyType.T: { return keyboard.tKey; }
            case KeyType.Y: { return keyboard.yKey; }
            case KeyType.U: { return keyboard.uKey; }
            case KeyType.I: { return keyboard.iKey; }
            case KeyType.O: { return keyboard.oKey; }
            case KeyType.P: { return keyboard.pKey; }

            case KeyType.A: { return keyboard.aKey; }
            case KeyType.S: { return keyboard.sKey; }
            case KeyType.D: { return keyboard.dKey; }
            case KeyType.F: { return keyboard.fKey; }
            case KeyType.G: { return keyboard.gKey; }
            case KeyType.H: { return keyboard.hKey; }
            case KeyType.J: { return keyboard.jKey; }
            case KeyType.K: { return keyboard.kKey; }
            case KeyType.L: { return keyboard.lKey; }

            case KeyType.Z: { return keyboard.zKey; }
            case KeyType.X: { return keyboard.xKey; }
            case KeyType.C: { return keyboard.cKey; }
            case KeyType.V: { return keyboard.vKey; }
            case KeyType.B: { return keyboard.bKey; }
            case KeyType.N: { return keyboard.nKey; }
            case KeyType.M: { return keyboard.mKey; }

            case KeyType.NUM_ONE:     { return keyboard.numpad1Key; }
            case KeyType.NUM_TWO:     { return keyboard.numpad2Key; }
            case KeyType.NUM_THREE:   { return keyboard.numpad3Key; }
            case KeyType.NUM_FOUR:    { return keyboard.numpad4Key; }
            case KeyType.NUM_FIVE:    { return keyboard.numpad5Key; }
            case KeyType.NUM_SIX:     { return keyboard.numpad6Key; }
            case KeyType.NUM_SEVEN:   { return keyboard.numpad7Key; }
            case KeyType.NUM_EIGHT:   { return keyboard.numpad8Key; }
            case KeyType.NUM_NINE:    { return keyboard.numpad9Key; }
            case KeyType.NUM_ZERO: { return keyboard.numpad0Key; }

            case KeyType.L_SHIFT: { return keyboard.leftShiftKey; }
            case KeyType.L_CTRL: { return keyboard.leftCtrlKey; }
            case KeyType.L_ALT: { return keyboard.leftAltKey; }
            case KeyType.TAB: { return keyboard.tabKey; }
            case KeyType.ESC: { return keyboard.escapeKey; }

            case KeyType.R_SHIFT: { return keyboard.rightShiftKey; }
            case KeyType.R_CTRL: { return keyboard.rightCtrlKey; }
            case KeyType.R_ALT: { return keyboard.rightAltKey; }
            case KeyType.ENTER: { return keyboard.endKey; }

            default:
                return null;
        }
    }

    public bool IsKeyDown(KeyType key)
    {
        return GetKey(key).wasPressedThisFrame;
    }

    public bool IsKeyUp(KeyType key)
    {
        return GetKey(key).wasReleasedThisFrame;
    }

    public bool IsKeyPressed(KeyType key)
    {
        return GetKey(key).isPressed;
    }
    public bool IsKeyReleased(KeyType key)
    {
        return !(GetKey(key).isPressed);
    }

    public bool IsAnyMousePressed()
    {
        return mouse.press.isPressed;
    }

    public ButtonControl GetMouseButton(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.LEFT:      { return mouse.leftButton; }
            case MouseButton.MIDDLE:    { return mouse.middleButton; }
            case MouseButton.RIGHT:     { return mouse.rightButton; }
           
            default:
                return null;
        }
    }

    public bool GetMouseButtonDown(MouseButton button)
    {
        return GetMouseButton(button).wasPressedThisFrame;
    }

    public bool GetMouseUp(MouseButton button)
    {
        return GetMouseButton(button).wasReleasedThisFrame;
    }

    public bool GetMousePressed(MouseButton button)
    {
        return GetMouseButton(button).isPressed;
    }

    public bool GetMouseReleased(MouseButton button)
    {
        return !(GetMouseButton(button).isPressed);
    }

    public Vector2 GetMouseDelta()
    {
        return new Vector2(mouse.delta.x.ReadValue(), mouse.delta.y.ReadValue());
    }

    /// <summary>
    /// When the controller button are being pressed
    /// </summary>
    /// <param name="buttonName"></param>
    /// <param name="playerIndex"></param>
    /// <returns></returns>

    public bool GetKeyPressed(ButtonType buttonName, int playerIndex)
    {
        //if (players[playerIndex].isKeyboard)
        //{
        //    switch (buttonName)
        //    {
        //        default:
        //            Debug.LogWarning($"Unsupported key type in GetKeyDown.");
        //            return false;
        //        case ButtonType.BUTTON_NORTH:
        //            return keyboard.wKey.isPressed;
        //
        //        case ButtonType.BUTTON_EAST:
        //            return keyboard.dKey.isPressed;
        //
        //        case ButtonType.BUTTON_WEST:
        //            return keyboard.aKey.isPressed;
        //
        //        case ButtonType.BUTTON_SOUTH:
        //            return keyboard.sKey.isPressed;
        //
        //        case ButtonType.BUTTON_START:
        //            return keyboard.spaceKey.isPressed;
        //
        //        case ButtonType.BUTTON_SELECT:
        //            return keyboard.tabKey.isPressed;
        //
        //        case ButtonType.BUTTON_LT:
        //            return mouse.rightButton.isPressed;
        //
        //        case ButtonType.BUTTON_RT:
        //            return mouse.leftButton.isPressed;
        //
        //        case ButtonType.BUTTON_LS:
        //            return keyboard.qKey.isPressed;
        //
        //        case ButtonType.BUTTON_RS:
        //            return keyboard.eKey.isPressed;
        //
        //    }
        //
        //
        //}
        //else
        //{
        //    Gamepad pad = players[playerIndex].gamepad; //Gamepad.all[players[playerIndex].controllerID];
        //    if (pad == null)
        //    {
        //        return false;
        //
        //    }
        //
        //    switch (buttonName)
        //    {
        //        default:
        //            Debug.LogWarning($"Unsupported button type in GetKeyPress.");
        //            return false;
        //        case ButtonType.BUTTON_NORTH:
        //            return pad.buttonNorth.isPressed;
        //
        //        case ButtonType.BUTTON_EAST:
        //            return pad.buttonEast.isPressed;
        //
        //        case ButtonType.BUTTON_WEST:
        //            return pad.buttonWest.isPressed;
        //
        //        case ButtonType.BUTTON_SOUTH:
        //            return pad.buttonSouth.isPressed;
        //
        //        case ButtonType.BUTTON_START:
        //            return pad.startButton.isPressed;
        //
        //        case ButtonType.BUTTON_SELECT:
        //            return pad.selectButton.isPressed;
        //
        //        case ButtonType.BUTTON_LT:
        //            return pad.leftTrigger.isPressed;
        //
        //        case ButtonType.BUTTON_RT:
        //            return pad.rightTrigger.isPressed;
        //
        //        case ButtonType.BUTTON_LS:
        //            return pad.leftShoulder.isPressed;
        //
        //        case ButtonType.BUTTON_RS:
        //            return pad.rightShoulder.isPressed;
        //
        //    }
        //
        //}
        return false;
    }

    //public bool GetStickDirection(StickDirection direction, int playerIndex)
    //{
    //    //if (players[playerIndex].isKeyboard)
    //    //{
    //    //    switch (direction)
    //    //    {
    //    //        default:
    //    //            Debug.LogWarning($"Unsupported key type in GetStickDirection.");
    //    //            return false;
    //    //        case StickDirection.UP:
    //    //            return keyboard.wKey.wasPressedThisFrame;
    //    //
    //    //        case StickDirection.DOWN:
    //    //            return keyboard.sKey.wasPressedThisFrame;
    //    //
    //    //        case StickDirection.LEFT:
    //    //            return keyboard.aKey.wasPressedThisFrame;
    //    //
    //    //        case StickDirection.RIGHT:
    //    //            return keyboard.dKey.wasPressedThisFrame;
    //    //
    //    //
    //    //    }
    //    //
    //    //
    //    //}
    //    //else
    //    //{
    //    //    Gamepad pad = players[playerIndex].gamepad; //Gamepad.all[players[playerIndex].controllerID];
    //    //    if (pad == null)
    //    //    {
    //    //        pad = Gamepad.current;
    //    //        //return false;
    //    //
    //    //    }
    //    //    if (pad == null)
    //    //    {
    //    //        
    //    //        return false;
    //    //
    //    //    }
    //    //
    //    //    switch (direction)
    //    //    {
    //    //        default:
    //    //            Debug.LogWarning($"Unsupported button type in GetStickDirection.");
    //    //            return false;
    //    //        case StickDirection.UP:
    //    //            return pad.leftStick.up.wasPressedThisFrame || pad.dpad.up.wasPressedThisFrame;
    //    //
    //    //        case StickDirection.DOWN:
    //    //            return pad.leftStick.down.wasPressedThisFrame || pad.dpad.down.wasPressedThisFrame;
    //    //
    //    //        case StickDirection.LEFT:
    //    //            return pad.leftStick.left.wasPressedThisFrame || pad.dpad.left.wasPressedThisFrame;
    //    //
    //    //        case StickDirection.RIGHT:
    //    //            return pad.leftStick.right.wasPressedThisFrame || pad.dpad.right.wasPressedThisFrame;
    //    //
    //    //    }
    //    //
    //    //}
    //    return false;
    //}

    /// <summary>
    /// Getting the vertical axis value from controller
    /// </summary>
    /// <param name="joystick"></param>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
    public float GetVerticalAxis(int playerIndex, Camera playercamera = null)
    {
        //if (players[playerIndex].isKeyboard)
        //{
        //    switch (joystick)
        //    {
        //        default:
        //            Debug.LogWarning($"Unsupported key type in GetVerticalAxis.");
        //            return 0;
        //        case Joysticks.LEFT:
        //            return GetVerticalAxis();
        //
        //        case Joysticks.RIGHT:
        //            return GetMouseVertAxis(playercamera);
        //    }
        //    
        //}
        //else
        //{
        //    Gamepad pad = players[playerIndex].gamepad; //Gamepad.all[players[playerIndex].controllerID];
        //    if (pad == null)
        //    {
        //        return 0;
        //
        //    }
        //
        //    switch (joystick)
        //    {
        //        default:
        //            Debug.LogWarning($"Unsupported button type in GetVerticalAxis.");
        //            return 0;
        //        case Joysticks.LEFT:
        //            return pad.leftStick.y.ReadValue();
        //
        //        case Joysticks.RIGHT:
        //            return pad.rightStick.y.ReadValue();
        //    }
        //}
        return 0.0f;
    }

    /// <summary>
    /// Getting the Horizontall Axis value from controller
    /// </summary>
    /// <param name="joystick"></param>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
    public float GetHorizontalAxis(int playerIndex, Camera playercamera = null)
    {
        //if (players[playerIndex].isKeyboard)
        //{
        //    switch (joystick)
        //    {
        //        default:
        //            Debug.LogWarning($"Unsupported key type in GetHorizontalAxis.");
        //            return 0;
        //        case Joysticks.LEFT:
        //            return GetHorizontalAxis();
        //
        //        case Joysticks.RIGHT:
        //            return GetMouseHortAxis(playercamera);
        //    }
        //
        //}
        //else
        //{
        //    Gamepad pad = players[playerIndex].gamepad; //Gamepad.all[players[playerIndex].controllerID];
        //    if (pad == null)
        //    {
        //        return 0;
        //
        //    }
        //
        //    switch (joystick)
        //    {
        //        default:
        //            Debug.LogWarning($"Unsupported button type in GetHorizontalAxis.");
        //            return 0;
        //        case Joysticks.LEFT:
        //            return pad.leftStick.x.ReadValue();
        //
        //        case Joysticks.RIGHT:
        //            return pad.rightStick.x.ReadValue();
        //    }
        //}
        return 0.0f;
    }

    /// <summary>
    /// Getting the vertical axis value from keyboard
    /// </summary>
    /// <param name="joystick"></param>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
    public float GetVerticalAxis()
    {
        if(keyboard.wKey.isPressed)
        {
            return 1.0f;
        }
        else if (keyboard.sKey.isPressed)
        {
            return -1.0f;
        }
        else
        {
            return 0.0f;
        }


        
    }

    /// <summary>
    /// Getting the Horizontall Axis value from keyboard
    /// </summary>
    /// <param name="joystick"></param>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
    public float GetHorizontalAxis()
    {
        if (keyboard.dKey.isPressed)
        {
            return 1.0f;
        }
        else if (keyboard.aKey.isPressed)
        {
            return -1.0f;
        }
        else
        {
            return 0.0f;
        }
    }
    /// <summary>
    /// Checking if the mouse button is down
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public bool GetMouseDown(MouseButton button)
    {
        switch (button)
        {
            default:
                Debug.LogWarning($"Unsupported mouse button type in GetMouseDown.");
                return false;
            case MouseButton.LEFT:
                return mouse.rightButton.wasPressedThisFrame;

            case MouseButton.RIGHT:
                return mouse.leftButton.wasPressedThisFrame;
        }

    }
    /// <summary>
    /// Checking if the mouse button is pressed
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public bool GetMousePress(MouseButton button)
    {

        switch (button)
        {
            default:
                Debug.LogWarning($"Unsupported mouse button type in GetMouseDown.");
                return false;
            case MouseButton.LEFT:
                return mouse.rightButton.isPressed;

            case MouseButton.RIGHT:
                return mouse.leftButton.isPressed;
        }
    }

    /// <summary>
    /// Get the mouse Vertical axis
    /// </summary>
    /// <returns></returns>
    public float GetMouseVertAxis(Camera playerCam)
    {
        if (playerCam != null)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            mousePos.z = playerCam.farClipPlane;
            Vector3 worldPoint = playerCam.ScreenToWorldPoint(mousePos);
            Vector3 direct = worldPoint - new Vector3(playerCam.transform.position.x, playerCam.transform.position.y, worldPoint.z);
            
            direct = direct / 800.0f;
            if (direct.magnitude > 1.0f)
                direct = direct.normalized * 1.0f;

            direct.z = 0;
            return direct.y;
        }
        return 0;

    }
    /// <summary>
    /// Get the mouse Horizontal axis
    /// </summary>
    /// <returns></returns>
    public float GetMouseHortAxis(Camera playerCam)
    {
        if (playerCam != null)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            mousePos.z = playerCam.farClipPlane;
            Vector3 worldPoint = playerCam.ScreenToWorldPoint(mousePos);
            Vector3 direct = worldPoint - new Vector3(playerCam.transform.position.x, playerCam.transform.position.y, worldPoint.z);

            direct = direct / 800.0f;
            if (direct.magnitude > 1.0f)
                direct = direct.normalized * 1.0f;

            direct.z = 0;
            return direct.x;
        }
        return 0;

    }
}
