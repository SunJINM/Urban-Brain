#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
从 cs2-ecs-explorer 的数据集中提取 Urban Brain 关心的 ECS 组件定义，
生成 docs/10-游戏API参考.md。

用法：
    python scripts/extract-api.py

前置：_reference/cs2-ecs-explorer/ 已 clone
    git clone --depth 1 https://github.com/Captain-Of-Coit/cs2-ecs-explorer.git

数据来源：Captain-Of-Coit/cs2-ecs-explorer (MIT)，从反编译源码提取。
注意：该数据集为 2023 年游戏发布初期版本，字段可能已变化，需实测确认。
"""
import json, io, os, sys
from collections import defaultdict

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DATA = os.path.join(ROOT, "_reference", "cs2-ecs-explorer", "data")
OUT  = os.path.join(ROOT, "docs", "10-游戏API参考.md")

# 我们关心的领域 -> (命名空间前缀, 说明)
DOMAINS = [
    ("Game.Net",       "路网与车道 —— 交通感知层的数据源"),
    ("Game.Pathfind",  "寻路 —— 绕行分析"),
    ("Game.Vehicles",  "载具 —— 货运/通勤主体"),
    ("Game.Citizens",  "市民 —— 通勤与出行"),
    ("Game.Companies", "公司 —— 货运需求与盈利"),
    ("Game.Buildings", "建筑 —— 废弃/服务/需求的落点"),
    ("Game.Zones",     "分区"),
    ("Game.Routes",    "公交线路"),
    ("Game.City",      "城市级聚合"),
]

def load(name):
    p = os.path.join(DATA, name)
    if not os.path.exists(p):
        sys.exit("找不到 %s\n请先 clone cs2-ecs-explorer 到 _reference/ (见本文件头部注释)" % p)
    return json.load(io.open(p, encoding="utf-8"))

def main():
    comps = load("Components.json")

    buckets = defaultdict(list)
    for full, d in comps.items():
        for prefix, _ in DOMAINS:
            if full.startswith(prefix + "."):
                buckets[prefix].append((full, d))
                break

    L = []
    L.append("# 游戏 ECS 组件参考（提取子集）")
    L.append("")
    L.append("> **本文件由 `scripts/extract-api.py` 自动生成，不要手改。**")
    L.append(">")
    L.append("> 数据来源：[Captain-Of-Coit/cs2-ecs-explorer](https://github.com/Captain-Of-Coit/cs2-ecs-explorer) (MIT)，")
    L.append("> 由反编译源码提取。仅收录 Urban Brain 关心的领域，`Game.Prefabs` 等未收录。")
    L.append(">")
    L.append("> ⚠ **该数据集为 2023 年游戏发布初期版本。** 字段可能已随版本变化，")
    L.append("> 每个用到的字段都需在真机编译时确认。已交叉验证的部分见文末。")
    L.append("")
    total = sum(len(v) for v in buckets.values())
    L.append("收录 %d 个组件，跨 %d 个领域。" % (total, len(buckets)))
    L.append("")
    L.append("---")
    L.append("")

    for prefix, desc in DOMAINS:
        items = sorted(buckets.get(prefix, []))
        if not items:
            continue
        L.append("## %s" % prefix)
        L.append("")
        L.append("*%s* — %d 个组件" % (desc, len(items)))
        L.append("")
        for full, d in items:
            short = full[len(prefix) + 1:]
            props = d.get("properties", [])
            L.append("### `%s`" % short)
            L.append("")
            if props:
                L.append("| 类型 | 字段 |")
                L.append("|---|---|")
                for p in props:
                    L.append("| `%s` | `%s` |" % (p.get("type", "?"), p.get("name", "?")))
            else:
                L.append("*(标记组件，无字段)*")
            L.append("")
            systems = sorted(set(d.get("used_in_system", [])))
            if systems:
                names = [s.split(".")[-1] for s in systems]
                shown = ", ".join("`%s`" % n for n in names[:10])
                more = " …等 %d 个" % len(names) if len(names) > 10 else ""
                L.append("被使用：%s%s" % (shown, more))
                L.append("")
        L.append("---")
        L.append("")

    L.append("## 交叉验证")
    L.append("")
    L.append("以下字段已通过活跃维护中的 mod 源码交叉确认（说明这部分结构在 2026 年仍然有效）：")
    L.append("")
    L.append("| 组件 | 字段 | 验证来源 |")
    L.append("|---|---|---|")
    L.append("| `Game.Net.LaneFlow` | `m_Distance` `m_Duration` `m_Next` | TrafficLightsEnhancement `CustomStateMachine.CalculateFlow` |")
    L.append("| `Game.Net.LaneSignal` | `m_GroupMask` `m_Petitioner` `m_Priority` | TrafficLightsEnhancement `CustomStateMachine` |")
    L.append("| `Game.Net.TrafficLights` | `m_State` `m_CurrentSignalGroup` | TrafficLightsEnhancement `CustomStateMachine` |")
    L.append("| `Game.Citizens.Citizen` / `Worker` / `Student` | (作为 `IJobChunk` 查询类型) | InfoLoom `WorkforceInfoLoomUISystem` |")
    L.append("")

    io.open(OUT, "w", encoding="utf-8", newline="\n").write("\n".join(L))
    print("生成 %s" % OUT)
    print("收录 %d 个组件：" % total)
    for prefix, _ in DOMAINS:
        n = len(buckets.get(prefix, []))
        if n:
            print("   %-18s %d" % (prefix, n))

if __name__ == "__main__":
    main()
