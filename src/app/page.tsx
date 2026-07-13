import { Hero } from "@/components/sections/Hero";
import { TrustBar } from "@/components/sections/TrustBar";
import { WhyChooseUs } from "@/components/sections/WhyChooseUs";
import { Services } from "@/components/sections/Services";
import { Team } from "@/components/sections/Team";
import { Security } from "@/components/sections/Security";
import { FAQ } from "@/components/sections/FAQ";
import { CTABand } from "@/components/sections/CTABand";
import { Contact } from "@/components/sections/Contact";

export default function Home() {
  return (
    <>
      <Hero />
      <TrustBar />
      <WhyChooseUs />
      <Services />
      <Team />
      <Security />
      <FAQ />
      <CTABand />
      <Contact />
    </>
  );
}
