export function Background() {
  return (
    <div aria-hidden className="pointer-events-none fixed inset-0 -z-10 overflow-hidden">
      {/* base wash */}
      <div className="absolute inset-0 bg-bg" />
      {/* soft teal tint, top */}
      <div
        className="absolute -top-40 left-1/2 h-[620px] w-[1100px] -translate-x-1/2 rounded-full opacity-[0.10] blur-[130px]"
        style={{
          background: "radial-gradient(closest-side, var(--accent), transparent 70%)",
        }}
      />
      {/* faint cool tint, lower right */}
      <div
        className="absolute bottom-[-12%] right-[-6%] h-[520px] w-[620px] rounded-full opacity-[0.07] blur-[140px]"
        style={{
          background: "radial-gradient(closest-side, #6aa8ff, transparent 70%)",
        }}
      />
      {/* fine blueprint grid */}
      <div className="bg-grid bg-grid-fade absolute inset-0" />
    </div>
  );
}
