import { MarqueeLogos } from "@/components/ui/MarqueeLogos";
import { Reveal } from "@/components/ui/Reveal";

export function TrustBar() {
  return (
    <section className="border-y border-line bg-bg-soft/40 py-16">
      <div className="container-x">
        <Reveal>
          <p className="mx-auto mb-9 max-w-2xl text-center text-sm text-muted">
            <span className="text-ink">Built by people who&apos;ve done it before.</span>{" "}
            Our engineers have previously worked at and alongside teams like
            these.
          </p>
        </Reveal>
      </div>
      <MarqueeLogos />
      <div className="container-x">
        <p className="mt-9 text-center font-mono text-[11px] uppercase tracking-label text-faint">
          Prior experience of our team — not client endorsements
        </p>
      </div>
    </section>
  );
}
