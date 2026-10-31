$ErrorActionPreference = 'Stop'
$p = 'E:\游戏策划\再一次农场\新版本\玩法设定.md'
$t = [System.IO.File]::ReadAllText($p)

function Rep($old, $new) {
    if (-not $script:t.Contains($old)) { throw "找不到: $($old.Substring(0,[Math]::Min(30,$old.Length)))" }
    $script:t = $script:t.Replace($old, $new)
}

$old5 = @'
- 未被稻草人覆盖的作物，每回合有 **25%** 概率被麻雀啄掉**一个生长阶段**（不是整株消失）。
- **成年作物不会被啄**，啄食只影响未成熟作物。
- 阶段 0 的作物再被啄 → **植株直接消失**，效果等同「铲除」：地块保持耕地状态，不退回土地。
'@
$new5 = @'
- **只有成年（可收获）作物会被吃**，未成熟作物不会被碰。
- 未被稻草人覆盖的成年作物，每回合有 **25%** 概率被麻雀吃掉：**植株直接消失**、没有产出。
- 被吃掉的作物，地块**保持当前状态**（等同铲除，不退回土地）。
'@
Rep $old5 $new5

Rep '| 2 | 麻雀判定 | 未成熟且未被稻草人覆盖 → 25% 掉 1 阶段；掉到 0 再被啄则植株消失 |' '| 2 | 麻雀判定 | 未被稻草人覆盖的**成年**作物 → 25% 被吃掉（直接消失） |'

Rep '| 麻雀吃掉概率 | 25% | 设计 |' '| 麻雀吃掉概率（每株每回合） | 25% | 设计 |'

Rep '| 带作物 · **未成熟** | 铲掉 | **保持当前状态**（耕地 / 潮湿耕地） |' @'
| 带作物 · **未成熟** | 铲掉 | **保持当前状态**（耕地 / 潮湿耕地） |
| 带作物 · **成年** | 被麻雀吃掉 | **保持当前状态**（同铲除，不退回土地） |
'@

[System.IO.File]::WriteAllText($p, $t, (New-Object System.Text.UTF8Encoding($false)))
"OK lines=" + ([System.IO.File]::ReadAllLines($p).Count)
