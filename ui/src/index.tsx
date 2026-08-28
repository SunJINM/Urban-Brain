import { ModRegistrar } from "cs2/modding";

import TLEApp from "./tle/app";
import { ModUI } from "./traffic/modUI/modUI";
import { DebugUiEditorButton } from "./traffic/debugUi/debugUi";

/**
 * Urban Brain 前端入口。
 *
 * 两套界面合并注册在同一个挂载点上：
 *   TLEApp —— 信号相位面板（选中路口后出现）
 *   ModUI  —— 车道连接与优先级工具条
 *
 * 二者各自独立管理显隐，互不干扰。
 */
const register: ModRegistrar = (moduleRegistry) => {
  moduleRegistry.append("GameTopLeft", () => (
    <div id="urban-brain-signals" style={{ margin: 0 }}>
      <TLEApp />
    </div>
  ));

  moduleRegistry.append("GameTopLeft", ModUI);

  moduleRegistry.append("Editor", DebugUiEditorButton);
};

export default register;
