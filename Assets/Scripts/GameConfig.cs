using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GameConfig 
{


    public static readonly string BASE_BRICK_TAG = "Brick";
    public static readonly string SOCKET_TAG_MALE = "Male";
    public static readonly string SOCKET_TAG_FEMALE = "Female";

    public static readonly Vector3 BASE_CELL_SIZE = new(0.78f, 0.32f, 0.78f); 

    public static readonly float STUD_HEIGHT = 0.17f;

    public static readonly float RAY_LENGTH_FOR_SNAPPING = 0.2f;
    public static readonly float RAY_LENGTH_FOR_GHOST_SNAPPING = 6f;

    //public static readonly float RAY_LENGTH_FOR_BRICK_SELECTION = 60f;

    public static readonly float PICKUP_GRACE_TIME = 0.3f;

    public static readonly int BRICK_LAYER;

    public static readonly string GHOST_BRICK_NAME = "Ghost Brick";

    public static readonly string OBJECT_FOLDER_NAME = "Objects";

    public static readonly InteractionLayerMask LAYER_MASK_ONLY_PLUCKABLE = InteractionLayerMask.NameToLayer("onlyPluckable");

    public static readonly InteractionLayerMask LAYER_MASK_INTERACT = InteractionLayerMask.NameToLayer("Interact");

    //12FPS to match with typical animations
    public static readonly float ANIMATION_UPDATE_TIME = 0.0833333333333f;


    
}
