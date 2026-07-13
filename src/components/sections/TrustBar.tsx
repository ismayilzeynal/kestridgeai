import { MarqueeLogos } from "@/components/ui/MarqueeLogos";
import { Reveal } from "@/components/ui/Reveal";

export function TrustBar() {
  return (
    <section className="border-y border-line bg-surface py-14">
      <div className="container-x">
        <Reveal>
          <p className="mb-10 text-center text-[15px] font-medium text-muted">
            Companies our team has worked with
          </p>
        </Reveal>
      </div>
      <MarqueeLogos />
    </section>
  );
}
