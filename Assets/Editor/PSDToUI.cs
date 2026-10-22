
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

public static class PSDToUI
{
    [MenuItem("Tools/PSD To UI Images")]
    static void Convert()
    {
        GameObject source = Selection.activeGameObject;
        Canvas canvas = Object.FindObjectOfType<Canvas>();

        if (source == null || canvas == null)
        {
            Debug.LogError("Select a PSD prefab and create a Canvas first.");
            return;
        }

        var sprites = source.GetComponentsInChildren<SpriteRenderer>(true)
            .Where(s => s.sprite != null)
            .OrderBy(s => SortingLayer.GetLayerValueFromID(s.sortingLayerID))
            .ThenBy(s => s.sortingOrder)
            .ToArray();

        if (sprites.Length == 0)
        {
            Debug.LogError("No SpriteRenderers found.");
            return;
        }

        float ppu = sprites[0].sprite.pixelsPerUnit;

        GameObject root = new GameObject(source.name + "_UI",
            typeof(RectTransform));

        Undo.RegisterCreatedObjectUndo(root, "PSD To UI");
        RectTransform parent = (RectTransform)root.transform;
        parent.SetParent(canvas.transform, false);
        parent.anchorMin = parent.anchorMax = Vector2.one * 0.5f;
        parent.anchoredPosition = Vector2.zero;
        parent.sizeDelta = Vector2.zero;

        foreach (var sr in sprites)
        {
            Sprite sprite = sr.sprite;
            GameObject obj = new GameObject(sr.name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));

            Undo.RegisterCreatedObjectUndo(obj, "Create UI Image");

            RectTransform rect = (RectTransform)obj.transform;
            rect.SetParent(parent, false);

            Matrix4x4 matrix =
                source.transform.worldToLocalMatrix *
                sr.transform.localToWorldMatrix;

            Vector3 pos = matrix.MultiplyPoint3x4(Vector3.zero);
            Vector3 right = matrix.MultiplyVector(Vector3.right);
            Vector3 up = matrix.MultiplyVector(Vector3.up);

            rect.anchorMin = rect.anchorMax = Vector2.one * 0.5f;
            rect.pivot = new Vector2(
                sprite.pivot.x / sprite.rect.width,
                sprite.pivot.y / sprite.rect.height
            );

            rect.sizeDelta = sprite.rect.size *
                (ppu / sprite.pixelsPerUnit);

            rect.anchoredPosition = new Vector2(pos.x, pos.y) * ppu;
            rect.localRotation = Quaternion.Euler(
                0, 0, Mathf.Atan2(right.y, right.x) * Mathf.Rad2Deg
            );
            rect.localScale = new Vector3(
                right.magnitude * (sr.flipX ? -1 : 1),
                up.magnitude * (sr.flipY ? -1 : 1),
                1
            );

            Image image = obj.GetComponent<Image>();
            image.sprite = sprite;
            image.color = sr.color;
            image.raycastTarget = false;

            obj.SetActive(sr.gameObject.activeInHierarchy);
        }

        Selection.activeGameObject = root;
    }
}
