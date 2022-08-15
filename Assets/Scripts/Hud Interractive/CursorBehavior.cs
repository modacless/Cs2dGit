using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorState
{
    BasicCursor,
    CursorShoot,
    CursorClicked,
}

public class CursorBehavior : MonoBehaviour
{

    //References
    [System.Serializable]
    public struct AllCursors
    {
        public string name;
        public GameObject cursor;
    }
    [Header("References")]
    [SerializeField]
    private ScriptablePlayerData playerData;

    [SerializeField]
    private GameObject cursorObjects;

    [SerializeField]
    private AllCursors[] allCursorsArray; //Use allCursors

    //Parameter
    public Dictionary<string, GameObject> allCursors = new Dictionary<string, GameObject>(); // use this to choose cursor




    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        for(int i = 0; i < allCursorsArray.Length; i++)
        {
            allCursors.Add(allCursorsArray[i].name, allCursorsArray[i].cursor);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 CursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorObjects.transform.position = CursorPosition;
    }

    static void ChangeCursor()
    {

    }
}
