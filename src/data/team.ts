export type Member = {
  name: string;
  initials: string;
  role: string;
  focus: string;
  /** Local portrait. Empty string renders the initials avatar instead. */
  photo: string;
};

// The four founders. Names and portraits come from their own LinkedIn
// profiles. Personal profile links are deliberately not published here.
// Sarvjeet and Robert have no photo on LinkedIn, so their cards show initials
// until they send a portrait. Sarvjeet's surname is Lnu.
export const team: Member[] = [
  {
    name: "Sarvjeet Lnu",
    initials: "SL",
    role: "Founder",
    focus: "Data, analytics and AI platforms",
    photo: "",
  },
  {
    name: "Robert Tomczyk",
    initials: "RT",
    role: "Founder",
    focus: "Software engineering and integration",
    photo: "",
  },
  {
    name: "Faig Garayev",
    initials: "FG",
    role: "Founder",
    focus: "Technology strategy and IT security",
    photo: "/team/faig-garayev.jpg",
  },
  {
    name: "Chingiz Abdilov",
    initials: "CA",
    role: "Founder",
    focus: "Technology operations and delivery",
    photo: "/team/chingiz-abdilov.jpg",
  },
];
