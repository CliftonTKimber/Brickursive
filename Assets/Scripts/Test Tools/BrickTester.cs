using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Assertions.Must;
using static GameConfig;

public class BrickTester : MonoBehaviour
{
    // Start is called before the first frame update

    public List<GameObject> bricksToTest;
    void Start()
    {

        for(int i = 0; i < bricksToTest.Count; i++)
        {
            GameObject theBrickToTest = bricksToTest[i];
            RunThroughTests(theBrickToTest);
        }

        UnityEditor.EditorApplication.isPlaying = false;
    }


    private void RunThroughTests(GameObject brickToTest)
    {
        Assert.IsNotNull(brickToTest);

        Debug.Log("\n Commencing tests for: " + brickToTest.name + "\n");

        Assert.AreEqual(BASE_BRICK_TAG, brickToTest.tag);
        Assert.AreEqual(BRICK_LAYER_MASK, brickToTest.layer);

        TestMesh(brickToTest);
        
        RunSocketTests(brickToTest);
    }

    private void TestMesh(GameObject brickToTest)
    {
        MeshFilter meshFilter = brickToTest.GetComponent<MeshFilter>();
        Assert.IsNotNull(meshFilter);

        Mesh mesh = meshFilter.sharedMesh;
        Assert.IsNotNull(mesh);

        Bounds bounds = mesh.bounds;

        //Does the mesh fit within the Base Cell Grid?
        float xSizeDifference = (bounds.size.x * 100f) % (BASE_CELL_SIZE.x * 100f);
        float ySizeDifference = (bounds.size.y * 100f) % (BASE_CELL_SIZE.y * 100f);
        float zSizeDifference = (bounds.size.z * 100f) % (BASE_CELL_SIZE.z * 100f);

        /*xSizeDifference %= (STUD_HEIGHT * 100f);
        xSizeDifference %= (STUD_HEIGHT * 100f);
        xSizeDifference %= (STUD_HEIGHT * 100f);*/

        xSizeDifference = Mathf.Round(xSizeDifference);
        ySizeDifference = Mathf.Round(ySizeDifference);
        zSizeDifference = Mathf.Round(zSizeDifference);

        xSizeDifference /= 100f;
        ySizeDifference /= 100f;
        zSizeDifference /= 100f;

        Debug.Log("Mesh X size is: " + xSizeDifference + " off. Check for Studs, and if the difference is near a full cell size.");
        Debug.Log("Mesh Y size is: " + ySizeDifference + " off. Check for Studs, and if the difference is near a full cell size.");
        Debug.Log("Mesh Z size is: " + zSizeDifference + " off. Check for Studs, and if the difference is near a full cell size.");
        

        /// Should check for UV's as well

    }

    private void RunSocketTests(GameObject brickToTest)
    {

        for(int i = 0; i < brickToTest.transform.childCount; i++)
        {
            GameObject child = brickToTest.transform.GetChild(i).gameObject;

            if(!child.CompareTag(SOCKET_TAG_FEMALE) && !child.CompareTag(SOCKET_TAG_MALE))
            {
                Debug.LogAssertion(child.name + "'s tag is: " + child.tag + ", when it should be Male or Female.");
                continue;
            }

            SocketHasCorrectLayerMask(child);
            SocketIsNotUpsideDown(child);

        }

    }


    private void SocketHasCorrectLayerMask(GameObject socket)
    {
        if(socket.layer != SOCKET_LAYER_MASK)
        {
            Debug.LogAssertion("Socket has a Layer Mask of: " + LayerMask.LayerToName(socket.layer) +
             " When it should be: " + LayerMask.LayerToName(SOCKET_LAYER_MASK));
        }
    }


    private void SocketIsNotUpsideDown(GameObject socket)
    {
        if(!socket.CompareTag(SOCKET_TAG_FEMALE))
        {
            return;
        }

        //There may be special cases where this is not true;

        if(Mathf.Abs(socket.transform.localEulerAngles.x) == 180)
        {
            Debug.LogAssertion(socket.name + " is flipped over it's X-axis, and will cause errors in brick placement." + 
            " Keep rotation between -180 -> 180");
        }
        if(Mathf.Abs(socket.transform.localEulerAngles.y) == 180)
        {
            Debug.LogAssertion(socket.name + " is flipped over it's Y-axis, and will cause errors in brick placement." + 
            " Keep rotation between -180 -> 180");
        }
        if(Mathf.Abs(socket.transform.localEulerAngles.z) == 180)
        {
            Debug.LogAssertion(socket.name + " is flipped over it's Z-axis, and will cause errors in brick placement." + 
            " Keep rotation between -180 -> 180");
        }
    }

}
