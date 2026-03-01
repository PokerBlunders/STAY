using UnityEngine;

public class CursorSetting : MonoBehaviour
{
    [SerializeField] private Texture2D cursor;
    private Vector2 cursorHotSpot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            cursorHotSpot = new Vector2(cursorHotSpot.x, cursorHotSpot.y);
            Cursor.SetCursor(cursor, cursorHotSpot, CursorMode.Auto);
        }
        if (Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

}
