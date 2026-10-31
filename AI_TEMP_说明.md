# AI_TEMP 临时工具说明（可整组删除）

> 全部为**附加**文件，未修改项目原有代码。
> 运行时自动挂载，只改内存，退出 Play 自动还原。
> 删除本组文件 + `Assets/Resources/AI_TEMP/` 即可完全还原。

---

## 总览

16 个脚本 + 4 个自动生成的配置，全部以 `AI_TEMP_` 开头。

| 编号 | 功能 | 脚本 | 配置 |
|---|---|---|---|
| 002 | 松/柏/橡 四季换图 | 3 | SeasonTreeConfig.asset |
| 003 | 树叶 / 树叶顶 / 大树 四季 | 3 | LeafSeasonConfig.asset |
| 004 | 季节换皮预览窗口 | 1 | — |
| 005 | 悬崖 / 悬崖底部 四季 | 3 | CliffSeasonConfig.asset |
| 006 | 修复四季贴图缓存 | 1 | — |
| 007 | 白桦（临时作弊树） | 3 | BirchConfig.asset |
| 008 | 锁定精力 | 1 | — |
| 009 | Ctrl+滚轮 翻热栏 | 1 | — |

---

## 002 · 松/柏/橡 四季换图

- `Assets/Editor/AI_TEMP_SeasonTreeSetup.cs` — 按名字扫图（`松树_秋`、`3阶段柏树_秋冬`…），生成配置
- `Assets/Scripts/AI_TEMP_SeasonTreeConfig.cs` — 配置数据结构
- `Assets/Scripts/AI_TEMP_SeasonTreeSprites.cs` — 每 0.5 秒检查季节并换 Sprite
- `Assets/Resources/AI_TEMP/SeasonTreeConfig.asset` — 自动生成

管：松树(15)、柏树(16)、橡树(17)，只处理「2阶段」和「成熟」。
触发：自动，跟 `GameManager.season`。

---

## 003 · 树叶 / 树叶顶 / 大树 四季

- `Assets/Editor/AI_TEMP_LeafSeasonSetup.cs` — 扫 `双层树叶`、`树叶顶` 四季图
- `Assets/Scripts/AI_TEMP_LeafSeasonConfig.cs` — 配置数据
- `Assets/Scripts/AI_TEMP_LeafSeason.cs` — 换 RuleTile 贴图 + 换场景「大树」Sprite
- `Assets/Resources/AI_TEMP/LeafSeasonConfig.asset` — 自动生成

管：地图树叶层（双层树叶 / 树叶顶）、场景里名为「大树」的对象（`大树_春/夏/秋/冬`）。
触发：自动。

---

## 004 · 季节换皮预览（常用入口）

- `Assets/Editor/AI_TEMP_SeasonPreview.cs`

入口：菜单 `AI TEMP → 季节换皮预览`
功能：
- 春 / 夏 / 秋 / 冬 四个按钮（只换显示，不改月份、不改存档）
- 满精力（作弊）
- 锁定精力（等同 F8）

---

## 005 · 悬崖 / 悬崖底部 四季

- `Assets/Editor/AI_TEMP_CliffSeasonSetup.cs` — 读 4 张悬崖 RuleTile + 4 套底部贴图
- `Assets/Scripts/AI_TEMP_CliffSeasonConfig.cs` — 配置数据
- `Assets/Scripts/AI_TEMP_CliffSeason.cs` — 换悬崖 RenderTile；按编号换底部 Tile Sprite
- `Assets/Resources/AI_TEMP/CliffSeasonConfig.asset` — 自动生成

触发：自动跟季节。

---

## 006 · 修复四季贴图缓存

- `Assets/Editor/AI_TEMP_RepairTilemapCache.cs`

入口：菜单 `AI TEMP → 修复四季贴图缓存`
用途：在 Play 里保存过场景、导致某层卡在冬天/夏天贴图时，重建显示缓存。

---

## 007 · 白桦（临时作弊树）

- `Assets/Editor/AI_TEMP_BirchSetup.cs` — 找 `白桦_春/夏/秋/冬`，生成配置
- `Assets/Scripts/AI_TEMP_BirchConfig.cs` — 配置数据
- `Assets/Scripts/AI_TEMP_BirchCheat.cs` — 运行时造临时白桦 + 热栏物品 + 四季换图
- `Assets/Resources/AI_TEMP/BirchConfig.asset` — 自动生成

入口：
- 热栏自动多一格「白桦（成年）」×999（item id 901）
- 按 `B` 在鼠标位置种一棵
- 按 `F9` 重新发一次物品

注意：白桦是运行时对象（crop id 900），不进存档、不进 CropDataList；砍了会变松树桩。

---

## 008 · 锁定精力

- `Assets/Scripts/AI_TEMP_EnergyLock.cs`

入口：`F8`，或季节预览窗口里的勾选项（显示当前开/关）。
效果：精力一掉就补满。

---

## 009 · Ctrl + 滚轮 翻热栏

- `Assets/Scripts/AI_TEMP_HotbarPaging.cs`

入口：按住 `Ctrl` + 滚轮上/下，整页翻动。
不按 Ctrl 时，滚轮仍是原来的“切换当前工具”。

---

## 快捷键 / 菜单汇总

| 操作 | 效果 |
|---|---|
| 菜单 `AI TEMP → 季节换皮预览` | 切四季 / 满精力 / 锁精力 |
| 菜单 `AI TEMP → 修复四季贴图缓存` | 修贴图缓存 |
| 菜单 `AI TEMP → 重新生成…配置` | 重切图后重建配置（共四个） |
| `B` | 鼠标处种白桦 |
| `F8` | 锁定精力开关 |
| `F9` | 补发白桦物品 |
| `Ctrl + 滚轮` | 热栏翻页 |

---

## 注意事项

1. 用预览切过季节后，**不要在 Play 里保存场景**，否则会把预览状态写进场景。
2. 重切图后要点一次对应的「重新生成」菜单，配置不会自动更新。
3. 删除某功能时，删对应脚本即可；`Resources/AI_TEMP` 下的配置删掉会自动重建。

---

## 改过的已有文件（仅导入设置）

- `Assets/Art/Crops/3阶段松树_秋季.png.meta`
- `Assets/Art/Crops/3阶段柏树_秋季.png.meta`
- `Assets/Art/Crops/3阶段松树_冬季.png.meta`

仅调整 Sprite 轴心（让树落地），不涉及逻辑。
