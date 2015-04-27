public class UIFrameDistributor : MapDebuggableBehaviour {
    //public ScenarioEditor ScenarioEditor;
    public Editor MapEditor;

    public void OnMouseDown() {
        //ScenarioEditor.OnMouseDown();
        MapEditor.OnMouseDown();
    }

    public void OnMouseUp() {
        MapEditor.OnMouseUp();
    }

    public void OnMouseDrag() {
        MapEditor.OnMouseDrag();
        //ScenarioEditor.OnMouseDrag();
    }
}
