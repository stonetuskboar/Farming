// =====================================================================
// 【AI TEMP-009】Ctrl + 滚轮 翻热栏整页（临时）
// 搜索关键字：AI TEMP-009
//
// 用法：Play 里按住 Ctrl，滚轮上/下 → 热栏整页上翻/下翻。
//       不按 Ctrl 时，滚轮还是原来的“切换当前工具”。
//
// 说明：不改 HotBarController 的代码，只在按住 Ctrl 时临时禁用它的滚轮，
//       避免“翻页”和“切工具”同时触发。
// 删除方法：删掉本文件即可。
// =====================================================================
using UnityEngine;
using UnityEngine.InputSystem;

public class AI_TEMP_HotbarPaging : MonoBehaviour
{
    private GameManager gm;
    private HotBarController hotbar;
    private float cooldown;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoBoot()
    {
        if (FindObjectOfType<AI_TEMP_HotbarPaging>() != null) return;

        GameObject go = new GameObject("[AI TEMP-009] HotbarPaging");
        DontDestroyOnLoad(go);
        go.AddComponent<AI_TEMP_HotbarPaging>();
    }

    private void Update()
    {
        if (gm == null)
        {
            gm = FindObjectOfType<GameManager>();
            if (gm == null) return;
        }

        if (hotbar == null) hotbar = gm.hotbarController;
        if (hotbar == null) return;

        Keyboard kb = Keyboard.current;
        bool ctrl = kb != null && (kb.leftCtrlKey.isPressed || kb.rightCtrlKey.isPressed);

        // 按住 Ctrl 时临时关掉热栏自己的滚轮，松开再恢复
        if (ctrl && hotbar.enabled) hotbar.enabled = false;
        else if (!ctrl && !hotbar.enabled) hotbar.enabled = true;

        if (!ctrl)
        {
            cooldown = 0f;
            return;
        }

        cooldown -= Time.unscaledDeltaTime;
        if (cooldown > 0f) return;

        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) < 0.01f) return;

        if (scroll > 0f) hotbar.UpPage();
        else hotbar.DownPage();

        cooldown = 0.18f;
    }
}
