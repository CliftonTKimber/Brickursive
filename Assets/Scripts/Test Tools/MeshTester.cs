using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using static GameConfig;

public class MeshTester : MonoBehaviour
{

    /// <summary>
    /// NOTE: application does not permanently alter mesh.
    /// </summary>
    public bool ForceMeshChange = false;
    public List<Mesh> meshes;
    // Start is called before the first frame update

    public bool setMeshManually = false;

    public Vector3 meshSize;
    void Start()
    {

        DoAllMeshTests();

        UnityEditor.EditorApplication.isPlaying = false;
        
    }

    private void DoAllMeshTests()
    {
        if(meshes == null || meshes.Count == 0)
        {
            return;
        }

        for(int i = 0; i < meshes.Count; i++)
        {
            Mesh subjectMesh = meshes[i];
            if(subjectMesh == null)
            {
                continue;
            }

            SimpleMeshCleanup(subjectMesh);
            TestMeshForCorrectSize(subjectMesh);
            SetMeshManually(subjectMesh);


        }


    }


    private void SimpleMeshCleanup(Mesh mesh)
    {

        float xSize = mesh.bounds.size.x;
        float ySize = mesh.bounds.size.y;
        float zSize = mesh.bounds.size.z;

        xSize *= 100;
        xSize = Mathf.Round(xSize);
        xSize /= 100;

        ySize *= 100;
        ySize = Mathf.Round(ySize);
        ySize /= 100;

        zSize *= 100;
        zSize = Mathf.Round(zSize);
        zSize /= 100;


        Vector3 newSize = new(xSize,
                              ySize,
                              zSize);
        Bounds newBounds = new(Vector3.zero, newSize);


        mesh.bounds = newBounds;

        Debug.Log("Set mesh " + mesh.name + " to new size of: " + newSize);

    }

    private void SetMeshManually(Mesh mesh)
    {

        if(!setMeshManually)
        {
            return;
        }

        Bounds newBounds = new(Vector3.zero, meshSize);


        mesh.bounds = newBounds;

        Debug.Log("Set mesh " + mesh.name + " to new size of: " + meshSize);

    }

    private void TestMeshForCorrectSize(Mesh mesh)
    {

        float xSize = mesh.bounds.size.x;
        float ySize = mesh.bounds.size.y;
        float zSize = mesh.bounds.size.z;


        float xRemainder = xSize % BASE_CELL_SIZE.x;
        if(xRemainder > 0.001f)
        {
            //R = .02 -> Subrtract |  R = .08 -> add

            Debug.Log("incorrect X Size of " + xRemainder +", accounting for possible extra studs.");
            xRemainder = xRemainder % STUD_HEIGHT;
            if(xRemainder > 0.001f)
            {
                Debug.Log("mesh: " + mesh.name +  " has an incorrect X Size. Remainder: " + xRemainder);

                

            }
            else
            {
                xRemainder = 0f;
            }
        }

        float yRemainder = ySize % BASE_CELL_SIZE.y;
        if(yRemainder > 0.001f)
        {
            Debug.Log("incorrect Y Size of " + yRemainder +", accounting for possible extra studs.");
            yRemainder = yRemainder % STUD_HEIGHT;
            if(yRemainder > 0.001f)
            {
                Debug.Log("mesh: " + mesh.name +  " has an incorrect Y Size. Remainder: " + yRemainder);
            }
            else
            {
                yRemainder = 0f;
            }
        }

        float zRemainder = zSize % BASE_CELL_SIZE.z;
        if(zRemainder > 0.001f)
        {
            Debug.Log("incorrect Z Size of " + zRemainder +", accounting for possible extra studs.");
            zRemainder = zRemainder % STUD_HEIGHT;
            if(zRemainder > 0.001f)
            {
                Debug.Log("mesh: " + mesh.name +  " has an incorrect Z Size. Remainder: " + zRemainder);
            }
            else
            {
                yRemainder = 0f;
            }
        }







        if(ForceMeshChange)
        {
            if(xRemainder < 0.001f && yRemainder < 0.001f && zRemainder < 0.001f)
            {
                Debug.Log("Size change is negligible. No changes will be made.");
                return;
            }

            //Round Up to numbers instead of default Down.
            //Only works for Stud checks
            if(xRemainder >= STUD_HEIGHT / 2 && xRemainder < STUD_HEIGHT)
            {
                xRemainder -= STUD_HEIGHT;
            }

            if(yRemainder >= STUD_HEIGHT / 2 && yRemainder < STUD_HEIGHT)
            {
                yRemainder -= STUD_HEIGHT;
            }

            if(zRemainder >= STUD_HEIGHT / 2 && zRemainder < STUD_HEIGHT)
            {
                zRemainder -= STUD_HEIGHT;
            }


            
            Vector3 newSize = new(xSize - xRemainder,
                                  ySize - yRemainder,
                                  zSize - zRemainder);
            Bounds newBounds = new(Vector3.zero, newSize);

            Debug.Log("ForceMeshChange is " + ForceMeshChange + ", altering mesh into: " + newBounds.size);



            mesh.bounds = newBounds;

        }


    }

}
