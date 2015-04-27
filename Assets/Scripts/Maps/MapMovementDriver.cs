using UnityEngine;

public class MapMovementDriver : MapDebuggableBehaviour {
    #region Static Code
    private static bool movementEnabled;
    public static bool MovementEnabled {
        get { return movementEnabled; }
        set { movementEnabled = value; }
    }

    private static KeyBinding moveDown;
    private static KeyBinding moveRight;
    private static KeyBinding moveUp;
    private static KeyBinding moveLeft;
    private static KeyBinding zoomIn;
    private static KeyBinding zoomOut;

    public static KeyBinding MoveDown { get { return moveDown; } }
    public static KeyBinding MoveRight { get { return moveRight; } }
    public static KeyBinding MoveUp { get { return moveUp; } }
    public static KeyBinding MoveLeft { get { return moveLeft; } }
    public static KeyBinding ZoomIn { get { return zoomIn; } }
    public static KeyBinding ZoomOut { get { return zoomOut; } }

    static MapMovementDriver() {
        moveDown = new KeyBinding(KeyCode.DownArrow, KeyCode.S);
        moveRight = new KeyBinding(KeyCode.RightArrow, KeyCode.D);
        moveUp = new KeyBinding(KeyCode.UpArrow, KeyCode.W);
        moveLeft = new KeyBinding(KeyCode.LeftArrow, KeyCode.A);
        zoomIn = new KeyBinding(KeyCode.PageUp, KeyCode.Equals, KeyCode.E);
        zoomOut = new KeyBinding(KeyCode.PageDown, KeyCode.Minus, KeyCode.Q);
    }
    #endregion Static Code

    #region Instance/Unity
    public float movementModifier;

    void Start() {
        movementEnabled = true;
    }

    void Update() {
        if (movementEnabled) {
            try {
                Vector3 movement = Vector3.zero;

                movement += (MoveDown.Held ? movementModifier : 0) * Vector3.down;
                movement += (MoveRight.Held ? movementModifier : 0) * Vector3.right;
                movement += (MoveUp.Held ? movementModifier : 0) * Vector3.up;
                movement += (MoveLeft.Held ? movementModifier : 0) * Vector3.left;
                movement += (ZoomIn.Held ? movementModifier : 0) * Vector3.back;
                movement += (ZoomOut.Held ? movementModifier : 0) * Vector3.forward;

                MapMovementAPI.MoveMap(movement);
            } catch {
                Debug("Exeception thrown.");
            }
        }
    }
    #endregion Instance/Unity
}
