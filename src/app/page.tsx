import { Background } from "@/components/layout/Background";
import { ScrollProgress } from "@/components/layout/ScrollProgress";
import { Navbar } from "@/components/layout/Navbar";
import { MobileCTA } from "@/components/layout/MobileCTA";
import { Footer } from "@/components/layout/Footer";
import { Hero } from "@/components/sections/Hero";
import { TrustBar } from "@/components/sections/TrustBar";
import { WhyChooseUs } from "@/components/sections/WhyChooseUs";
import { Services } from "@/components/sections/Services";
import { Team } from "@/components/sections/Team";
import { Security } from "@/components/sections/Security";
import { CTABand } from "@/components/sections/CTABand";
import { Contact } from "@/components/sections/Contact";

export default function Home() {
  return (
    <>
      <Background />
      <ScrollProgress />
      <Navbar />
      <main id="main">
        <Hero />
        <TrustBar />
        <WhyChooseUs />
        <Services />
        <Team />
        <Security />
        <CTABand />
        <Contact />
      </main>
      <Footer />
      <MobileCTA />
    </>
  );
}
