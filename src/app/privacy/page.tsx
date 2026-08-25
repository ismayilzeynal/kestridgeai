import type { Metadata } from "next";
import { LegalHeader, LegalBody, LegalFootnote } from "@/components/legal/Legal";
import { site } from "@/lib/site";

export const metadata: Metadata = {
  title: "Privacy Policy",
  description: `How ${site.legalName} collects, uses, protects and shares personal information on this website.`,
  alternates: { canonical: "/privacy" },
  openGraph: {
    title: `Privacy Policy | ${site.brand}`,
    description: `How ${site.legalName} collects, uses, protects and shares personal information on this website.`,
    url: "/privacy",
    type: "website",
    locale: "en_US",
    siteName: site.brand,
    // A child openGraph replaces the parent's rather than merging, and the root
    // opengraph-image.png file convention only attaches to the root segment, so
    // without this the page ships a large-image card with no image.
    images: ["/opengraph-image.png"],
  },
  // Same reason in reverse: define no twitter block and the root's homepage
  // title and description leak onto this page.
  twitter: {
    card: "summary_large_image",
    title: `Privacy Policy | ${site.brand}`,
    description: `How ${site.legalName} collects, uses, protects and shares personal information on this website.`,
    images: ["/opengraph-image.png"],
  },
};

const UPDATED = "August 24, 2026";

export default function PrivacyPage() {
  return (
    <>
      <LegalHeader
        title="Privacy Policy"
        updated={UPDATED}
        intro={`This Privacy Policy explains how ${site.legalName} ("Kestridge AI", "we", "us" or "our") collects, uses, shares and protects information when you visit this website or contact us through it. We keep it simple: we collect only what we need to respond to you and to run the site, we never sell your information, and everything you share with us is treated as confidential.`}
      />
      <LegalBody>
        <h2>1. Who we are</h2>
        <p>
          {site.legalName} is a technology company based in Illinois, United
          States, providing AI, automation, IT security, and data analytics
          services. For anything related to this Policy, you can reach us at{" "}
          <a href={`mailto:${site.email}`}>{site.email}</a>.
        </p>

        <h2>2. Information we collect</h2>
        <h3>Information you provide to us</h3>
        <p>
          When you contact us through the form on this website or by email, we
          collect the information you choose to share:
        </p>
        <ul>
          <li>Your name and email address;</li>
          <li>Your company name and phone number, if you provide them;</li>
          <li>
            The service area you select and the description of your project or
            inquiry.
          </li>
        </ul>
        <p>
          Please do not include sensitive personal information (such as
          government identification numbers, financial account numbers or
          health information) in your messages. We do not need it to respond to
          you.
        </p>
        <h3>Information collected automatically</h3>
        <p>
          Like most websites, we and our service providers may automatically
          collect certain technical information when you visit, including your
          IP address, browser and device type, operating system, the pages you
          view, the time and duration of your visit, and the website that
          referred you. This information is collected through cookies, log
          files and similar technologies, and is used in aggregate to operate,
          secure and improve the site.
        </p>

        <h2>3. Cookies and analytics</h2>
        <p>
          We use, or may use, the following categories of cookies and similar
          technologies:
        </p>
        <ul>
          <li>
            <strong>Essential cookies</strong>, required for the website to
            function properly and securely;
          </li>
          <li>
            <strong>Analytics cookies</strong>, where we may use Google Analytics to
            understand how visitors use the site (pages visited, time on site,
            approximate location at city level). Google Analytics does not give
            us access to your identity.
          </li>
        </ul>
        <p>
          We also use Vercel Web Analytics, which counts page views without
          cookies and without collecting information that identifies you.
          Because it sets no cookies, it runs on every visit.
        </p>
        <p>
          When cookie-based analytics are active on this site, a small notice
          appears on your first visit letting you{" "}
          <strong>accept or decline</strong> analytics cookies. Cookie-based
          analytics run only if you accept. You can also
          control or delete cookies through your browser settings, and you can
          opt out of Google Analytics using Google&apos;s{" "}
          <a
            href="https://tools.google.com/dlpage/gaoptout"
            target="_blank"
            rel="noopener noreferrer"
          >
            browser add-on
          </a>
          . Disabling cookies may affect some site functionality.
        </p>

        <h2>4. How we use your information</h2>
        <p>We use the information described above to:</p>
        <ul>
          <li>Respond to your inquiries and communicate with you about them;</li>
          <li>Prepare proposals and provide services you request;</li>
          <li>Operate, maintain, secure and improve this website;</li>
          <li>Analyze site usage in aggregate to improve our content;</li>
          <li>Comply with legal obligations and enforce our terms;</li>
          <li>
            Protect the rights, safety and property of Kestridge AI, our clients and
            others.
          </li>
        </ul>
        <p>
          We do not use your information for automated decision-making that
          produces legal effects, and we do not send marketing emails unless
          you have asked to hear from us.
        </p>

        <h2>5. How we share information</h2>
        <p>
          <strong>We do not sell your personal information</strong>, and we do
          not share it for cross-context behavioral advertising. We share
          information only in these limited situations:
        </p>
        <ul>
          <li>
            <strong>Service providers</strong>, companies that help us run
            this website and our business, such as website hosting, email and
            analytics providers. They may access information only to perform
            services for us and are bound by confidentiality obligations;
          </li>
          <li>
            <strong>Professional advisers</strong>, lawyers, accountants and
            insurers, where reasonably necessary;
          </li>
          <li>
            <strong>Legal requirements</strong>, where required by law, court
            order or governmental authority, or to protect rights, safety and
            security;
          </li>
          <li>
            <strong>Business transfers</strong>, in connection with a merger,
            acquisition or sale of assets, in which case this Policy will
            continue to apply to your information.
          </li>
        </ul>

        <h2>6. Data retention</h2>
        <p>
          We keep personal information only as long as it is needed for the
          purposes described in this Policy, typically for as long as we are
          corresponding with you about an inquiry or engagement, plus any
          period required by law or needed to resolve disputes. When
          information is no longer needed, we delete it or anonymize it.
        </p>

        <h2>7. How we protect your information</h2>
        <p>
          We use administrative, technical and organizational safeguards
          appropriate to the nature of the information, including encryption of
          data in transit, access limited to the people who need it, and
          confidentiality commitments from everyone who works with us. No
          method of transmission or storage is completely secure, but we treat
          the information you share with us with the same care as our own.
        </p>

        <h2>8. Your rights and choices</h2>
        <p>
          Depending on where you live, you may have rights under applicable
          privacy laws, which can include the right to:
        </p>
        <ul>
          <li>Request access to the personal information we hold about you;</li>
          <li>Request that we correct or delete your personal information;</li>
          <li>
            Object to, or ask us to limit, certain processing of your
            information;
          </li>
          <li>Receive a copy of your information in a portable format.</li>
        </ul>
        <p>
          To exercise any of these rights, email us at{" "}
          <a href={`mailto:${site.email}`}>{site.email}</a>. We will respond
          within the time required by applicable law, and we will never treat
          you differently for exercising a privacy right.
        </p>

        <h2>9. Do Not Track and Global Privacy Control</h2>
        <p>
          Some browsers offer a &quot;Do Not Track&quot; signal. There is
          currently no common industry standard for responding to these
          signals, so this website does not respond to them. We do, however,
          honor the <strong>Global Privacy Control (GPC)</strong> signal: if
          your browser sends GPC, analytics cookies stay disabled
          automatically. We apply the same privacy practices described in this
          Policy to all visitors.
        </p>

        <h2>10. Children&apos;s privacy</h2>
        <p>
          This website is intended for business audiences and is not directed
          to children under 13. We do not knowingly collect personal
          information from children. If you believe a child has provided us
          with personal information, contact us at{" "}
          <a href={`mailto:${site.email}`}>{site.email}</a> and we will delete
          it.
        </p>

        <h2>11. Visitors from outside the United States</h2>
        <p>
          We are based in the United States, and the information you provide is
          processed and stored here. Where a project calls for it, specialists
          we engage outside the United States may also handle it, under
          confidentiality obligations. Privacy laws in those countries may
          differ from those of your country.
        </p>

        <h2>12. Third-party websites</h2>
        <p>
          This website contains links to third-party sites, such as our
          LinkedIn page. Those sites have their own privacy policies, and we
          are not responsible for their practices. We encourage you to review
          the privacy policy of any site you visit.
        </p>

        <h2>13. Changes to this Policy</h2>
        <p>
          We may update this Policy from time to time. When we do, we will
          revise the &quot;Last updated&quot; date at the top of this page. If
          we make material changes, we will provide a more prominent notice on
          this website. Your continued use of the site after an update means
          you accept the revised Policy.
        </p>

        <h2>14. Contact us</h2>
        <p>
          Questions, requests or concerns about this Policy or your personal
          information: email{" "}
          <a href={`mailto:${site.email}`}>{site.email}</a>. {site.legalName},{" "}
          {site.location}.
        </p>

        <LegalFootnote
          label="See also:"
          href="/terms"
          linkText="Terms of Use"
        />
      </LegalBody>
    </>
  );
}
