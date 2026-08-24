export function Background() {
  return (
    <div aria-hidden className="pointer-events-none fixed inset-0 -z-10 overflow-hidden">
      {/* base wash */}
      <div className="absolute inset-0 bg-bg" />
      {/* soft tints - plain radial gradients (no blur filters: cheap to paint) */}
      <div
        className="absolute -top-40 left-1/2 h-[36.4706rem] w-[64.7059rem] -translate-x-1/2"
        style={{
          background:
            "radial-gradient(closest-side, rgba(11,122,103,0.09), transparent 70%)",
        }}
      />
      <div
        className="absolute bottom-[-12%] right-[-6%] h-[30.5882rem] w-[36.4706rem]"
        style={{
          background:
            "radial-gradient(closest-side, rgba(106,168,255,0.07), transparent 70%)",
        }}
      />
      {/* fine blueprint grid */}
      <div className="bg-grid bg-grid-fade absolute inset-0" />
    </div>
  );
}
