-- Kestridge AI backend - seed the content tables with the copy the site
-- currently ships.
--
-- Run once as kestridge_migrator, AFTER ops/migrate.sql:
--   mysql -u kestridge_migrator -p kestridge < ops/05-seed-content.sql
--
-- The migration deliberately does not carry this. A migration that seeds runs
-- again on every fresh database including the test one, and it makes the site
-- copy part of the schema history, where changing a sentence means writing a
-- migration.
--
-- Every block is guarded on the table being empty, so a second run after real
-- edits does nothing at all. That is the only safety property this file has,
-- and it is the reason each INSERT is a SELECT with a NOT EXISTS rather than a
-- plain VALUES list.
--
-- Until this has run, /api/content returns empty arrays, getContent() rejects
-- them, and the site renders the constants in src/data/*.ts, which are the same
-- words. There is no visible transition.

-- ---------------------------------------------------------------- questions

INSERT INTO site_faq (question, answer, sort_order, updated_at, updated_by)
SELECT q, a, s, UTC_TIMESTAMP(6), 'seed' FROM (
    SELECT 'What does Kestridge AI do?' AS q,
           'We build AI, automation, IT security, and data analytics systems, connect them to what you already run, and support them after launch.' AS a,
           0 AS s
    UNION ALL SELECT 'What kinds of work do you take on?',
           'Common examples are entering incoming orders, matching invoices, routing approvals, and reporting from records you already keep.', 1
    UNION ALL SELECT 'Where is Kestridge AI based?',
           'Kestridge AI is based in Illinois. We also work with engineering specialists outside the United States.', 2
    UNION ALL SELECT 'How does a project start?',
           'A project starts with a consultation to gather your requirements. We then send a written plan and proposed solution for your approval.', 3
    UNION ALL SELECT 'How do you handle our data?',
           'Only authorized people can reach your data, and we use it only for your work. We sign a nondisclosure agreement for the project.', 4
    UNION ALL SELECT 'What size companies do you work with?',
           'We accept projects from companies of any size. Scope, schedule, and cost are set per project in the written plan.', 5
    UNION ALL SELECT 'How is cost determined?',
           'Cost depends on the scope of the work, which is set in the written plan. Nothing is committed until you approve it.', 6
    UNION ALL SELECT 'What do you need from us during a project?',
           'Access to the systems involved, and someone on your team who knows the process.', 7
) AS seed
WHERE NOT EXISTS (SELECT 1 FROM site_faq);

-- ----------------------------------------------------------------- founders

-- photo holds a basename. /api/content assembles /team/<photo>.jpg, so the
-- column can never point at another origin.
INSERT INTO site_team (name, initials, role, focus, photo, sort_order, updated_at, updated_by)
SELECT n, i, r, f, p, s, UTC_TIMESTAMP(6), 'seed' FROM (
    SELECT 'Chingiz Abdilov' AS n, 'CA' AS i, 'Founder' AS r,
           'Technology operations and delivery' AS f, 'chingiz-abdilov' AS p, 0 AS s
    UNION ALL SELECT 'Faig Garayev', 'FG', 'Founder',
           'Technology strategy and IT security', 'faig-garayev', 1
    UNION ALL SELECT 'Sarvjeet', 'S', 'Founder',
           'Data, analytics and AI platforms', 'sarvjeet', 2
    UNION ALL SELECT 'Robert Tomczyk', 'RT', 'Founder',
           'Software engineering and integration', 'robert-tomczyk', 3
) AS seed
WHERE NOT EXISTS (SELECT 1 FROM site_team);

-- -------------------------------------------------------------- client logos

-- The last three are images that are committed under /public/logos but absent
-- from the current array. They are seeded hidden rather than left out, so the
-- panel can reveal one without a developer. Their names are placeholders:
-- whoever reveals one should set the name it should carry on the page.
INSERT INTO site_companies (name, logo_file, hidden, sort_order, updated_at, updated_by)
SELECT n, lf, h, s, UTC_TIMESTAMP(6), 'seed' FROM (
    SELECT 'Avanade' AS n, 'avanade' AS lf, 0 AS h, 0 AS s
    UNION ALL SELECT 'EY', 'ey', 0, 1
    UNION ALL SELECT 'Ecolab', 'ecolab', 0, 2
    UNION ALL SELECT 'Community Health Systems', 'community-health-systems', 0, 3
    UNION ALL SELECT 'Cleveland Clinic', 'cleveland-clinic', 0, 4
    UNION ALL SELECT 'Aon', 'aon', 0, 5
    UNION ALL SELECT 'Anthem', 'anthem', 0, 6
    UNION ALL SELECT 'Constellation Energy', 'constellation-energy', 0, 7
    UNION ALL SELECT 'Discovery', 'discovery', 0, 8
    UNION ALL SELECT 'Infosys', 'infosys', 0, 9
    UNION ALL SELECT 'Ipsos', 'ipsos', 0, 10
    UNION ALL SELECT 'Sopra Steria', 'sopra-steria', 0, 11
    UNION ALL SELECT 'Sears', 'sears', 0, 12
    UNION ALL SELECT 'Northwestern University', 'northwestern-university', 0, 13
    UNION ALL SELECT 'Clark University', 'clark-university', 0, 14
    UNION ALL SELECT 'Robert Morris University', 'robert-morris-university', 0, 15
    UNION ALL SELECT 'eiGroup', 'eigroup', 0, 16
    UNION ALL SELECT 'Ministry of Taxes', 'ministry-of-taxes-az', 1, 17
    UNION ALL SELECT 'Sahara India', 'sahara-india', 1, 18
    UNION ALL SELECT 'Synovate', 'synovate', 1, 19
) AS seed
WHERE NOT EXISTS (SELECT 1 FROM site_companies);

-- ----------------------------------------------------------------- services

-- slug is the same string that appears as a DOM id, an ARIA target, the
-- contact form select value and a member of Kestridge:Contact:AllowedServices.
-- The panel cannot create or change one; these four are the whole set.
INSERT INTO site_services
    (slug, name, tagline, card_label, description, icon_name, sort_order, updated_at, updated_by)
SELECT sl, n, t, cl, d, ic, s, UTC_TIMESTAMP(6), 'seed' FROM (
    SELECT 'ai' AS sl, 'AI Solutions' AS n,
           'AI solutions that will improve business' AS t,
           'Forecasting and assistants for your staff' AS cl,
           'We analyze your business with you to identify the AI solution best suited to your needs - including ROI projections, an implementation roadmap, and security considerations.' AS d,
           'Brain' AS ic, 0 AS s
    UNION ALL SELECT 'analytics', 'Data Analytics',
           'Business reports from your data',
           'Dashboards and business reporting',
           'We pull your data together, check it for errors, and build the reports you need.',
           'BarChart3', 1
    UNION ALL SELECT 'automation', 'Automation',
           'Automation for repetitive work',
           'Routine steps run without manual work',
           'We document the current manual process and define the conditions under which each step should run automatically.',
           'Workflow', 2
    UNION ALL SELECT 'security', 'IT Security',
           'Security review and correction',
           'Assessment, access rules, and monitoring',
           'We review your systems, report what we find, and fix what you approve. We provide staff training and certification preparation.',
           'ShieldCheck', 3
) AS seed
WHERE NOT EXISTS (SELECT 1 FROM site_services);

-- Joined on slug rather than on a captured LAST_INSERT_ID, so this block is
-- correct whether or not the block above just ran.
INSERT INTO site_service_steps (service_id, phase, summary, detail, sort_order)
SELECT sv.id, seed.phase, seed.summary, seed.detail, seed.s
  FROM site_services sv
  JOIN (
    SELECT 'ai' AS slug, 'Consultation' AS phase, 'We gather requirements with your team.' AS summary,
           'We go through what the system needs to do, and confirm the requirements with you.' AS detail, 0 AS s
    UNION ALL SELECT 'ai', 'Solution Design', 'We write the plan and agree on it with you.',
           'You get the solution, scope, and schedule in writing, revised until you approve it.', 1
    UNION ALL SELECT 'ai', 'Build and Integration', 'We build the system and connect it.',
           'We build it, connect it to your existing systems, and test it with your team.', 2
    UNION ALL SELECT 'ai', 'Go Live and Support', 'We put the system into use and support it.',
           'The system goes into daily use. You choose whether we keep running it or hand it over with documentation.', 3

    UNION ALL SELECT 'analytics', 'Consultation', 'We gather requirements with your team.',
           'We go through what the system needs to do, and confirm the requirements with you.', 0
    UNION ALL SELECT 'analytics', 'Solution Design', 'We write the plan and agree on it with you.',
           'You get the solution, scope, and schedule in writing, revised until you approve it.', 1
    UNION ALL SELECT 'analytics', 'Build and Integration', 'We build the system and connect it.',
           'We build it, connect it to your existing systems, and test it with your team.', 2
    UNION ALL SELECT 'analytics', 'Go Live and Support', 'We put the system into use and support it.',
           'The system goes into daily use. You choose whether we keep running it or hand it over with documentation.', 3

    UNION ALL SELECT 'automation', 'Consultation', 'We gather requirements with your team.',
           'We go through what the system needs to do, and confirm the requirements with you.', 0
    UNION ALL SELECT 'automation', 'Solution Design', 'We write the plan and agree on it with you.',
           'You get the solution, scope, and schedule in writing, revised until you approve it.', 1
    UNION ALL SELECT 'automation', 'Build and Integration', 'We build the system and connect it.',
           'We build it, connect it to your existing systems, and test it with your team.', 2
    UNION ALL SELECT 'automation', 'Go Live and Support', 'We put the system into use and support it.',
           'The system goes into daily use. You choose whether we keep running it or hand it over with documentation.', 3

    UNION ALL SELECT 'security', 'Consultation', 'We review what you need to protect.',
           'We go through your current systems and record what needs protection and who can reach it today.', 0
    UNION ALL SELECT 'security', 'Assessment', 'We test the systems and list the weaknesses.',
           'You receive a written report of the weaknesses we find, ordered by how serious each one is.', 1
    UNION ALL SELECT 'security', 'Implementation', 'We fix the findings and set up controls.',
           'We correct the findings you choose, set up access rules, encryption, and backups, and confirm each item is closed.', 2
    UNION ALL SELECT 'security', 'Monitoring and Support', 'We monitor and respond to issues.',
           'We monitor the systems we run for you and act on the issues that come up.', 3
  ) AS seed ON seed.slug = sv.slug
WHERE NOT EXISTS (SELECT 1 FROM site_service_steps);

INSERT INTO site_service_highlights (service_id, text, sort_order)
SELECT sv.id, seed.text, seed.s
  FROM site_services sv
  JOIN (
    SELECT 'ai' AS slug, 'Predictive analytics and forecasting' AS text, 0 AS s
    UNION ALL SELECT 'ai', 'Machine learning', 1
    UNION ALL SELECT 'ai', 'Agentic AI and generative AI', 2
    UNION ALL SELECT 'ai', 'Deep learning', 3
    UNION ALL SELECT 'ai', 'Other services on request', 4

    UNION ALL SELECT 'analytics', 'Data cleanup and checks', 0
    UNION ALL SELECT 'analytics', 'Management dashboards', 1
    UNION ALL SELECT 'analytics', 'Scheduled reports', 2
    UNION ALL SELECT 'analytics', 'Other services on request', 3

    UNION ALL SELECT 'automation', 'Invoice matching and processing', 0
    UNION ALL SELECT 'automation', 'Approval routing', 1
    UNION ALL SELECT 'automation', 'Report generation', 2
    UNION ALL SELECT 'automation', 'Reminders and follow-ups', 3
    UNION ALL SELECT 'automation', 'Data entry from forms and emails', 4
    UNION ALL SELECT 'automation', 'Other services on request', 5

    UNION ALL SELECT 'security', 'Vulnerability assessment', 0
    UNION ALL SELECT 'security', 'Access control review', 1
    UNION ALL SELECT 'security', 'Configuration audits', 2
    UNION ALL SELECT 'security', 'Remediation', 3
    UNION ALL SELECT 'security', 'Staff training and certification preparation', 4
    UNION ALL SELECT 'security', 'Other services on request', 5
  ) AS seed ON seed.slug = sv.slug
WHERE NOT EXISTS (SELECT 1 FROM site_service_highlights);

-- Expected after a first run: 8 questions, 4 founders, 20 logos (17 visible),
-- 4 services, 16 steps, 21 highlights. ContentSeedTests asserts exactly that,
-- so a botched transcription fails the suite rather than the website.
SELECT (SELECT COUNT(*) FROM site_faq)                AS faq,
       (SELECT COUNT(*) FROM site_team)               AS team,
       (SELECT COUNT(*) FROM site_companies)          AS companies,
       (SELECT COUNT(*) FROM site_services)           AS services,
       (SELECT COUNT(*) FROM site_service_steps)      AS steps,
       (SELECT COUNT(*) FROM site_service_highlights) AS highlights;
