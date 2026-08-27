# Urban Brain — 项目约定

## 提交规范

- **禁止在 commit message 中添加 `Co-Authored-By` 尾注**（包括 AI 署名）
- **禁止在 PR / commit 中添加 "Generated with Claude Code" 之类的标记**
- commit message 用中文，格式 `type: 简述`，type 取 `feat` / `fix` / `docs` / `chore` / `refactor`

## 安全红线

以下内容**绝不能进入版本控制**，提交前必检：

- 游戏私有程序集（`Game.dll`、`Colossal.*.dll`、`Unity.*.dll`）—— PDX/CO 版权
- 反编译产物（`decompiled/`）
- 任何 API key、PDX 账号凭据（`pdx_account.txt`、`.env`、`*.key`）

`.gitignore` 已配置对应规则，但不要依赖它兜底。

## 协作约定

**用户不写 C#。** 这决定了几件事：

- 任何需要用户执行的操作，必须提供一键脚本，不要让用户手动跑 MSBuild 或改代码
- 里程碑验收标准要写成**视觉可判断**的描述（"右上角出现一个数字，切换路口会变"），不是"实现了 X 功能"
- 日志要自解释：关键异常行带 `⚠` 标记并附带"这说明什么"，用户只需贴带标记的行
- 编译、进游戏测试、收集日志只能由用户完成；Claude 侧无游戏环境

详见 [docs/01-协作流程与分工.md](../docs/01-协作流程与分工.md)。

## 项目定位

做**跨域因果诊断**，不做数值优化（后者已有 TrafficLightsEnhancement 等成熟方案）。

架构三层解耦，L2 快照 JSON 是稳定契约，也是让 Claude 能在无游戏环境下独立调试的支点。

## 参考资料位置

`_reference/` 下的仓库不入库，需要时自行 clone：

```bash
git clone --depth 1 https://github.com/Infixo/CS2-InfoLoom.git
git clone --depth 1 https://github.com/slyh/Cities2-TrafficLightsEnhancement.git
git clone --depth 1 https://github.com/krzychu124/Traffic.git
git clone --depth 1 https://github.com/Captain-Of-Coit/cs2-ecs-explorer.git
```

ECS 组件定义已提取到 [docs/10-游戏API参考.md](../docs/10-游戏API参考.md)，由 `scripts/extract-api.py` 生成。
