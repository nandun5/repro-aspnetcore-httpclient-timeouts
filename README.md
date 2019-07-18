# repro-aspnetcore-httpclient-timeouts
This repo has different methods of making http requests concurrently.

was originally using and incorrect implementation using Parallel.ForEach which was causing the app to exhaust the ephemeral ports causing timeouts.
