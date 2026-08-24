"use client";

import { useEffect, useState } from "react";
import Link from "next/link";

const KEY = "kestridge-cookie-consent"; // "granted" | "denied"

function loadAnalytics(gaId: string) {
  if (document.getElementById("ga-script")) return;
  const src = document.createElement("script");
  src.id = "ga-script";
  src.async = true;
  src.src = `https://www.googletagmanager.com/gtag/js?id=${gaId}`;
  document.head.appendChild(src);
  const init = document.createElement("script");
  init.id = "ga-init";
  init.textContent = `window.dataLayer=window.dataLayer||[];function gtag(){dataLayer.push(arguments);}gtag('js',new Date());gtag('config','${gaId}',{anonymize_ip:true});`;
  document.head.appendChild(init);
}

/**
 * Consent-gated analytics. Renders nothing (and loads nothing) unless a GA id
 * is configured. Analytics only run after the visitor accepts; a Global
 * Privacy Control signal counts as a decline automatically.
 */
export function CookieConsent({ gaId }: { gaId?: string }) {
  const [visible, setVisible] = useState(false);

  useEffect(() => {
    if (!gaId) return;
    let stored: string | null = null;
    try {
      stored = localStorage.getItem(KEY);
    } catch {}
    if (stored === "granted") {
      loadAnalytics(gaId);
      return;
    }
    if (stored === "denied") return;
    if ((navigator as { globalPrivacyControl?: boolean }).globalPrivacyControl) {
      try {
        localStorage.setItem(KEY, "denied");
      } catch {}
      return;
    }
    // Small delay so the banner never competes with first paint.
    const t = setTimeout(() => setVisible(true), 1200);
    return () => clearTimeout(t);
  }, [gaId]);

  if (!gaId || !visible) return null;

  const choose = (value: "granted" | "denied") => {
    try {
      localStorage.setItem(KEY, value);
    } catch {}
    setVisible(false);
    if (value === "granted") loadAnalytics(gaId);
  };

  return (
    <div
      role="dialog"
      aria-label="Cookie notice"
      className="anim-rise card fixed bottom-4 left-4 right-4 z-[70] rounded-2xl p-5 sm:left-auto sm:right-6 sm:w-[370px]"
    >
      <p className="text-sm leading-relaxed text-muted">
        We use a few cookies to understand how this site is used. You can
        accept or decline. Details are in our{" "}
        <Link
          href="/privacy"
          className="font-medium text-accent underline-offset-4 hover:underline"
        >
          Privacy Policy
        </Link>
        .
      </p>
      <div className="mt-4 flex gap-2.5">
        <button
          onClick={() => choose("granted")}
          className="btn-primary flex-1 !px-5 !py-2.5 !text-[14px]"
        >
          Accept
        </button>
        <button
          onClick={() => choose("denied")}
          className="btn-ghost flex-1 !px-5 !py-2.5 !text-[14px]"
        >
          Decline
        </button>
      </div>
    </div>
  );
}
