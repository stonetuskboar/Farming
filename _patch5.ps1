$ErrorActionPreference = 'Stop'
$p = 'E:\游戏策划\再一次农场\新版本\玩法设定.md'
$t = [System.IO.File]::ReadAllText($p)
if ($t.Contains("`r`n")) { $nl = "`r`n" } else { $nl = "`n" }

function Rep($old, $new) {
    if (-not $script:t.Contains($old)) { throw "找不到: $($old.Substring(0,[Math]::Min(30,$old.Length)))" }
    $script:t = $script:t.Replace($old, $new)
}

# 1. 锄头、镰刀并入覆盖型
Rep '| 覆盖型 | 水壶、铲子 |' '| 覆盖型 | 水壶、铲子、锄头、镰刀 |'
$t = $t -replace '> 待确认：锄头、镰刀属于哪一种模型。\r?\n\r?\n', ''

# 2. 回合结束结算顺序
$settle = @(
    '**回合结束结算（顺序固定）**',
    '',
    '| 顺序 | 步骤 | 说明 |',
    '|---|---|---|',
    '| 1 | 保湿结算 | 记录本回合湿润值，供下一回合沿用 |',
    '| 2 | 生长判定 | 应季 + 本回合湿润 → +1 阶段 |',
    '| 3 | 麻雀判定 | 未成熟且未被稻草人覆盖 → 25% 掉 1 阶段 |',
    '| 4 | 湿润重置 | 无保湿的地块 → 否 |',
    '| 5 | 地块状态转移 | 按 §3 的地块状态表 |',
    '',
    '> 节日回合跳过整段结算。'
) -join $nl
Rep '### 3. 浇水' ($settle + $nl + $nl + '### 3. 浇水')

# 3. 麻雀啄到 0 阶段
Rep '- **成年作物不会被啄**，啄食只影响未成熟作物。' @'
- **成年作物不会被啄**，啄食只影响未成熟作物。
- 阶段 0 的作物再被啄 → **植株直接消失**，效果等同「铲除」：地块保持耕地状态，不退回土地。
'@

# 4. 地块状态表加「被占」
Rep '| 带作物 · **未成熟** | 铲掉 | **保持当前状态**（耕地 / 潮湿耕地） |' @'
| 带作物 · **未成熟** | 铲掉 | **保持当前状态**（耕地 / 潮湿耕地） |
| 土地 / 耕地 | 放置**树木**或建成**巨大作物** | **被占**：不可锄、不可种、不可浇水 |
'@

[System.IO.File]::WriteAllText($p, $t, (New-Object System.Text.UTF8Encoding($false)))
"OK lines=" + ([System.IO.File]::ReadAllLines($p).Count)
