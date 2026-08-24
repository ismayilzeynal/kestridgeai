import type { Metadata } from "next";
import { LegalHeader, LegalBody, LegalFootnote } from "@/components/legal/Legal";
import { site } from "@/lib/site";

export const metadata: Metadata = {
  title: "Terms of Use",
  description: `The terms that govern your use of the ${site.brand} website.`,
  alternates: { canonical: "/terms" },
  openGraph: {
    title: `Terms of Use | ${site.brand}`,
    description: `The terms that govern your use of the ${site.brand} website.`,
    url: "/terms",
    type: "website",
    locale: "en_US",
    siteName: site.brand,
  },
};

const UPDATED = "August 24, 2026";

export default function TermsPage() {
  return (
    <>
      <LegalHeader
        title="Terms of Use"
        updated={UPDATED}
        intro={`These Terms of Use ("Terms") govern your access to and use of this website, operated by ${site.legalName} ("Kestridge AI", "we", "us" or "our"). By accessing or using the site, you agree to these Terms. If you do not agree, please do not use the site.`}
      />
      <LegalBody>
        <h2>1. Who we are</h2>
        <p>
          {site.legalName} is a technology company based in Illinois, United
          States, providing AI, automation, IT security, and data analytics
          services. You can contact us at{" "}
          <a href={`mailto:${site.email}`}>{site.email}</a>.
        </p>

        <h2>2. This website is informational</h2>
        <p>
          The content on this website is provided for general information about
          Kestridge AI and our services. It does not constitute a binding offer,
          professional advice, or a guarantee of any particular result. Any
          services we provide to a client are governed by a separate written
          agreement between Kestridge AI and that client; if there is a conflict
          between these Terms and such an agreement, the agreement controls for
          the services it covers.
        </p>

        <h2>3. Permitted use of the site</h2>
        <p>
          We grant you a limited, non-exclusive, non-transferable, revocable
          license to access and use this website for its intended purpose:
          learning about Kestridge AI and contacting us. You agree that you will
          not:
        </p>
        <ul>
          <li>
            Use the site in any way that violates applicable law or regulation;
          </li>
          <li>
            Attempt to gain unauthorized access to the site, its servers or any
            connected systems, or probe, scan or test their vulnerability
            without our written authorization;
          </li>
          <li>
            Introduce viruses, malware or any other harmful code, or interfere
            with the site&apos;s operation or availability;
          </li>
          <li>
            Scrape, harvest or collect data from the site by automated means,
            or use the content to train machine-learning models, without our
            written consent;
          </li>
          <li>
            Impersonate any person or entity, or misrepresent your affiliation
            with any person or entity;
          </li>
          <li>
            Copy, reproduce, republish or redistribute site content except as
            allowed under Section 4.
          </li>
        </ul>

        <h2>4. Intellectual property</h2>
        <p>
          The website and its content, including text, graphics, logos, the
          Kestridge AI name and mark, page designs and underlying code, are owned
          by {site.legalName} or its licensors and are protected by copyright,
          trademark and other intellectual-property laws. You may view, print
          or download content for your personal or internal business use in
          evaluating our services. Any other use, including commercial
          reproduction or creation of derivative works, requires our prior
          written consent.
        </p>

        <h2>5. Information you submit</h2>
        <p>
          When you submit information through our contact form or by email, you
          represent that it is accurate and that you have the right to share
          it. We handle personal information as described in our{" "}
          <a href="/privacy">Privacy Policy</a>, and we treat project
          information you share with us as confidential, as described on this
          website. Please do not submit trade secrets or highly sensitive
          material through the contact form. If your inquiry requires it, ask
          us first and we will agree on a secure channel and, where
          appropriate, a nondisclosure agreement.
        </p>

        <h2>6. Third-party links and services</h2>
        <p>
          The site contains links to third-party websites and services, such as
          LinkedIn, that we do not control. We are not responsible for their
          content, policies or practices, and a link does not imply our
          endorsement. Your use of third-party sites is at your own risk and
          subject to their terms.
        </p>

        <h2>7. Disclaimer of warranties</h2>
        <p>
          THE WEBSITE AND ITS CONTENT ARE PROVIDED &quot;AS IS&quot; AND
          &quot;AS AVAILABLE&quot;, WITHOUT WARRANTIES OF ANY KIND, EXPRESS OR
          IMPLIED, INCLUDING WITHOUT LIMITATION WARRANTIES OF MERCHANTABILITY,
          FITNESS FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. WE DO
          NOT WARRANT THAT THE SITE WILL BE UNINTERRUPTED, SECURE OR ERROR-FREE,
          THAT DEFECTS WILL BE CORRECTED, OR THAT THE CONTENT IS ACCURATE,
          COMPLETE OR CURRENT. SOME JURISDICTIONS DO NOT ALLOW THE EXCLUSION OF
          IMPLIED WARRANTIES, SO SOME OF THE ABOVE EXCLUSIONS MAY NOT APPLY TO
          YOU.
        </p>

        <h2>8. Limitation of liability</h2>
        <p>
          TO THE MAXIMUM EXTENT PERMITTED BY LAW, {`${site.legalName.toUpperCase()}`}{" "}
          AND ITS OFFICERS, DIRECTORS, EMPLOYEES AND AGENTS WILL NOT BE LIABLE
          FOR ANY INDIRECT, INCIDENTAL, SPECIAL, CONSEQUENTIAL, EXEMPLARY OR
          PUNITIVE DAMAGES, INCLUDING LOST PROFITS, LOST DATA OR BUSINESS
          INTERRUPTION, ARISING OUT OF OR RELATED TO YOUR USE OF, OR INABILITY
          TO USE, THIS WEBSITE, EVEN IF WE HAVE BEEN ADVISED OF THE POSSIBILITY
          OF SUCH DAMAGES. OUR TOTAL AGGREGATE LIABILITY FOR ALL CLAIMS RELATING
          TO THE WEBSITE WILL NOT EXCEED ONE HUNDRED US DOLLARS (USD $100).
          THIS SECTION DOES NOT LIMIT LIABILITY THAT CANNOT BE LIMITED UNDER
          APPLICABLE LAW.
        </p>

        <h2>9. Indemnification</h2>
        <p>
          You agree to indemnify and hold harmless {site.legalName} and its
          officers, directors, employees and agents from any claims, damages,
          liabilities and expenses (including reasonable attorneys&apos; fees)
          arising out of your violation of these Terms or your misuse of the
          website.
        </p>

        <h2>10. Changes to the site and these Terms</h2>
        <p>
          We may change, suspend or discontinue any part of the website at any
          time without notice. We may also update these Terms from time to
          time; when we do, we will revise the &quot;Last updated&quot; date at
          the top of this page. Your continued use of the site after an update
          means you accept the revised Terms.
        </p>

        <h2>11. Governing law and venue</h2>
        <p>
          These Terms and any dispute arising out of or related to them or the
          website are governed by the laws of the State of Illinois and
          applicable United States federal law, without regard to
          conflict-of-laws principles. You agree that the state and federal
          courts located in Illinois will have exclusive jurisdiction over any
          such dispute, and you consent to personal jurisdiction and venue in
          those courts.
        </p>

        <h2>12. General</h2>
        <p>
          If any provision of these Terms is found unenforceable, the remaining
          provisions remain in full force. Our failure to enforce any provision
          is not a waiver of it. These Terms, together with the{" "}
          <a href="/privacy">Privacy Policy</a>, are the entire agreement
          between you and {site.legalName} regarding your use of this website.
          You may not assign these Terms; we may assign them in connection with
          a merger, acquisition or sale of assets.
        </p>

        <h2>13. Contact us</h2>
        <p>
          Questions about these Terms: email{" "}
          <a href={`mailto:${site.email}`}>{site.email}</a>. {site.legalName},{" "}
          {site.location}.
        </p>

        <LegalFootnote
          label="See also:"
          href="/privacy"
          linkText="Privacy Policy"
        />
      </LegalBody>
    </>
  );
}
