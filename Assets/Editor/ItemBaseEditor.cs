using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(ItemBase))]
public class ItemBaseEditor : Editor
{
    //https://www.youtube.com/watch?v=lttDfhw7jhM

    public ItemBase Item { get { return target as ItemBase; } }

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        if (Item.ItemIcon != null)
        {
            Type t = GetType("UnityEditor.SpriteUtility");
            if (t != null)
            {
                MethodInfo method = t.GetMethod("RenderStaticPreview", new[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) });
                if (method != null)
                {
                    object ret = method.Invoke("RenderStaticPreview", new object[] { Item.ItemIcon, Color.white, width, height });
                    if (ret is Texture2D)
                        return ret as Texture2D;
                }
            }
        }
        return base.RenderStaticPreview(assetPath, subAssets, width, height);
    }

    private static Type GetType(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type != null)
            return type;

        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
            {
                type = assembly.GetType(typeName);
                if (type != null)
                    return type;
            }
        }
        return null;
    }
}