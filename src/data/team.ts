export type Member = {
  name: string;
  initials: string;
  role: string;
  focus: string;
  /** Local portrait. Empty string renders the initials avatar instead. */
  photo: string;
};

// The four founders. Personal profile links are deliberately not published
// here. Portraits are cropped square around the head: the card renders them
// in a 72px circle with object-cover, so anything off-centre loses the face.
export const team: Member[] = [
  {
    name: "Chingiz Abdilov",
    initials: "CA",
    role: "Founder",
    focus: "Technology operations and delivery",
    photo: "/team/chingiz-abdilov.jpg",
  },
  {
    name: "Faig Garayev",
    initials: "FG",
    role: "Founder",
    focus: "Technology strategy and IT security",
    photo: "/team/faig-garayev.jpg",
  },
  {
    name: "Sarvjeet",
    initials: "S",
    role: "Founder",
    focus: "Data, analytics and AI platforms",
    photo: "/team/sarvjeet.jpg",
  },
  {
    name: "Robert Tomczyk",
    initials: "RT",
    role: "Founder",
    focus: "Software engineering and integration",
    photo: "/team/robert-tomczyk.jpg",
  },
];
