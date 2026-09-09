import { revalidatePath } from "next/cache";

// Without this an edit in the admin panel is live within the 300 second ISR
// window. With it, on the next request after the save.
//
// Cuttable: remove the route, drop Kestridge:Admin:RevalidateUrl from
// appsettings.Production.json, and everything else still works. The panel then
// reports "Saved. The website did not confirm the update." and says the change
// appears within 5 minutes, which is true.
//
// One header, no cookie, no session. The secret lives in Vercel's project
// environment and in a 0640 file on the API host, and in neither case in git.
export async function POST(request: Request) {
  const secret = process.env.REVALIDATE_SECRET;

  // Unconfigured means closed, not open. Otherwise a deployment that forgot
  // the variable would accept a purge from anyone who found the URL.
  if (!secret || request.headers.get("x-revalidate-key") !== secret) {
    return new Response("no", { status: 401 });
  }

  revalidatePath("/");

  return Response.json({ ok: true });
}
