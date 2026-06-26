"use client";

import { MotionConfig } from "framer-motion";
import { type ReactNode } from "react";

// Globally honor the user's prefers-reduced-motion setting for every
// framer-motion animation on the site.
export function Providers({ children }: { children: ReactNode }) {
  return <MotionConfig reducedMotion="user">{children}</MotionConfig>;
}
