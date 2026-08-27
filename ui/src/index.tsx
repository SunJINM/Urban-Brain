import { ModRegistrar } from "cs2/modding";

import Panel from "./panel";

const register: ModRegistrar = (moduleRegistry) => {
  // GameTopLeft 是游戏左上角的锚点，TLE 也挂在这里。
  moduleRegistry.append("GameTopLeft", () => (
    <div id="urban-brain-root" style={{ margin: 0 }}>
      <Panel />
    </div>
  ));
};

export default register;
