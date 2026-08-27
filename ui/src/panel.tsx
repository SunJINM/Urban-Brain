import { useState } from "react";
import { bindValue, useValue } from "cs2/api";
import engine from "cohtml/cohtml";

const GROUP = "UrbanBrain";

const statusBinding = bindValue<string>(GROUP, "GetStatus", "{}");
const planBinding = bindValue<string>(GROUP, "GetPlan", "{}");

interface Status {
  mode?: string;
  controlled?: number;
  takeoverAvailable?: boolean;
  apiConfigured?: boolean;
  model?: string;
  aiBusy?: boolean;
  inGame?: boolean;
}

interface Movement {
  approach: number;
  direction: string;
  turn: string;
}

interface Phase {
  name: string;
  movements: Movement[];
  targetDuration: number;
  reason: string;
}

interface Plan {
  source?: string;
  rationale?: string;
  cycleLength?: number;
  phases?: Phase[];
  warnings?: string[];
}

interface PlanPair {
  rule?: Plan | null;
  ai?: Plan | null;
}

// ---- 样式。全部内联，省掉一整层构建依赖 ----

const S = {
  root: {
    position: "relative" as const,
    margin: "8rem",
    width: "440rem",
    fontSize: "13rem",
    color: "#e8eaed",
    fontFamily: "inherit",
  },
  card: {
    background: "rgba(20, 24, 30, 0.92)",
    borderRadius: "6rem",
    border: "1rem solid rgba(255,255,255,0.12)",
    overflow: "hidden" as const,
  },
  header: {
    display: "flex",
    alignItems: "center",
    justifyContent: "space-between",
    padding: "10rem 14rem",
    cursor: "pointer",
    background: "rgba(255,255,255,0.05)",
  },
  title: { fontWeight: 600, letterSpacing: "0.5rem" },
  body: { padding: "12rem 14rem" },
  row: {
    display: "flex",
    justifyContent: "space-between",
    padding: "3rem 0",
    color: "#b6bcc6",
  },
  value: { color: "#e8eaed" },
  buttons: {
    display: "flex",
    flexWrap: "wrap" as const,
    gap: "6rem",
    marginTop: "10rem",
  },
  btn: (disabled: boolean) => ({
    flex: "1 1 45%",
    padding: "8rem 10rem",
    borderRadius: "4rem",
    border: "1rem solid rgba(255,255,255,0.18)",
    background: disabled ? "rgba(255,255,255,0.04)" : "rgba(90,140,220,0.25)",
    color: disabled ? "#6b7280" : "#e8eaed",
    cursor: disabled ? "default" : "pointer",
    textAlign: "center" as const,
  }),
  section: {
    marginTop: "12rem",
    paddingTop: "10rem",
    borderTop: "1rem solid rgba(255,255,255,0.1)",
  },
  planTitle: { fontWeight: 600, marginBottom: "4rem" },
  rationale: { color: "#b6bcc6", lineHeight: 1.5, marginBottom: "6rem" },
  phase: {
    padding: "5rem 8rem",
    marginBottom: "4rem",
    borderRadius: "3rem",
    background: "rgba(255,255,255,0.05)",
  },
  warn: { color: "#e0a34a", marginTop: "4rem", lineHeight: 1.4 },
  toast: {
    marginTop: "8rem",
    padding: "6rem 8rem",
    borderRadius: "3rem",
    background: "rgba(90,140,220,0.18)",
    color: "#c9d6ea",
    lineHeight: 1.4,
  },
};

function parse<T>(raw: string, fallback: T): T {
  try {
    return JSON.parse(raw) as T;
  } catch {
    return fallback;
  }
}

/** 把一个相位的放行流向压成一行可读文本，例如「N 左转 · S 左转」。 */
function describeMovements(movements: Movement[]): string {
  const turnName: Record<string, string> = {
    left: "左转",
    straight: "直行",
    right: "右转",
    uturn: "掉头",
  };
  return movements
    .map((m) => `${m.direction} ${turnName[m.turn] ?? m.turn}`)
    .join(" · ");
}

function PlanView({ plan, label }: { plan?: Plan | null; label: string }) {
  if (!plan || !plan.phases || plan.phases.length === 0) {
    return null;
  }

  return (
    <div style={S.section}>
      <div style={S.planTitle}>
        {label} · {plan.phases.length} 相位 · 周期 {Math.round(plan.cycleLength ?? 0)} 秒
      </div>

      {plan.rationale ? <div style={S.rationale}>{plan.rationale}</div> : null}

      {plan.phases.map((p, i) => (
        <div key={i} style={S.phase}>
          <div>
            {i + 1}. {p.name} — {Math.round(p.targetDuration)}s
          </div>
          <div style={{ color: "#96a0ad", marginTop: "2rem" }}>
            {describeMovements(p.movements ?? [])}
          </div>
          {p.reason ? (
            <div style={{ color: "#7f8a97", marginTop: "2rem", lineHeight: 1.4 }}>
              {p.reason}
            </div>
          ) : null}
        </div>
      ))}

      {(plan.warnings ?? []).map((w, i) => (
        <div key={i} style={S.warn}>
          ⚠ {w}
        </div>
      ))}
    </div>
  );
}

export default function Panel() {
  const [open, setOpen] = useState(true);
  const [toast, setToast] = useState<string>("");

  const status = parse<Status>(useValue(statusBinding), {});
  const plans = parse<PlanPair>(useValue(planBinding), {});

  const canAct = !!status.inGame && !!status.takeoverAvailable;
  const canAi = canAct && !!status.apiConfigured && !status.aiBusy;

  const call = (name: string) => {
    engine.call(`${GROUP}.${name}`, "").then(
      (result) => setToast(String(result ?? "")),
      (err) => setToast(`调用失败：${String(err)}`)
    );
  };

  return (
    <div style={S.root}>
      <div style={S.card}>
        <div style={S.header} onClick={() => setOpen(!open)}>
          <span style={S.title}>Urban Brain</span>
          <span style={{ color: "#96a0ad" }}>{open ? "▾" : "▸"}</span>
        </div>

        {open ? (
          <div style={S.body}>
            <div style={S.row}>
              <span>接管模式</span>
              <span style={S.value}>{status.mode ?? "—"}</span>
            </div>
            <div style={S.row}>
              <span>已接管路口</span>
              <span style={S.value}>{status.controlled ?? 0}</span>
            </div>
            <div style={S.row}>
              <span>AI</span>
              <span style={S.value}>
                {status.aiBusy
                  ? "请求中…"
                  : status.apiConfigured
                    ? (status.model ?? "已配置")
                    : "未配置密钥"}
              </span>
            </div>

            {!status.takeoverAvailable ? (
              <div style={S.warn}>
                ⚠ 接管通道不可用，原版信号系统的字段名可能已变，详见日志。
              </div>
            ) : null}

            <div style={S.buttons}>
              <div style={S.btn(!canAct)} onClick={() => canAct && call("CallApplyRule")}>
                规则方案
              </div>
              <div style={S.btn(!canAi)} onClick={() => canAi && call("CallApplyAi")}>
                AI 方案
              </div>
              <div style={S.btn(!status.inGame)} onClick={() => status.inGame && call("CallExportSnapshot")}>
                导出快照
              </div>
              <div style={S.btn(!status.inGame)} onClick={() => status.inGame && call("CallRelease")}>
                释放路口
              </div>
            </div>

            {toast ? <div style={S.toast}>{toast}</div> : null}

            {/* 两个方案并排列出，方便直接比较 AI 相对规则引擎有没有带来增量 */}
            <PlanView plan={plans.rule} label="规则方案" />
            <PlanView plan={plans.ai} label="AI 方案" />
          </div>
        ) : null}
      </div>
    </div>
  );
}
