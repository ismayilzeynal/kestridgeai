import { ArrowUpRight } from "lucide-react";
import { LinkedInIcon } from "@/components/ui/LinkedInIcon";
import { site } from "@/lib/site";

export function LinkedInButton({
  label = "View our LinkedIn page",
  variant = "ghost",
  className = "",
}: {
  label?: string;
  variant?: "ghost" | "primary";
  className?: string;
}) {
  return (
    <a
      href={site.linkedin}
      target="_blank"
      rel="noopener noreferrer"
      className={`${variant === "primary" ? "btn-primary" : "btn-ghost"} group ${className}`}
    >
      <LinkedInIcon className="h-[17px] w-[17px]" />
      <span>{label}</span>
      <ArrowUpRight className="h-4 w-4 transition-transform duration-300 ease-smooth group-hover:translate-x-0.5 group-hover:-translate-y-0.5" />
    </a>
  );
}
