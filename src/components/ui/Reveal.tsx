"use client";

import {
  Children,
  useEffect,
  useRef,
  useState,
  type ReactNode,
} from "react";

const EASE = "cubic-bezier(0.22, 1, 0.36, 1)";

/**
 * Lightweight scroll-reveal. Progressive enhancement by design:
 * - Server render is fully VISIBLE (no JS → no blank content, good LCP).
 * - After mount, elements still below the viewport get hidden and animate
 *   in when they scroll into view. Elements already on screen never blink.
 */
export function Reveal({
  children,
  delay = 0,
  y = 20,
  once = true,
  className,
}: {
  children: ReactNode;
  delay?: number;
  y?: number;
  once?: boolean;
  className?: string;
}) {
  const ref = useRef<HTMLDivElement>(null);
  const [hidden, setHidden] = useState(false);

  useEffect(() => {
    const el = ref.current;
    if (!el) return;
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;
    // Only elements comfortably below the fold participate in the animation.
    if (el.getBoundingClientRect().top < window.innerHeight * 0.92) return;
    setHidden(true);
    const io = new IntersectionObserver(
      ([entry]) => {
        if (entry.isIntersecting) {
          setHidden(false);
          if (once) io.disconnect();
        } else if (!once) {
          setHidden(true);
        }
      },
      { rootMargin: "0px 0px -40px 0px" }
    );
    io.observe(el);
    return () => io.disconnect();
  }, [once]);

  return (
    <div
      ref={ref}
      className={className}
      style={{
        opacity: hidden ? 0 : 1,
        transform: hidden ? `translateY(${y}px)` : "none",
        transition: `opacity 0.7s ${EASE} ${delay}s, transform 0.7s ${EASE} ${delay}s`,
        willChange: hidden ? "opacity, transform" : undefined,
      }}
    >
      {children}
    </div>
  );
}

/** Staggered group — each direct child reveals with an incremental delay. */
export function Stagger({
  children,
  className,
  step = 0.08,
}: {
  children: ReactNode;
  className?: string;
  step?: number;
}) {
  const items = Children.toArray(children);
  return (
    <div className={className}>
      {items.map((child, i) => (
        <Reveal key={i} delay={i * step}>
          {child}
        </Reveal>
      ))}
    </div>
  );
}

/** Kept for API compatibility — Stagger now handles the reveal itself. */
export function StaggerItem({
  children,
  className,
}: {
  children: ReactNode;
  className?: string;
}) {
  return <div className={className}>{children}</div>;
}
