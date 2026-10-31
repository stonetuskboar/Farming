// =====================================================================
// 【AI TEMP-008】锁定精力（临时作弊·运行部分）
// 搜索关键字：AI TEMP-008
//
// 用法：Play 模式里按 F8 开/关；也可以在「季节换皮预览」窗口里勾选。
//       开启后，精力一掉就立刻补满。
//
// 说明：只在游戏循环里跑，不依赖编辑器窗口，关掉窗口也有效。
// 删除方法：删掉本文件即可。
// =====================================================================
using UnityEngine;
using UnityEngine.InputSystem;

public class AI_TEMP_EnergyLock : MonoBehaviour
{
    public static bool Enabled;

    private GameManager gm;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoBoot()
    {
        if (FindObjectOfType<AI_TEMP_EnergyLock>() != null) return;

        GameObject go = new GameObject("[AI TEMP-008] EnergyLock");
        DontDestroyOnLoad(go);
        go.AddComponent<AI_TEMP_EnergyLock>();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.f8Key.wasPressedThisFrame)
        {
            Enabled = !Enabled;
            Debug.Log("[AI TEMP-008] 锁定精力：" + (Enabled ? "开" : "关"));
        }

        if (!Enabled) return;

        if (gm == null)
        {
            gm = FindObjectOfType<GameManager>();
            if (gm == null) return;
        }

        if (gm.NowEnergy < gm.MaxEnergy)
        {
            gm.NowEnergy = gm.MaxEnergy;
            if (gm.energyBar != null)
            {
                gm.energyBar.SetEnergy(gm.MaxEnergy, gm.MaxEnergy);
            }
        }
    }
}
