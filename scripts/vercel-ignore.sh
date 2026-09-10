#!/usr/bin/env bash
# Vercel skips the build when this exits 0, and builds when it exits non-zero.
# git diff --quiet exits 0 when nothing changed, which is exactly the skip case.
# Every error path falls through to exit 1 so an unknown state still builds.
set -u

# A redeploy of the same commit is somebody clicking Redeploy on purpose, and
# the only reason to do that is something this script cannot see: an
# environment variable, a cleared cache, a failed build to retry. Comparing the
# commit against itself finds no changes and would skip the build, so setting
# API_ORIGIN and pressing Redeploy did nothing at all until this existed.
[ "${VERCEL_GIT_PREVIOUS_SHA:-}" = "${VERCEL_GIT_COMMIT_SHA:-}" ] && exit 1

[ -z "${VERCEL_GIT_PREVIOUS_SHA:-}" ] && exit 1
git diff --quiet "$VERCEL_GIT_PREVIOUS_SHA" HEAD -- \
  ':(exclude)backend/' ':(exclude)senedler/' ':(exclude)Logo/' \
  ':(exclude)TAMAMLANACAQ-ISLER.txt' || exit 1
exit 0
