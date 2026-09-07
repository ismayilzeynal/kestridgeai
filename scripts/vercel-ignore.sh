#!/usr/bin/env bash
# Vercel skips the build when this exits 0, and builds when it exits non-zero.
# git diff --quiet exits 0 when nothing changed, which is exactly the skip case.
# Every error path falls through to exit 1 so an unknown state still builds.
set -u
[ -z "${VERCEL_GIT_PREVIOUS_SHA:-}" ] && exit 1
git diff --quiet "$VERCEL_GIT_PREVIOUS_SHA" HEAD -- \
  ':(exclude)backend/' ':(exclude)senedler/' ':(exclude)Logo/' \
  ':(exclude)TAMAMLANACAQ-ISLER.txt' || exit 1
exit 0
