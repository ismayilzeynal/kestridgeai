export function Background() {
  return (
    <div aria-hidden className="pointer-events-none fixed inset-0 -z-10 overflow-hidden">
      {/* base wash */}
      <div className="absolute inset-0 bg-bg" />
      {/* cool accent glow, top */}
      <div
        className="absolute -top-40 left-1/2 h-[640px] w-[1100px] -translate-x-1/2 rounded-full opacity-[0.18] blur-[120px]"
        style={{
          background:
            "radial-gradient(closest-side, var(--accent), transparent 70%)",
        }}
      />
      {/* faint warm counter-glow, lower right — keeps it from feeling clinical */}
      <div
        className="absolute bottom-[-10%] right-[-5%] h-[520px] w-[620px] rounded-full opacity-[0.10] blur-[130px]"
        style={{
          background: "radial-gradient(closest-side, #ff9b6a, transparent 70%)",
        }}
      />
      {/* fine blueprint grid */}
      <div className="bg-grid bg-grid-fade absolute inset-0" />
    </div>
  );
}
