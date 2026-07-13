export type Member = {
  name: string;
  role: string;
  focus: string;
  photo: string;
  linkedin: string;
};

// NOTE: portraits are placeholder faces from randomuser.me until real
// team photography is approved. Names/roles are illustrative.
export const team: Member[] = [
  {
    name: "Daniel Hart",
    role: "Principal AI Engineer",
    focus: "Decision systems & applied ML",
    photo: "https://randomuser.me/api/portraits/men/32.jpg",
    linkedin: "https://www.linkedin.com/company/aivanta",
  },
  {
    name: "Sofia Marchetti",
    role: "Head of Security",
    focus: "Threat modeling & compliance",
    photo: "https://randomuser.me/api/portraits/women/44.jpg",
    linkedin: "https://www.linkedin.com/company/aivanta",
  },
  {
    name: "Marcus Bell",
    role: "Automation Lead",
    focus: "Systems orchestration & integration",
    photo: "https://randomuser.me/api/portraits/men/64.jpg",
    linkedin: "https://www.linkedin.com/company/aivanta",
  },
  {
    name: "Priya Nair",
    role: "Analytics Lead",
    focus: "Data pipelines & visualization",
    photo: "https://randomuser.me/api/portraits/women/68.jpg",
    linkedin: "https://www.linkedin.com/company/aivanta",
  },
  {
    name: "Jonas Weber",
    role: "Solutions Architect",
    focus: "Cloud infrastructure & delivery",
    photo: "https://randomuser.me/api/portraits/men/76.jpg",
    linkedin: "https://www.linkedin.com/company/aivanta",
  },
  {
    name: "Amara Okafor",
    role: "ML Research Engineer",
    focus: "LLM systems & retrieval",
    photo: "https://randomuser.me/api/portraits/women/90.jpg",
    linkedin: "https://www.linkedin.com/company/aivanta",
  },
  {
    name: "Elena Vasquez",
    role: "Client Engagement Lead",
    focus: "Requirements & delivery management",
    photo: "https://randomuser.me/api/portraits/women/12.jpg",
    linkedin: "https://www.linkedin.com/company/aivanta",
  },
  {
    name: "Tomáš Novák",
    role: "Senior Security Engineer",
    focus: "Monitoring & incident response",
    photo: "https://randomuser.me/api/portraits/men/41.jpg",
    linkedin: "https://www.linkedin.com/company/aivanta",
  },
];
