import { Reveal } from "@/components/ui/Reveal";

export function SectionHeading({
  eyebrow,
  title,
  description,
  align = "left",
  compact = false,
  className = "",
}: {
  eyebrow: string;
  title: React.ReactNode;
  description?: React.ReactNode;
  align?: "left" | "center";
  compact?: boolean;
  className?: string;
}) {
  const center = align === "center";
  return (
    <div
      className={`${center ? "mx-auto max-w-2xl text-center" : "max-w-2xl"} ${className}`}
    >
      <Reveal>
        <span className={`eyebrow ${center ? "justify-center" : ""}`}>{eyebrow}</span>
      </Reveal>
      <Reveal delay={0.06}>
        <h2
          className={`text-balance text-signal ${
            compact ? "mt-4 text-3xl sm:text-[2.5rem]" : "mt-5 text-4xl sm:text-5xl"
          }`}
        >
          {title}
        </h2>
      </Reveal>
      {description && (
        <Reveal delay={0.12}>
          <p
            className={`text-pretty leading-relaxed text-muted ${
              compact ? "mt-3.5 text-[16px]" : "mt-5 text-lg"
            }`}
          >
            {description}
          </p>
        </Reveal>
      )}
    </div>
  );
}
