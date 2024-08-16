using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig 
{


    public static readonly string BASE_BRICK_TAG = "Brick";
    public static readonly string SOCKET_TAG_MALE = "Male";
    public static readonly string SOCKET_TAG_FEMALE = "Female";

    public static readonly Vector3 BASE_CELL_SIZE = new(0.78f, 0.32f, 0.78f); 

    public static readonly float RAY_LENGTH_FOR_SNAPPING = 0.2f;
    public static readonly float RAY_LENGTH_FOR_GHOST_SNAPPING = 6f;

    public static readonly float RAY_LENGTH_FOR_BRICK_SELECTION = 60f;

    public static readonly int BRICK_LAYER;

    public static readonly string GHOST_BRICK_NAME = "Ghost Brick";

    public static readonly string OBJECT_FOLDER_NAME = "Objects";


    
}
