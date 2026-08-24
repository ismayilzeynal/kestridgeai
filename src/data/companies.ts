export type Company = {
  name: string;
  file: string;
};

// Organizations the four founders have worked at over their careers.
// Logos are the current marks taken from each organization's own LinkedIn
// page and stored locally in /public/logos so nothing depends on an
// external CDN. Order runs roughly most recent first.
export const companies: Company[] = [
  { name: "Avanade", file: "avanade" },
  { name: "EY", file: "ey" },
  { name: "Ecolab", file: "ecolab" },
  { name: "Community Health Systems", file: "community-health-systems" },
  { name: "Cleveland Clinic", file: "cleveland-clinic" },
  { name: "Aon", file: "aon" },
  { name: "Anthem", file: "anthem" },
  { name: "Constellation Energy", file: "constellation-energy" },
  { name: "Discovery", file: "discovery" },
  { name: "Infosys", file: "infosys" },
  { name: "Ipsos", file: "ipsos" },
  { name: "Sopra Steria", file: "sopra-steria" },
  { name: "Sears", file: "sears" },
  { name: "Northwestern University", file: "northwestern-university" },
  { name: "Clark University", file: "clark-university" },
  { name: "Robert Morris University", file: "robert-morris-university" },
  { name: "eiGroup", file: "eigroup" },
];
