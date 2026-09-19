using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

// ============ 切片对齐128网格工具 ============
// 作用：把自动切片（沿素材边缘裁的框）自动向外扩到128像素网格线上，
//       保住你在画图软件里的格子对齐信息
// 用法：
//   1. Project 里先照常「自动切片」
//   2. 右键点这张图 → 「切片对齐128网格」
//   3. 完事：所有框的边缘都落在网格线上，素材内容一像素不丢
// 说明：
//   Unity 2022 起旧的 TextureImporter.spritesheet 写入接口已失效，
//   这里改用官方新接口 ISpriteEditorDataProvider（2D Sprite 包）。
//   另外 Unity 网格原点在图片【左下角】、画图软件在【左上角】，
//   图片尺寸不是128倍数时两边会错开，本工具会自动补偿。
public static class SliceGridAlignTool
{
    const float Grid = 128f; // 网格大小，以后格子变了改这里

    [MenuItem("Assets/切片对齐128网格", false, 2000)]
    static void Align()
    {
        int done = 0;

        foreach (Object o in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(o);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;
            if (importer.textureType != TextureImporterType.Sprite) continue;
            if (importer.spriteImportMode != SpriteImportMode.Multiple) continue;

            // 用官方新接口读写切片数据
            SpriteDataProviderFactories factory = new SpriteDataProviderFactories();
            factory.Init();
            ISpriteEditorDataProvider provider = factory.GetSpriteEditorDataProviderFromObject(importer);
            if (provider == null) continue;
            provider.InitSpriteEditorDataProvider();

            // 图片真实尺寸 → 算两套网格的偏差量
            ITextureDataProvider texProvider = provider.GetDataProvider<ITextureDataProvider>();
            int texW = 0, texH = 0;
            if (texProvider != null) texProvider.GetTextureActualWidthAndHeight(out texW, out texH);

            float offX = texW > 0 ? texW % Grid : 0f;
            float offY = texH > 0 ? texH % Grid : 0f;

            SpriteRect[] rects = provider.GetSpriteRects();
            if (rects == null || rects.Length == 0) continue;

            int aligned = 0;
            for (int i = 0; i < rects.Length; i++)
            {
                Rect r = rects[i].rect;

                float x = offX + Mathf.Floor((r.x - offX) / Grid) * Grid;
                float y = offY + Mathf.Floor((r.y - offY) / Grid) * Grid;
                float right = offX + Mathf.Ceil((r.x + r.width - offX) / Grid) * Grid;
                float top = offY + Mathf.Ceil((r.y + r.height - offY) / Grid) * Grid;

                if (texW > 0 && texH > 0)
                {
                    x = Mathf.Clamp(x, 0f, texW);
                    y = Mathf.Clamp(y, 0f, texH);
                    right = Mathf.Clamp(right, 0f, texW);
                    top = Mathf.Clamp(top, 0f, texH);
                }

                rects[i].rect = new Rect(x, y, right - x, top - y);
                aligned++;
            }

            provider.SetSpriteRects(rects);
            provider.Apply();
            importer.SaveAndReimport();
            done++;

            Debug.Log("[切片对齐] " + path + "：" + aligned + " 个切片已对齐网格"
                      + "（图 " + texW + "x" + texH
                      + "，横向偏差 " + offX + "px，纵向偏差 " + offY + "px 已补偿）");
        }

        if (done == 0)
            Debug.LogWarning("[切片对齐] 没有处理任何图片：请右键一张 Sprite模式=多个 的图片");
    }

    // 只在右键的是"多图切片"素材时才显示菜单项
    [MenuItem("Assets/切片对齐128网格", true)]
    static bool Validate()
    {
        foreach (Object o in Selection.objects)
        {
            TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(o)) as TextureImporter;
            if (importer != null && importer.spriteImportMode == SpriteImportMode.Multiple)
                return true;
        }
        return false;
    }
}
