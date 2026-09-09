import { MarqueeLogos } from "@/components/ui/MarqueeLogos";
import { Reveal } from "@/components/ui/Reveal";
import type { Company } from "@/data/companies";

export function TrustBar({ companies }: { companies: Company[] }) {
  return (
    <section className="border-y border-line bg-surface py-14">
      <div className="container-x">
        <Reveal>
          {/* Precise by design: these are places our team has worked, not
              clients of Kestridge AI. */}
          <p className="mb-10 text-center text-[1rem] font-semibold tracking-tight text-ink sm:text-[1.1176rem]">
            Our talents worked here.
          </p>
        </Reveal>
      </div>
      <MarqueeLogos companies={companies} />
    </section>
  );
}
