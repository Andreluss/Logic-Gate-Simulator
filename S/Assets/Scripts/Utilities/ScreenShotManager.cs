using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotManager : MonoBehaviour
{
    public void SaveScreenShot(int projectID)
    {
        //StartCoroutine(Save(projectID));
    }
    IEnumerator Save(int projectID)
    {
        // We should only read the screen after all rendering is complete
        yield return new WaitForEndOfFrame();

        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width * 3/4;
        int height = Screen.height * 3/4;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read screen contents into the texture
        tex.ReadPixels(new Rect(width / 8, height / 8 , width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToJPG(20);
        Destroy(tex);

        try
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + $"/thumb");
            System.IO.File.WriteAllBytes(Application.dataPath + $"/thumb/thumb{projectID}.jpg", bytes);
            Debug.Log("succesfully saved");
        }
        catch {
            Debug.LogError("Saving failed");
        }

        yield break;
    }

}
