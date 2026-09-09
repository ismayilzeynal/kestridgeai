namespace Kestridge.Api.Content;

// Property names are chosen so that the default camelCase serialization lands
// exactly on the field names in src/data/*.ts: q, a, file, cardLabel, what.
// That is what lets those files stay the frontend fallback with no adapter
// layer between them and the fetched payload.
public sealed record FaqPayload(string Q, string A);

public sealed record TeamPayload(string Name, string Initials, string Role, string Focus, string Photo);

public sealed record CompanyPayload(string Name, string File);

// "What" rather than "Detail": DeliveryStep in src/data/services.ts calls it
// what. The column is detail because that is what it is.
public sealed record ServiceStepPayload(string Phase, string Summary, string What);

// Id is the slug. No index property: Hero.tsx and Services.tsx render
// String(i + 1).padStart(2, "0"), so reordering renumbers for free and a
// stored number can never disagree with the position on the page.
public sealed record ServicePayload(
    string Id,
    string Name,
    string Tagline,
    string CardLabel,
    string Description,
    string Icon,
    string[] Highlights,
    ServiceStepPayload[] Steps);

public sealed record ContentPayload(
    DateTime GeneratedAt,
    FaqPayload[] Faq,
    TeamPayload[] Team,
    CompanyPayload[] Companies,
    ServicePayload[] Services);
