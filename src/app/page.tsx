import { Hero } from "@/components/sections/Hero";
import { TrustBar } from "@/components/sections/TrustBar";
import { Company } from "@/components/sections/Company";
import { Services } from "@/components/sections/Services";
import { WhyChooseUs } from "@/components/sections/WhyChooseUs";
import { Team } from "@/components/sections/Team";
import { Security } from "@/components/sections/Security";
import { FAQ } from "@/components/sections/FAQ";
import { Contact } from "@/components/sections/Contact";
import { getContent } from "@/lib/content";

export default async function Home() {
  // One fetch for the whole page. getContent falls back to the constants in
  // src/data on any failure, so this call cannot fail the build and cannot
  // render a blank section.
  const { faq: faqs, team, companies, services } = await getContent();

  // Derived, not hand maintained. The select in the contact form is the same
  // list as the page, in the same order, because it is built from it.
  const serviceOptions = [
    ...services.map((s) => ({ value: s.id, label: s.name })),
    { value: "general", label: "General inquiry" },
  ];

  return (
    <>
      <Hero services={services} />
      <TrustBar companies={companies} />
      <Company />
      <Services services={services} />
      <WhyChooseUs />
      <Team members={team} />
      <Security />
      <FAQ faqs={faqs} />
      <Contact serviceOptions={serviceOptions} />
    </>
  );
}
