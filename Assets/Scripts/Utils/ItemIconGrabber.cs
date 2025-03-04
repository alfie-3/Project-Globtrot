using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;


public class ItemIconGrabber : MonoBehaviour
{
    [SerializeField] private Camera cam;

    public List<ScreenShotAsset> sceneObects;

    [SerializeField] string pathFolder;

    private void Start()
    {
        cam.backgroundColor = Color.black;
    }

    [ContextMenu("Screenshot")]
    private void ProcessScreenshots()
    {
        StartCoroutine(Screenshot());
    }

    private IEnumerator Screenshot()
    {
        foreach (var item in sceneObects)
        {
            item.go.SetActive(false);
        }

        for (int i = 0; i < sceneObects.Count; i++)
        {
            if (!sceneObects[i].capture) continue;

            GameObject obj = sceneObects[i].go;
            ItemBase data = sceneObects[i].item;

            obj.gameObject.SetActive(true);

            Vector3 newCamLocalPos = new(0, 0, -obj.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds.max.magnitude * 1.4f);
            cam.transform.localPosition = newCamLocalPos;

            TakeScreenshot($"{Application.dataPath}/{pathFolder}/{data.ItemID}_Icon.png");
            obj.gameObject.SetActive(false);

            Debug.Log($"Assets/{pathFolder}/{data.ItemID}_Icon.png");

            yield return new WaitForSeconds(0.2f);

            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/{pathFolder}/{data.ItemID}_Icon.png");

            data.SetIcon(s);
            EditorUtility.SetDirty(data);

            yield return null;
        }

        foreach (var item in sceneObects)
        {
            item.go.SetActive(true);
        }

        yield return null;
    }

    void TakeScreenshot(string fullPath)
    {

        RenderTexture rt = new RenderTexture(256, 256, 24);
        cam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        cam.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        cam.targetTexture = null;
        RenderTexture.active = null;

        Color colortrigger = Color.black;

        if (Application.isEditor)
        {
            DestroyImmediate(rt);
        }
        else
        {
            Destroy(rt);
        }

        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}

[System.Serializable]
public class ScreenShotAsset
{
    public GameObject go;
    public ItemBase item;
    public bool capture = true;
}