import { ImageResponse } from "next/og";
import { site } from "@/lib/site";

// Rendered per request at the edge, so the wordmark always tracks site.brand.
// Replace with static artwork once the final logo kit is delivered.
export const runtime = "edge";
export const alt = `${site.brand}, AI, automation, IT security, and data analytics`;
export const size = { width: 1200, height: 630 };
export const contentType = "image/png";

export default function OpengraphImage() {
  return new ImageResponse(
    (
      <div
        style={{
          width: "100%",
          height: "100%",
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
          background: "#f5f6f9",
          fontFamily: "sans-serif",
        }}
      >
        <div style={{ display: "flex", alignItems: "center", gap: 34 }}>
          <div
            style={{
              width: 128,
              height: 128,
              borderRadius: 32,
              background: "#14181f",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
            }}
          >
            <div
              style={{
                width: 14,
                height: 62,
                borderRadius: 999,
                background: "#0b7a67",
              }}
            />
          </div>
          <div
            style={{
              fontSize: 104,
              fontWeight: 700,
              letterSpacing: -4,
              color: "#111821",
            }}
          >
            {site.brand}
          </div>
        </div>
        <div
          style={{
            marginTop: 40,
            fontSize: 26,
            letterSpacing: 4,
            color: "#4a5360",
          }}
        >
          AI · AUTOMATION · IT SECURITY · DATA ANALYTICS
        </div>
      </div>
    ),
    size
  );
}
